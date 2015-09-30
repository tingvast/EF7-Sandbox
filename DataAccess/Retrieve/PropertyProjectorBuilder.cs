using DataAccess.Interaces;
using EF7;
using LatticeUtils;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Caching;

namespace DataAccess
{
    public class PropertyProjectorBuilder<T> : IPropertyProjectorBuilder<T>, IIncludePropertySelector<T> where T : class, IEntity
    {
        private int _id;

        private EF7BloggContext _context;

        public PropertyProjectorBuilder()
        {
            AllProjections = new Projections();
        }

        public PropertyProjectorBuilder(int id)
            : this()
        {
            this._id = id;
        }

        public PropertyProjectorBuilder(int id, EF7BloggContext context)
            : this()
        {
            this._id = id;
            this._context = context;
        }

        public IProjections AllProjections
        {
            get; private set;
        }

        public IPropertyProjectorBuilder<T> Select(
            params Expression<Func<T, dynamic>>[] p1)
        {
            AllProjections.Projection.AddRange(p1);

            return this;
        }

        IPropertyProjectorBuilder<T> IPropertyProjectorBuilder<T>.Include<TProperty>(
            Expression<Func<T, dynamic>> navigationPropery,
            params Expression<Func<TProperty, dynamic>>[] selectedProperties)
        {
            var propertyInfo = (PropertyInfo)ParameterHelper.GetMemberExpression(navigationPropery as LambdaExpression).Member;

            var propertyNameOfReferer = propertyInfo.Name;

            var navigationProperty = new NavigationProperty(
                   propertyNameOfReferer,
                   typeof(TProperty).Name,
                   typeof(TProperty));

            navigationProperty.Projections.AddRange(selectedProperties);

            AllProjections.NavigationPropertiesProjections.Add(navigationProperty);

            // TODO: Maybe remove any tracked entities of the navigation property type (and id), when
            return this;
        }

        public IPropertySeletor<T> Build()
        {
            Expression<Func<T, dynamic>> fullProjectionLambda;
            var cacheKey = AllProjections.CacheKey;
            var cacheValue = MemoryCache.Default[cacheKey];

            if (cacheValue == null)
            {
                var projectedEntityAnonymousType = CreateLampbda(out fullProjectionLambda);
                cacheValue = new PropertyProjector<T>(
                    fullProjectionLambda,
                    AllProjections,
                    projectedEntityAnonymousType);

                MemoryCache.Default[cacheKey] = cacheValue;
            }

            PropertyProjector<T> ret = null;
            try
            {
                ret = (PropertyProjector<T>)cacheValue;
            }
            catch (Exception e)
            {
                Debugger.Break();
            }

            return ret; ;
        }

        #region Private

        private Type CreateLampbda
            (out Expression<Func<T, dynamic>> lambd11a1)
        {
            List<KeyValuePair<string, Type>> anonymousTypeProperties = new List<KeyValuePair<string, Type>>();
            List<Expression> anonymousTypePropertiesValues = new List<Expression>();
            ParameterExpression lambdaParameter = Expression.Parameter(typeof(T), "p");
            foreach (var projection in AllProjections.Projection)
            {
                var projectionLambda = projection as LambdaExpression;

                MemberExpression member = ParameterHelper.GetMemberExpression(projectionLambda);

                var propertyInfo = (PropertyInfo)member.Member;
                var propertyName = propertyInfo.Name;
                var propertyType = propertyInfo.PropertyType;

                var memberAccess = Expression.Property(lambdaParameter, propertyName);

                anonymousTypeProperties.Add(new KeyValuePair<string, Type>(propertyName, propertyType));
                anonymousTypePropertiesValues.Add(memberAccess);
            }

            foreach (var navigationProperty in AllProjections.NavigationPropertiesProjections)
            {
                var navigationProperyType = navigationProperty.Type;

                // Creates the <T>.where(p => p.id == id) part of the expression
                MethodCallExpression whereCallExpression = CreateWhereCall<T>(_id, navigationProperyType);

                ParameterExpression p1 = Expression.Parameter(navigationProperyType, "p1");
                var navigationPropertyAnoymousTypeProperties = new List<KeyValuePair<string, Type>>();
                List<MemberExpression> navigationPropertyAnoymousTypePropertiesValues = new List<MemberExpression>();
                foreach (var projection in navigationProperty.Projections)
                {
                    var navigationPropertyProjection = projection as LambdaExpression;

                    MemberExpression member = ParameterHelper.GetMemberExpression(navigationPropertyProjection);

                    var propertyInfo = (PropertyInfo)member.Member;
                    var propertyName = propertyInfo.Name;
                    var propertyType = propertyInfo.PropertyType;

                    var memberAccess = Expression.Property(p1, propertyName);

                    navigationPropertyAnoymousTypeProperties.Add(new KeyValuePair<string, Type>(propertyName, propertyType));
                    navigationPropertyAnoymousTypePropertiesValues.Add(memberAccess);
                }

                var anonymousTypeOfNavigationPropertyProjection =
                    AnonymousTypeUtils.CreateType(navigationPropertyAnoymousTypeProperties);
                Type typeOfSubProj = null;
                var anonymousTypeOfNavigationPropertyProjectionConstructor = anonymousTypeOfNavigationPropertyProjection
                    .GetConstructor(navigationPropertyAnoymousTypeProperties.Select(kv => kv.Value).ToArray());
                typeOfSubProj = anonymousTypeOfNavigationPropertyProjectionConstructor.ReflectedType;

                var selectMethod = typeof(Queryable).GetMethods().Where(m => m.Name == "Select").ToList()[0];
                var genericSelectMethod = selectMethod.MakeGenericMethod(navigationProperyType, typeOfSubProj);

                var newInstanceOfTheGenericType = Expression.New(anonymousTypeOfNavigationPropertyProjectionConstructor,
                    navigationPropertyAnoymousTypePropertiesValues);

                var projectionLamdba = Expression.Lambda(newInstanceOfTheGenericType, p1);

                MethodCallExpression selctCallExpression = Expression.Call(
                    genericSelectMethod,
                    whereCallExpression,
                    projectionLamdba);

                var provider = ((IQueryable)_context.Set<T>()).Provider;
                // TODO, Is it ok to assume navigation properties has the same provider?
                var theMethods = typeof(IQueryProvider).GetMethods();
                var createQMethd = theMethods.Where(name => name.Name == "CreateQuery").ToList()[1];
                var speciifMethod =
                    createQMethd.MakeGenericMethod(anonymousTypeOfNavigationPropertyProjectionConstructor.ReflectedType);
                var navigationProppertyQueryWithProjection1 = speciifMethod.Invoke(provider, new object[] { selctCallExpression });

                Type genericFunc = typeof(IEnumerable<>);
                Type funcOfTypeOfSubProj = genericFunc.MakeGenericType(typeOfSubProj);

                var allMethodsOnEnumerableClass = typeof(Enumerable).GetMethods();
                var genericToListMethod = allMethodsOnEnumerableClass.Where(m => m.Name == "ToList").ToList()[0];

                var toListOfTypeOfSubProj = genericToListMethod.MakeGenericMethod(typeOfSubProj);

                MethodCallExpression toListExpression11 = Expression.Call(
                    toListOfTypeOfSubProj,
                    Expression.Constant(navigationProppertyQueryWithProjection1));

                anonymousTypeProperties.Add(new KeyValuePair<string, Type>(navigationProperty.Name, funcOfTypeOfSubProj));
                anonymousTypePropertiesValues.Add(toListExpression11);
            }

            var projectedEntityAnonymousType = AnonymousTypeUtils.CreateType(anonymousTypeProperties);

            var constructor = projectedEntityAnonymousType.GetConstructor(anonymousTypeProperties.Select(p => p.Value).ToArray());

            var anonymousTypeInstance = Expression.New(constructor, anonymousTypePropertiesValues);

            lambd11a1 = Expression.Lambda<Func<T, dynamic>>(anonymousTypeInstance, lambdaParameter);
            return projectedEntityAnonymousType;
        }

