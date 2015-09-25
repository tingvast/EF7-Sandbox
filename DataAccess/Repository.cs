﻿
using Core;
using EF7;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using DataAccess.Interaces;
using System.Linq.Expressions;
using AutoMapper;
using System.Reflection;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Internal;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using System.Threading;
using System.Reflection.Emit;
using LatticeUtils;
using Microsoft.Data.Entity.Query;

namespace DataAccess
{
    public class Repository : IRepository
    {
        EF7BloggContext context;       

        public Repository(EF7BloggContext context)
        {
            this.context = context;
            
        }

        public T Create<T>(T entity) where T : class, IEntity
        {
            context.Set<T>().Add(entity);

            return entity;
        }

        public T CreateGraph<T>(T entityWithRelations) where T : class, IEntity
        {
            context.ChangeTracker.TrackGraph(entityWithRelations, (e) => e.State = EntityState.Added);

            return entityWithRelations;
        }

        public T Retrieve<T> (int id) where T : class, IEntity
        {
            var retrievedEntity = context.Set<T>().SingleOrDefault(e => e.Id == id);
            return retrievedEntity;
        }

        public static Type CreateAnonymousType<TFieldA, TFieldB>(string fieldNameA, string fieldNameB)
        {
            AssemblyName dynamicAssemblyName = new AssemblyName("TempAssembly");
            AssemblyBuilder dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(dynamicAssemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder dynamicModule = dynamicAssembly.DefineDynamicModule("TempAssembly");

            TypeBuilder dynamicAnonymousType = dynamicModule.DefineType("AnonymousType", TypeAttributes.Public);

            var f1 = dynamicAnonymousType.DefineField(fieldNameA, typeof(TFieldA), FieldAttributes.Public);
            dynamicAnonymousType.DefineField(fieldNameB, typeof(TFieldB), FieldAttributes.Public);

            var constructorBuilder = dynamicAnonymousType.DefineConstructor(MethodAttributes.Public, CallingConventions.Any, new Type[1] { typeof(string) });
            // Generate IL for the method.The constructor stores its argument in the private field.
            ILGenerator myConstructorIL = constructorBuilder.GetILGenerator();
            myConstructorIL.Emit(OpCodes.Ldarg_0);
            myConstructorIL.Emit(OpCodes.Ldarg_1);
            myConstructorIL.Emit(OpCodes.Stfld, f1);
            myConstructorIL.Emit(OpCodes.Ret);

            return dynamicAnonymousType.CreateType();
        }

        //public T Retrieve<T, TResult>(int id, Func<T, TResult> selectedProperties) where T : class, IEntity
        //{            
        //    var entiry = context.Set<T>().AsNoTracking().               
        //        Where(e => e.ID == id).
        //        Select(selectedProperties).
        //        Single();

        //    var retEntity = Mapper.DynamicMap<T>(entiry;)
        //    retEntity.ID = id;
        //    var alreadytrackedentity = context.ChangeTracker.Entries<T>().Where(e => e.Entity.ID == id).SingleOrDefault();

        //    if(alreadytrackedentity != null)
        //    {
        //        // Remove it from tracking
        //        context.ChangeTracker.StateManager.StopTracking(alreadytrackedentity.StateEntry);
        //    }
        //    context.Attach(retEntity);
        //    return retEntity;
        //}

        public T Retrieve<T, TResult>(int id, Expression<Func<T, TResult>> selectedProperties) where T : class, IEntity
        {
            //var arguments = selectedProperties.P.Body.
            var body = (System.Linq.Expressions.NewExpression)selectedProperties.Body;

            List<Expression<Func<T, string>>> selectors = new List<Expression<Func<T, string>>>();

            var entiry444 = context.Set<T>().AsNoTracking().
                 //Include(selectedProperties).
                 Where(e => e.Id == id);

            List<KeyValuePair<string, Type>> projs = new List<KeyValuePair<string, Type>>();
            List<Expression> exprs = new List<Expression>();
            IQueryable<PreRegistration> navigationProppertyQuery = null;
            IQueryable<string> navigationProppertyQueryWithProjection = null;
            object navigationProppertyQueryWithProjection1 = null;
            ParameterExpression pe = Expression.Parameter(typeof(T), "p");
            Type typeOfSubProj = null;
            foreach (var arg in body.Arguments)
            {
                if(arg.NodeType == ExpressionType.MemberAccess)
                {

                    var body1 = arg as MemberExpression;                    


                    var propertyInfo = (PropertyInfo)body1.Member;
                    var nameOfTheProperty = propertyInfo.Name;
                    var typeOfTheProperty = propertyInfo.PropertyType;

                    

                    var memberAccess1 = Expression.Property(pe, nameOfTheProperty);
                    exprs.Add(memberAccess1);



                    //Type anonType = CreateAnonymousType<String, String>(nameOfTheProperty, "Dummy");
                    //var constructor = anonType.GetConstructor(new Type[1] { typeof(string) });


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

                    //Type typeParameterType = propertyType.GetType();
                    var navPropType = propertyType.GetGenericArguments()[0];

                    
                    //var propertyInfo1 = (PropertyInfo)navigationPropertyProjection.Member;
                    //var nameOfTheProperty = propertyInfo1.Name;
                    //var typeOfTheProperty = propertyInfo1.PropertyType;



                    //var memberAccess1 = Expression.Property(pe, nameOfTheProperty);



                    //System.Linq.Dynamic.DynamicExpression
                    //System.Linq.Expressions.DynamicExpression.Lambda()
                    //var ddd = (from pp in context.Set<PreRegistration>() where pp.MeetingId == id select new { pp.Text });
                    //var preRegistrations = context.Set<PreRegistration>()
                    //    .Where(pp => pp.MeetingId == id)
                    //    .Select(pp =>  new { pp.Text });
                    //IQueryable<PreRegistration> jj = context.Preregistrations;


                    var generic = typeof(Queryable).GetMethods().Where(m => m.Name == "AsQueryable").ToList()[0];
                    var genericMetgid = generic.MakeGenericMethod(navPropType);                    

                    ParameterExpression pe1 = Expression.Parameter(navPropType, "pre");

                    Expression left = Expression.PropertyOrField(pe1, "MeetingId");
                    Expression right = Expression.Constant(id, typeof(int?));
                    Expression e1 = Expression.Equal(left, right);

                    //var set = TypeExtensions.GetGenericMethod(typeof(DbContext), "Set");
                    //var hhh = set.MakeGenericMethod(navPropType);
                    //set = typeof(DbContext).GetMethod("Set").MakeGenericMethod(navPropType);                   

                    //var xRef = Expression.Constant(context);
                    //var callRef = Expression.Call(xRef, hhh);

                    //var lambda = Expression.Lambda(callRef);
                    //var uyuy = Expression.Convert(lambda, typeof(IQueryable<PreRegistration>));

                    //var gg = Expression.Call(genericMetgid, uyuy);

                    var constructorinfo = typeof(EntityQueryable<>).MakeGenericType(navPropType).GetConstructors()[0];

                    var newstsy = constructorinfo.Invoke(new [] { context.GetService<IEntityQueryProvider>()});                    
                    var nwtwDynamically = Expression.Constant(newstsy, typeof(Microsoft.Data.Entity.Query.EntityQueryable<Core.PreRegistration>));

                    var m1 = GetGenericMethod(typeof(Queryable), "Where", new[] { navPropType }, typeof(IQueryable<PreRegistration>), typeof(Expression<Func<PreRegistration, bool>>));
                    
                    MethodCallExpression whereCallExpression = Expression.Call(
                            m1,
                             nwtwDynamically,//nwtw
                           Expression.Lambda<Func<PreRegistration, bool>>(e1, new ParameterExpression[] { pe1 }));

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

                        //Type anonType = CreateAnonymousType<String, String>(nameOfTheProperty, "Dummy");
                        //var constructor = anonType.GetConstructor(new Type[1] { typeof(string) });


                        

                        var annnontype11 = AnonymousTypeUtils.CreateType(jan11);

                        
                    //ParameterExpression pe111 = Expression.Parameter(navPropType, "p1");

                    var constructor11 = annnontype11.GetConstructor(jan11.Select(kv => kv.Value).ToArray());
                    typeOfSubProj = constructor11.ReflectedType;
                    var genericSelectMethod = typeof(Queryable).GetMethods().Where(m => m.Name == "Select").ToList()[0];
                    var selectMetgid = genericSelectMethod.MakeGenericMethod(navPropType, typeOfSubProj);


                    var kkkkk1 = Expression.New(constructor11, exprs1);

                    //var kajaia = AnonymousTypeCast(annnontype11, Type.GetType("System.RuntimeType"));
                    //var tt22 = Type.GetType("System.RuntimeType");

                    


                    //ar converted = Expression.Convert(kkkkk1, Type.GetType("System.RuntimeType"));

                    var lambd11a = Expression.Lambda(kkkkk1, peeee);

                    


                    //var expressionlamba111 = typeof(Expression).GetMethods().Where(m => m.Name == "Lambda");
                    //var expressionlamba = expressionlamba111.ToList()[0];
                    //Type func = typeof(Func<,>);
                    //Type generic2323 = func.MakeGenericType(navPropType, annnontype11.GetType());
                    ////var result = Delegate.CreateDelegate(generic2323, get)
                    //var hjaahg = expressionlamba.MakeGenericMethod(generic2323);

                    //var ajah = hjaahg.Invoke(null, new object[] { kkkkk1, new ParameterExpression[] { pe111 } });

                    //var selctCallExpression = Expression.Call(
                    //    typeof(Queryable),
                    //    "Select",
                    //    new Type[] { typeof(List<PreRegistration>), lambd11a.Body.Type },
                    //    whereCallExpression,
                    //    lambd11a);

                    MethodCallExpression selctCallExpression = Expression.Call(     
                                     
                          selectMetgid,
                          whereCallExpression,
                           lambd11a);


                    //MethodCallExpression complete = Expression.Call(

                    ////////////////////////////////////////////////////////////////////////////////////////
                    //projs.Add(new KeyValuePair<string, Type>(propertyName, typeof(IList<PreRegistration>)));
                    

                        navigationProppertyQuery = ((IQueryable)context.Set<PreRegistration>()).Provider.CreateQuery<PreRegistration>(whereCallExpression);

                    var provider = ((IQueryable)context.Set<PreRegistration>()).Provider;
                    var theMethods = typeof(IQueryProvider).GetMethods();
                    var createQMethd = theMethods.Where(name => name.Name == "CreateQuery").ToList()[1];
                    var speciifMethod = createQMethd.MakeGenericMethod(constructor11.ReflectedType);


                    Type func = typeof(IEnumerable<>);
                    Type generic2323 = func.MakeGenericType(typeOfSubProj);
                    //var result = Delegate.CreateDelegate(generic2323, get)
                    //var hjaahg = expressionlamba.MakeGenericMethod(generic2323);


                    projs.Add(new KeyValuePair<string, Type>(propertyName, generic2323));

                    navigationProppertyQueryWithProjection1 = speciifMethod.Invoke(provider, new object[] { selctCallExpression });
                    //navigationProppertyQueryWithProjection = ((IQueryable)context.Set<PreRegistration>()).Provider.CreateQuery<string>(selctCallExpression);


                    //var tt = typeof(Enumerable).GetMethods();
                    //var m133 = tt.Where(m => m.Name == "ToList").ToList()[0];
                    //var genericMetgid222 = m133.MakeGenericMethod(typeof(PreRegistration));
                    //MethodCallExpression toListExpression = Expression.Call(
                    //                genericMetgid222,
                    //                 Expression.Constant(kk));

                    //exprs.Add(toListExpression);

                    //navigationPropertyProjection
                    var preRegistrations = context.Set<PreRegistration>()
                        .Where(pp => pp.MeetingId == id)
                        .Select(pp => new { pp.Text });


                    // This is how projections on includes has to be performed.
                    var entiry44 = context.Set<Meeting>()
                    //.Include(m => m.PreRegistrations)
                    .Where(m => m.Id == id)
                    .Select(m => new
                    {
                        PreRegistrations = preRegistrations.ToList()// kk.ToList()
                    });
                    //.Single();


                }
            }
            
            //ParameterExpression pe = Expression.Parameter(typeof(T), "p");
            var annnontype = AnonymousTypeUtils.CreateType(projs);

            var constructor = annnontype.GetConstructor(projs.Select(kv => kv.Value).ToArray());

            var tt = typeof(Enumerable).GetMethods();
            var m133 = tt.Where(m => m.Name == "ToList").ToList()[0];
            //var genericMetgid222 = m133.MakeGenericMethod(typeof(PreRegistration));
            var genericMetgid2222 = m133.MakeGenericMethod(typeOfSubProj);
            //MethodCallExpression toListExpression = Expression.Call(
            //                genericMetgid222,
            //                 Expression.Constant(navigationProppertyQuery));

            MethodCallExpression toListExpression11 = Expression.Call(
                            genericMetgid2222,
                             Expression.Constant(navigationProppertyQueryWithProjection1));

            ////exprs.Add(toListExpression);
            exprs.Add(toListExpression11);

            var kkkkk = Expression.New(constructor, exprs);



            Expression < Func<T, dynamic>> lambd11a1 = Expression.Lambda<Func<T, dynamic>>(kkkkk, pe);

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

        public T AnonymousTypeCast<T>(object anonymous, T typeExpression)
        {
            return (T)anonymous;
        }

        public static MethodInfo GetGenericMethod(Type declaringType, string methodName, Type[] typeArgs, params Type[] argTypes)
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

        public static Expression<Func<T, TReturn>> GetSelector<T, TReturn>(string fieldName)
              where T : class
              where TReturn : class
        {
            var t = typeof(TReturn);
            ParameterExpression p = Expression.Parameter(typeof(T), "t");
            var body = Expression.Property(p, fieldName);
            return Expression.Lambda<Func<T, TReturn>>(body, new ParameterExpression[] { p });
        }


        

        //static Expression<Func<T, bool>> LabmdaExpression<T>(string propertyName)//, string value1, string property2, int value2)
        //{

        //    ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "o");
        //    MemberExpression memberExpression1 = Expression.PropertyOrField(parameterExpression, propertyName);
        //    //MemberExpression memberExpression2 = Expression.PropertyOrField(parameterExpression, property2);

        //    //ConstantExpression valueExpression1 = Expression.Constant(value1, typeof(string));
        //    //ConstantExpression valueExpression2 = Expression.Constant(value2, typeof(int));

        //    //BinaryExpression binaryExpression1 = Expression.Equal(memberExpression1, valueExpression1);
        //    //BinaryExpression binaryExpression2 = Expression.Equal(memberExpression2, valueExpression2);

        //    //var ret1 = Expression.Lambda<Func<T, bool>>(binaryExpression1, parameterExpression);
        //    //var ret2 = Expression.Lambda<Func<T, bool>>(binaryExpression2, parameterExpression);

        //    //Expression andExpression = Expression.AndAlso(binaryExpression1, binaryExpression2);

        //    //return Expression.Lambda<Func<T, bool>>(andExpression, parameterExpression);

        //    //return Expression.Lambda<Func<T, bool>>()
        //}
        public Meeting Retrieve(int j, int id)
        {

            //var m11 = (from e in context.Set<Meeting>()
            //    join p in context.Set<PreRegistration>() on e.Id equals p.MeetingId
            //    where e.Id == id
            //    select new {e.Location, p.Text});



            var entiry = context.Set<Meeting>().
                Include(m => m.PreRegistrations).
                Where(m => m.Id == id).
                Select(m => new { l = m.Location, p = m.PreRegistrations.Count}).                
                Single();

            var ddd = (from pp in context.Set<PreRegistration>() where pp.MeetingId == id select new { pp.Text });
            var entiry44 = context.Set<Meeting>().
                //Include(m => m.PreRegistrations).
                Where(m => m.Id == id).
                Select(m => new { m.Location, yy = ddd.ToList()}).
                Single();


            var entiry11 = context.Set<Meeting>().
                Include(m => m.PreRegistrations).
                Where(m => m.Id == id).
                //Select(m => new { m.Location, yy = m.PreRegistrations.Select(pre => new { pre.Text }) }).
                Single();


            //var entiry12 = context.Set<Meeting>().
            //    Include(m => m.PreRegistrations.Select(p => new { p.Text })).
            //    Where(m => m.Id == id).
            //    //Select(m => new { m.Location, yy = m.PreRegistrations.Select(pre => new { pre.Text }) }).
            //    Single();

            var retEntity = Mapper.DynamicMap<Meeting>(entiry);
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

        public T RetrieveReadonly<T, TResult>(int id, Func<T, TResult> selectedProperties) where T : class, IEntity
        {
            var entiry = context.Set<T>().AsNoTracking().Where(e => e.Id == id).Select(selectedProperties).Single();
            var retEntity = Mapper.DynamicMap<T>(entiry);
            return retEntity;
        }

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
                if (e.State == EntityState.Detached)
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

        public void Delete<T>(T entity) where T : class, IEntity
        {
            var keys = context.Entry<T>(entity).Metadata.GetForeignKeys();

            context.Remove(entity);
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
    }
}
