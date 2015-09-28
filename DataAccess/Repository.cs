using AutoMapper;
using Core;
using DataAccess.Interaces;
using EF7;
using LatticeUtils;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Query;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


namespace DataAccess
{
    public class Repository : IRepository
    {
        private EF7BloggContext context;

        public Repository(EF7BloggContext context)
        {
            this.context = context;
        }

        #region Create

        public T Create<T>(T entity) where T : class, IEntity
        {
            context.Set<T>().Add(entity);

            return entity;
        }

        public List<T> CreateMany<T>(params T[] entities) where T : class, IEntity
        {
            context.Set<T>().AddRange(entities);

            return entities.ToList();
        }

        public T CreateGraph<T>(T entityWithRelations) where T : class, IEntity
        {
            context.ChangeTracker.TrackGraph(entityWithRelations, (e) => e.State = EntityState.Added);

            return entityWithRelations;
        }

        #endregion Create

        #region Retrieve

        public T RetrieveById<T>(int id, IPropertyProjectorBuilder<T> selectedProperties) where T : class, IEntity
        {
            // The query will be projected onto an anonymous type.
            List<KeyValuePair<string, Type>> anonymousTypeProperties = new List<KeyValuePair<string, Type>>();
            List<Expression> anonymousTypePropertiesValues = new List<Expression>();
            ParameterExpression lambdaParameter = Expression.Parameter(typeof(T), "p");
            foreach (var projection in selectedProperties.AllProjections.Projection)
            {
                var projectionLambda = projection as LambdaExpression;

                MemberExpression member = CreateMemberExpression(projectionLambda);

                var propertyInfo = (PropertyInfo)member.Member;
                var propertyName = propertyInfo.Name;
                var propertyType = propertyInfo.PropertyType;

                var memberAccess = Expression.Property(lambdaParameter, propertyName);

                anonymousTypeProperties.Add(new KeyValuePair<string, Type>(propertyName, propertyType));
                anonymousTypePropertiesValues.Add(memberAccess);
            }

            foreach (var navigationProperty in selectedProperties.AllProjections.NavigationPropertiesProjections)
            {
                var navigationProperyType = navigationProperty.Type;

                // Creates the <T>.where(p => p.id == id) part of the expression
                MethodCallExpression whereCallExpression = CreateWhereCall<T>(id, navigationProperyType);

                ParameterExpression p1 = Expression.Parameter(navigationProperyType, "p1");
                var navigationPropertyAnoymousTypeProperties = new List<KeyValuePair<string, Type>>();
                List<MemberExpression> navigationPropertyAnoymousTypePropertiesValues = new List<MemberExpression>();
                foreach (var projection in navigationProperty.Projections)
                {
                    var navigationPropertyProjection = projection as LambdaExpression;

                    MemberExpression member = CreateMemberExpression(navigationPropertyProjection);

                    var propertyInfo = (PropertyInfo)member.Member;
                    var propertyName = propertyInfo.Name;
                    var propertyType = propertyInfo.PropertyType;

                    var memberAccess = Expression.Property(p1, propertyName);

                    navigationPropertyAnoymousTypeProperties.Add(new KeyValuePair<string, Type>(propertyName, propertyType));
                    navigationPropertyAnoymousTypePropertiesValues.Add(memberAccess);
                }

                var anonymousTypeOfNavigationPropertyProjection = AnonymousTypeUtils.CreateType(navigationPropertyAnoymousTypeProperties);
                Type typeOfSubProj = null;
                var anonymousTypeOfNavigationPropertyProjectionConstructor = anonymousTypeOfNavigationPropertyProjection
                    .GetConstructor(navigationPropertyAnoymousTypeProperties.Select(kv => kv.Value).ToArray());
                typeOfSubProj = anonymousTypeOfNavigationPropertyProjectionConstructor.ReflectedType;

                var selectMethod = typeof(Queryable).GetMethods().Where(m => m.Name == "Select").ToList()[0];
                var genericSelectMethod = selectMethod.MakeGenericMethod(navigationProperyType, typeOfSubProj);

                var newInstanceOfTheGenericType = Expression.New(anonymousTypeOfNavigationPropertyProjectionConstructor, navigationPropertyAnoymousTypePropertiesValues);

                var projectionLamdba = Expression.Lambda(newInstanceOfTheGenericType, p1);

                MethodCallExpression selctCallExpression = Expression.Call(
                      genericSelectMethod,
                      whereCallExpression,
                      projectionLamdba);

                var provider = ((IQueryable)context.Set<T>()).Provider; // TODO, Is it ok to assube navigation properties has the same provider?
                var theMethods = typeof(IQueryProvider).GetMethods();
                var createQMethd = theMethods.Where(name => name.Name == "CreateQuery").ToList()[1];
                var speciifMethod = createQMethd.MakeGenericMethod(anonymousTypeOfNavigationPropertyProjectionConstructor.ReflectedType);
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

            Type projectedEntityAnonymousType = AnonymousTypeUtils.CreateType(anonymousTypeProperties);

            var constructor = projectedEntityAnonymousType.GetConstructor(anonymousTypeProperties.Select(p => p.Value).ToArray());

            var anonymousTypeInstance = Expression.New(constructor, anonymousTypePropertiesValues);

            Expression<Func<T, dynamic>> lambd11a1 = Expression.Lambda<Func<T, dynamic>>(anonymousTypeInstance, lambdaParameter);

            var projectedEntity = context.Set<T>()
                .AsNoTracking()
                .Where(e => e.Id == id)
                .Select(lambd11a1)
                .Single();

            var mainEntity = Mapper.DynamicMap<T>(projectedEntity);
            mainEntity.Id = id;

            if (selectedProperties.AllProjections.NavigationPropertiesProjections.Count() > 0)
            {
                MaterializeNavigationProperties<T>(mainEntity, projectedEntity, projectedEntityAnonymousType, selectedProperties);
            }

            var alreadytrackedentity = context.ChangeTracker.Entries<T>().Where(e => e.Entity.Id == id).SingleOrDefault();

            if (alreadytrackedentity != null)
            {
                // Remove it from tracking
                alreadytrackedentity.State = EntityState.Detached;
            }
            context.Attach(mainEntity);
            return mainEntity;
        }

        public T RetrieveByIdNew<T>(int id, IPropertyProjector<T> projection) where T : class, IEntity
        {
            var projectedEntity = context.Set<T>()
                .AsNoTracking()
                .Where(e => e.Id == id)
                .Select(projection.Expression)
                .Single();

            var mainEntity = Mapper.DynamicMap<T>(projectedEntity);
            mainEntity.Id = id;

            if (projection.AllProjections.NavigationPropertiesProjections.Any())
            {
                MaterializeNavigationPropertiesNew<T>(mainEntity, projectedEntity, projection.ProjectedEntityAnonymousType, projection.AllProjections);
            }

            var alreadytrackedentity = context
                .ChangeTracker.Entries<T>()
                .Where(e => e.Entity.Id == id)
                .SingleOrDefault();

            if (alreadytrackedentity != null)
            {
                // Remove it from tracking
                alreadytrackedentity.State = EntityState.Detached;
            }
            context.Attach(mainEntity);
            return mainEntity;
        }

        public IEnumerable<T> Retrieve<T>(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy = null) where T : class, IEntity
        {
            var retrievedEntity = context.Set<T>().Where(predicate).OrderBy(orderBy);
            return retrievedEntity;
        }

        public T RetrieveReadonly<T, TResult>(int id, Func<T, TResult> selectedProperties) where T : class, IEntity
        {
            var entiry = context.Set<T>().AsNoTracking().Where(e => e.Id == id).Select(selectedProperties).Single();
            var retEntity = Mapper.DynamicMap<T>(entiry);
            return retEntity;
        }

        #endregion Retrieve

        #region Update

        public T Update<T>(T entity) where T : class, IEntity
        {
            context.ChangeTracker.AutoDetectChangesEnabled = true;
            context.Set<T>().Update(entity);

            context.ChangeTracker.AutoDetectChangesEnabled = false;
            return entity;
        }

        public T Update<T, TResult>(T entity, Expression<Func<T, TResult>> selectedProperties) where T : class, IEntity
        {
            context.ChangeTracker.AutoDetectChangesEnabled = true;
            context.Entry(entity).State = EntityState.Modified;
            string prop = ResolvePropertyName(selectedProperties);

            var allProperties = context.Entry(entity).Metadata.GetProperties().ToList();
            var toRenove = allProperties.Find(p => p.Name == prop);
            allProperties.Remove(toRenove);

            var entity1 = context.Entry(entity);
            allProperties.ForEach(p =>
            {
                entity1.Property(p.Name).IsModified = false;
                int i = 0;
            });

            context.Entry(entity).Property(prop).IsModified = true;

            context.ChangeTracker.AutoDetectChangesEnabled = false;
            return entity;
        }

        public T UpdateGraph<T>(T entityWithRelations) where T : class, IEntity
        {
            //context.ChangeTracker.E.Entries.UpdateGraph<T>(entityWithRelations);

            context.ChangeTracker.TrackGraph(entityWithRelations, (e) =>
            {
                if (((EntityState)e.State) == EntityState.Detached)
                {
                    if (e.IsKeySet)
                    {
                        e.State = EntityState.Modified;
                    }
                    else
                    {
                        e.State = EntityState.Added;
                    }
                }
            });
            //context.Update<T>(entityWithRelations);

            return entityWithRelations;
        }

        #endregion Update

        #region Delete

        public void Delete<T>(T entity) where T : class, IEntity
        {
            var keys = context.Entry<T>(entity).Metadata.GetForeignKeys();

            context.Remove(entity);
        }

        #endregion Delete

        #region Private Methods

        private static MemberExpression CreateMemberExpression(LambdaExpression projectionLambda)
        {
            MemberExpression member;
            switch (projectionLambda.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    var ue = projectionLambda.Body as UnaryExpression;
                    member = ((ue != null) ? ue.Operand : null) as MemberExpression;
                    break;

                default:
                    member = projectionLambda.Body as MemberExpression;
                    break;
            }

            return member;
        }

        private static void MaterializeNavigationProperties<T>(
        dynamic mainEntity,
        dynamic projectedEntity,
        Type projectedEntityAnonymousType,
        IPropertyProjectorBuilder<T> selectedProperties) where T : class, IEntity
        {
            Type genericListType = typeof(List<>);
            var genericDynamicMapper = typeof(Mapper).GetMethods(BindingFlags.Static | BindingFlags.Public).Where(m => m.Name == "DynamicMap").ToList()[2];

            foreach (var projection in selectedProperties.AllProjections.NavigationPropertiesProjections)
            {
                var navigationPropertyOnMainEntity = typeof(T).GetProperty(projection.ReferingPropertyName);
                var navigationPropertyOnProjectedAnonymousType = projectedEntityAnonymousType.GetProperty(projection.Name);

                var propertValues = (IEnumerable)navigationPropertyOnProjectedAnonymousType.GetValue(projectedEntity);

                Type listOfTypeProjectionType = genericListType.MakeGenericType(new[] { projection.Type });

                var mapperOfProjectionType = genericDynamicMapper.MakeGenericMethod(projection.Type);
                IList navigationPropertyList = (IList)Activator.CreateInstance(listOfTypeProjectionType);
                foreach (var value in propertValues)
                {
                    var valueOfNavigationPropertyProjection = mapperOfProjectionType.Invoke(null, new object[] { value });

                    navigationPropertyList.Add(valueOfNavigationPropertyProjection);
                }

                navigationPropertyOnMainEntity.SetValue(mainEntity, navigationPropertyList);
            }
        }


        private static void MaterializeNavigationPropertiesNew<T>(
            dynamic mainEntity,
            dynamic projectedEntity,
            Type projectedEntityAnonymousType,
            IProjections projections) where T : class, IEntity
        {
            Type genericListType = typeof(List<>);
            var genericDynamicMapper = typeof(Mapper).GetMethods(BindingFlags.Static | BindingFlags.Public).Where(m => m.Name == "DynamicMap").ToList()[2];

            foreach (var projection in projections.NavigationPropertiesProjections)
            {
                var navigationPropertyOnMainEntity = typeof(T).GetProperty(projection.ReferingPropertyName);
                var navigationPropertyOnProjectedAnonymousType = projectedEntityAnonymousType.GetProperty(projection.Name);

                var propertValues = (IEnumerable)navigationPropertyOnProjectedAnonymousType.GetValue(projectedEntity);

                Type listOfTypeProjectionType = genericListType.MakeGenericType(new[] { projection.Type });

                var mapperOfProjectionType = genericDynamicMapper.MakeGenericMethod(projection.Type);
                IList navigationPropertyList = (IList)Activator.CreateInstance(listOfTypeProjectionType);
                foreach (var value in propertValues)
                {
                    var valueOfNavigationPropertyProjection = mapperOfProjectionType.Invoke(null, new object[] { value });

                    navigationPropertyList.Add(valueOfNavigationPropertyProjection);
                }

                navigationPropertyOnMainEntity.SetValue(mainEntity, navigationPropertyList);
            }
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

        private string ResolvePropertyName<T, TResult>(Expression<Func<T, TResult>> expression)
        {
            try
            {
                if (expression == null)
                {
                    throw new ArgumentNullException("propertyExpression");
                }

                var memberExpression = expression.Body as MemberExpression;
                if (memberExpression == null)
                {
                    throw new ArgumentException("The expression is not a member access expression.", "propertyExpression");
                }

                var property = memberExpression.Member as PropertyInfo;
                if (property == null)
                {
                    throw new ArgumentException("The member access expression does not access a property.", "propertyExpression");
                }

                var getMethod = property.GetGetMethod(true);
                if (getMethod.IsStatic)
                {
                    throw new ArgumentException("The referenced property is a static property.", "propertyExpression");
                }
                return memberExpression.Member.Name;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        private MethodCallExpression CreateWhereCall<T>(int id, Type navigationProperyType) where T : class, IEntity
        {
            ParameterExpression pe = Expression.Parameter(navigationProperyType, "nav");

            var foreigKeys = context.Model.GetEntityType(navigationProperyType).GetForeignKeys();
            var thePrincipalEntityKey = foreigKeys.Single(fk => fk.PrincipalEntityType.ClrType == typeof(T));
            var thePrincipalEntityKeyName = thePrincipalEntityKey.Properties[0].Name;

            Expression left = Expression.PropertyOrField(pe, thePrincipalEntityKeyName);
            Expression right = Expression.Constant(id, typeof(int?));
            Expression e1 = Expression.Equal(left, right);

            var constructorinfo = typeof(EntityQueryable<>).MakeGenericType(navigationProperyType).GetConstructors()[0];

            var newstsy = constructorinfo.Invoke(new[] { context.GetService<IEntityQueryProvider>() });
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

        #endregion Private Methods

        #region Obsolete

        public Blog RetrieveBlogNonGeneric(int id)
        {
            //var m11 = (from e in context.Set<Meeting>()
            //    join p in context.Set<PreRegistration>() on e.Id equals p.MeetingId
            //    where e.Id == id
            //    select new {e.Location, p.Text});

            var entiry = context.Set<Blog>().
                Include(m => m.Posts).
                Where(m => m.Id == id).
                Select(m => new { l = m.Name, p = m.Posts.Count }).
                Single();

            var ddd = (from pp in context.Set<Post>() where pp.BlogId == id select new { pp.Text });
            var entiry44 = context.Set<Blog>().
                //Include(m => m.PreRegistrations).
                Where(m => m.Id == id).
                Select(m => new { m.Name, yy = ddd.ToList() }).
                Single();

            var entiry11 = context.Set<Blog>().
                Include(m => m.Posts).
                Where(m => m.Id == id).
                //Select(m => new { m.Location, yy = m.PreRegistrations.Select(pre => new { pre.Text }) }).
                Single();

            //var entiry12 = context.Set<Meeting>().
            //    Include(m => m.PreRegistrations.Select(p => new { p.Text })).
            //    Where(m => m.Id == id).
            //    //Select(m => new { m.Location, yy = m.PreRegistrations.Select(pre => new { pre.Text }) }).
            //    Single();

            var retEntity = Mapper.DynamicMap<Blog>(entiry);
            //var entiry = context.Set<Meeting>().
            //    //Include(m => m.PreRegistrations).
            //    Where(m => m.ID == id).
            //    Select(m => new { m = m.Location }).//, k = m.PreRegistrations.Select(p => new { p.Text }) }).
            //    //Select(m => new { Meeting = new { m.Location }, PreRegs = m.PreRegistrations }).
            //    Single();
            //var kk = from m in context.Set<Meeting>()
            //         where m.ID == id
            //         select new { kalle = m.PreRegistrations.Select(p => p.Text)};

            //var ff = kk.ToList();
            //var retEntity = Mapper.DynamicMap<Meeting>(entiry);
            //context.ChangeTracker.Entries<Meeting>().First().StateEntry.
            //var navprop = context.Model.GetEntityType(typeof(Meeting))..Navigations.First();
            //var nav = navprop.EntityType.TryGetNavigation("kkj");
            //var en = nav.EntityType;
            //var preregistrations = context.Preregistrations.Where(p => p.MeetingID == id);

            //var retEntity = Mapper.Map<Meeting>(entiry);
            return retEntity;
        }

        public IPropertyProjectorBuilder<T> CreatePropertyProjectorBuilder<T>(T entity) where T : class, IEntity
        {
            if (entity == null) return new PropertyProjectorBuilder<T>();

            return new PropertyProjectorBuilder<T>(entity.Id, context);
        }
    

        public T RetrieveObsolete<T, TResult>(int id, Expression<Func<T, TResult>> selectedProperties) where T : class, IEntity
        {
            //var arguments = selectedProperties.P.Body.
            var body = (NewExpression)selectedProperties.Body;

            List<KeyValuePair<string, Type>> projs = new List<KeyValuePair<string, Type>>();
            List<Expression> exprs = new List<Expression>();

            object navigationProppertyQueryWithProjection1 = null;
            ParameterExpression pe = Expression.Parameter(typeof(T), "p");
            Type typeOfSubProj = null;
            foreach (var arg in body.Arguments)
            {
                if (arg.NodeType == ExpressionType.MemberAccess)
                {
                    var body1 = arg as MemberExpression;

                    var propertyInfo = (PropertyInfo)body1.Member;
                    var nameOfTheProperty = propertyInfo.Name;
                    var typeOfTheProperty = propertyInfo.PropertyType;

                    var memberAccess1 = Expression.Property(pe, nameOfTheProperty);
                    exprs.Add(memberAccess1);

                    projs.Add(new KeyValuePair<string, Type>(nameOfTheProperty, typeOfTheProperty));
                }

                if (arg.NodeType == ExpressionType.Call)
                {
                    // Navigation property found
                    MethodCallExpression navProp = arg as MethodCallExpression;

                    var navigationProperty = navProp.Arguments[0] as MemberExpression;

                    var propertyInfo = (PropertyInfo)navigationProperty.Member;

                    var propertyType = propertyInfo.PropertyType;
                    var propertyName = propertyInfo.Name;

                    var navPropType = propertyType.GetGenericArguments()[0];

                    var generic = typeof(Queryable).GetMethods().Where(m => m.Name == "AsQueryable").ToList()[0];
                    var genericMetgid = generic.MakeGenericMethod(navPropType);

                    ParameterExpression pe1 = Expression.Parameter(navPropType, "pre");

                    Expression left = Expression.PropertyOrField(pe1, "MeetingId"); // TODO, get the reference based on information from  model
                    Expression right = Expression.Constant(id, typeof(int?));
                    Expression e1 = Expression.Equal(left, right);

                    var constructorinfo = typeof(EntityQueryable<>).MakeGenericType(navPropType).GetConstructors()[0];

                    var newstsy = constructorinfo.Invoke(new[] { context.GetService<IEntityQueryProvider>() });
                    Type func11 = typeof(EntityQueryable<>);
                    Type generic23233 = func11.MakeGenericType(navPropType);
                    var nwtwDynamically = Expression.Constant(newstsy, generic23233);

                    Type func1111 = typeof(IQueryable<>);
                    Type generic2323311 = func1111.MakeGenericType(navPropType);

                    Type func11111 = typeof(Func<,>);
                    Type generic232331123 = func11111.MakeGenericType(navPropType, typeof(bool));
                    Type funxs = typeof(Expression<>);
                    Type skljslk = funxs.MakeGenericType(generic232331123);

                    var m1 = GetGenericMethod(typeof(Queryable), "Where", new[] { navPropType }, generic2323311, skljslk);

                    MethodCallExpression whereCallExpression = Expression.Call(
                            m1,
                             nwtwDynamically,//nwtw
                           Expression.Lambda(e1, new ParameterExpression[] { pe1 }));

                    //////////////////////////////////////////////////////////

                    var jan11 = new List<KeyValuePair<string, Type>>();
                    ParameterExpression peeee = Expression.Parameter(navPropType, "p1");
                    List<MemberExpression> exprs1 = new List<MemberExpression>();
                    foreach (var hhhhh in navProp.Arguments.Skip(1))
                    {
                        var navigationPropertyProjection = hhhhh as LambdaExpression;

                        var body1111 = navigationPropertyProjection.Body as MemberExpression;

                        var propertyInfo11111 = (PropertyInfo)body1111.Member;
                        var nameOfTheProperty11 = propertyInfo11111.Name;
                        var typeOfTheProperty11 = propertyInfo11111.PropertyType;

                        var memberAccess1 = Expression.Property(peeee, nameOfTheProperty11);
                        exprs1.Add(memberAccess1);

                        var jan = new KeyValuePair<string, Type>(nameOfTheProperty11, typeOfTheProperty11);
                        jan11.Add(jan);
                    }

                    var annnontype11 = AnonymousTypeUtils.CreateType(jan11);

                    var constructor11 = annnontype11.GetConstructor(jan11.Select(kv => kv.Value).ToArray());
                    typeOfSubProj = constructor11.ReflectedType;
                    var genericSelectMethod = typeof(Queryable).GetMethods().Where(m => m.Name == "Select").ToList()[0];
                    var selectMetgid = genericSelectMethod.MakeGenericMethod(navPropType, typeOfSubProj);

                    var kkkkk1 = Expression.New(constructor11, exprs1);

                    var lambd11a = Expression.Lambda(kkkkk1, peeee);

                    MethodCallExpression selctCallExpression = Expression.Call(
                          selectMetgid,
                          whereCallExpression,
                           lambd11a);

                    var provider = ((IQueryable)context.Set<Post>()).Provider;
                    var theMethods = typeof(IQueryProvider).GetMethods();
                    var createQMethd = theMethods.Where(name => name.Name == "CreateQuery").ToList()[1];
                    var speciifMethod = createQMethd.MakeGenericMethod(constructor11.ReflectedType);

                    Type func = typeof(IEnumerable<>);
                    Type generic2323 = func.MakeGenericType(typeOfSubProj);

                    projs.Add(new KeyValuePair<string, Type>(propertyName, generic2323));

                    navigationProppertyQueryWithProjection1 = speciifMethod.Invoke(provider, new object[] { selctCallExpression });
                }
            }

            var annnontype = AnonymousTypeUtils.CreateType(projs);

            var constructor = annnontype.GetConstructor(projs.Select(kv => kv.Value).ToArray());

            var tt = typeof(Enumerable).GetMethods();
            var m133 = tt.Where(m => m.Name == "ToList").ToList()[0];
            var genericMetgid2222 = m133.MakeGenericMethod(typeOfSubProj);

            MethodCallExpression toListExpression11 = Expression.Call(
                            genericMetgid2222,
                             Expression.Constant(navigationProppertyQueryWithProjection1));

            exprs.Add(toListExpression11);

            var kkkkk = Expression.New(constructor, exprs);

            Expression<Func<T, dynamic>> lambd11a1 = Expression.Lambda<Func<T, dynamic>>(kkkkk, pe);

            var entiry22 = context.Set<T>().AsNoTracking().
                Where(e => e.Id == id).
                Select(lambd11a1).
                Single();

            //var rrr = entiry444.
            //    Select(selectors[0]).
            //    Single();

            //var entiry = context.Set<T>().AsNoTracking().
            //    //Include(selectedProperties).
            //    Where(e => e.Id == id).
            //    Select(selectedProperties).
            //    Single();

            var retEntity = Mapper.DynamicMap<T>(entiry22);
            retEntity.Id = id;
            var alreadytrackedentity = context.ChangeTracker.Entries<T>().Where(e => e.Entity.Id == id).SingleOrDefault();

            if (alreadytrackedentity != null)
            {
                // Remove it from tracking
                alreadytrackedentity.State = EntityState.Detached;
            }
            context.Attach(retEntity);
            return retEntity;
        }

        #endregion Obsolete
    }
}