        private static MethodInfo GetGenericMethod(Type declaringType, string methodName, Type[] typeArgs, params Type[] argTypes)
        {
            foreach (var m in from m in declaringType.GetMethods()
                              where m.Name == methodName
                                  && typeArgs.Length == m.GetGenericArguments().Length
                                  && argTypes.Length == m.GetParameters().Length
                              select m.MakeGenericMethod(typeArgs))
            {
                if (m.GetParameters().Select((p, i) => p.ParameterType == argTypes[i]).All(x => x == true))
                    return m;
            }

            return null;
        }

        private MethodCallExpression CreateWhereCall<T>(int id, Type navigationProperyType) where T : class, IEntity
        {
            ParameterExpression pe = Expression.Parameter(navigationProperyType, "nav");

            var foreigKeys = _context.Model.GetEntityType(navigationProperyType).GetForeignKeys();
            var thePrincipalEntityKey = foreigKeys.Single(fk => fk.PrincipalEntityType.ClrType == typeof(T));
            var thePrincipalEntityKeyName = thePrincipalEntityKey.Properties[0].Name;

            Expression left = Expression.PropertyOrField(pe, thePrincipalEntityKeyName);
            Expression right = Expression.Constant(id, typeof(int?));
            Expression e1 = Expression.Equal(left, right);

            var constructorinfo = typeof(EntityQueryable<>).MakeGenericType(navigationProperyType).GetConstructors()[0];

            var newstsy = constructorinfo.Invoke(new[] { _context.GetService<IEntityQueryProvider>() });
            Type func11 = typeof(EntityQueryable<>);
            Type generic23233 = func11.MakeGenericType(navigationProperyType);
            var nwtwDynamically = Expression.Constant(newstsy, generic23233);

            Type func1111 = typeof(IQueryable<>);
            Type generic2323311 = func1111.MakeGenericType(navigationProperyType);

            Type func11111 = typeof(Func<,>);
            Type generic232331123 = func11111.MakeGenericType(navigationProperyType, typeof(bool));
            Type funxs = typeof(Expression<>);
            Type skljslk = funxs.MakeGenericType(generic232331123);

            var m1 = GetGenericMethod(typeof(Queryable), "Where", new[] { navigationProperyType }, generic2323311, skljslk);

            MethodCallExpression whereCallExpression = Expression.Call(
                        m1,
                        nwtwDynamically,//nwtw
                        Expression.Lambda(e1, new ParameterExpression[] { pe }));
            return whereCallExpression;
        }

        #endregion Private

        #region Obsolete

        [Obsolete]
        IPropertyProjectorBuilder<T> IPropertyProjectorBuilder<T>.IncludeOld<TProperty>(
            Expression<Func<T, dynamic>> navigationPropery,
            params Expression<Func<TProperty, dynamic>>[] properties)
        {
            var navigationPropertyProjection = navigationPropery as LambdaExpression;

            var member = navigationPropertyProjection.Body as MemberExpression;

            var propertyInfo = (PropertyInfo)member.Member;

            var propertyNameOfReferer = propertyInfo.Name;

            var navigationProperty = new NavigationProperty(
                   propertyNameOfReferer,
                   typeof(TProperty).Name,
                   typeof(TProperty));

            navigationProperty.Projections.AddRange(properties);

            AllProjections.NavigationPropertiesProjections.Add(navigationProperty);

            return this;
        }

        #endregion Obsolete
    }
}