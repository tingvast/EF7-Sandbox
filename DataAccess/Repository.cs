
using ConsoleApplication3;
using EF7;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using DataAccess.Interaces;
using System.Linq.Expressions;
using AutoMapper;
using System.Reflection;

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
            var entiry = context.Set<T>().AsNoTracking().
                //Include(selectedProperties).
                Where(e => e.Id == id).
                Select(selectedProperties).
                Single();

            var retEntity = Mapper.DynamicMap<T>(entiry);
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

        public Meeting Retrieve(int j, int id)
        {
            var entiry = context.Set<Meeting>().Include(m => m.PreRegistrations).Where(m => m.Id == id).Select(m => m.Location1).Single();
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
            return null; ;


        }

        public T RetrieveReadonly<T, TResult>(int id, Func<T, TResult> selectedProperties) where T : class, IEntity
        {
            var entiry = context.Set<T>().AsNoTracking().Where(e => e.Id == id).Select(selectedProperties).Single();
            var retEntity = Mapper.DynamicMap<T>(entiry);
            return retEntity;
        }

        public T Update<T>(T entity) where T : class, IEntity
        {
            context.Set<T>().Update(entity);

            return entity;
        }

        public T Update<T, TResult>(T entity, Expression<Func<T, TResult>> selectedProperties) where T : class, IEntity
        {

            context.Entry(entity).State = EntityState.Modified;
            string prop = ResolvePropertyName(selectedProperties);

            var properties = context.Entry(entity).Metadata.GetProperties().ToList();
            var toRenove = properties.Find(p => p.Name == prop);
            properties.Remove(toRenove);


            var entity1 = context.Entry(entity);
            properties.ForEach(p =>
            {
                entity1.Property(p.Name).IsModified = false;
                int i = 0;
            });


            context.Entry(entity).Property(prop).IsModified = true;

            

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
