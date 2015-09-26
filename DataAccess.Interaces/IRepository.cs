using Core;
using EF7;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interaces
{
    public interface INavigationProperty
    {
        string Name { get; set; }
        Type Type { get; set; }

        List<Expression> Projections { get; set; }
    }



    public interface IProjections
    {
        List<Expression> Projection { get; set; }

        List<INavigationProperty> NavigationPropertiesProjections { get; set; }
    }

    public interface IPropertyProjector<TEntity> where TEntity : class, IEntity
    {
        
        IProjections AllProjections { get; }

        //IPropertyProjector<TEntity> SelectSimple(Expression<Func<TEntity, dynamic>> f);

        //IPropertyProjector<TProperty> SelectNavigation<TProperty>(Expression<Func<TEntity, IEnumerable<TProperty>>> p);

        //IPropertyProjector SelectNavigation<TProperty>(Expression<Func<TEntity, IEnumerable<TProperty>>> p, Expression<Func<TProperty, dynamic>> q);

        IPropertyProjector<TEntity> Select(params Expression<Func<TEntity, dynamic>>[] p1);

        //IPropertyProjector<TEntity> Where(params Expression<Func<TEntity, dynamic>>[] p1);

        //IPropertyProjector<TEntity> Select<TProperty>(IPropertyProjector<TProperty> propertySelector, params Expression<Func<TEntity, dynamic>>[] p1);

        // IPropertyProjector<TEntity> Include<TProperty>(IPropertyProjector<TProperty> propertySelector);

        IIncludePropertySelector<TEntity> Include<TProperty>(params Expression<Func<TProperty, dynamic>>[] p) where TProperty : class, IEntity;
    }




    public interface IIncludePropertySelector<TEntity> : IPropertyProjector<TEntity> where TEntity : class, IEntity
    {
        //IIncludePropertySelector<TEntity> ThenInclude<TProperty>(params Expression<Func<TProperty, dynamic>>[] p);

    }

    public interface IRepository
    {

        T Create<T>(T entity) where T : class, IEntity;

        T CreateGraph<T>(T entityWithRelations) where T : class, IEntity;

        T Retrieve<T>(int id) where T : class, IEntity;

        T Retrieve<T, TResult>(int id, Expression<Func<T, TResult>> selectedProperties) where T : class, IEntity;

        Meeting Retrieve(int j, int id);

        T RetrieveReadonly<T, TResult>(int id, Func<T, TResult> selectedProperties) where T : class, IEntity;

        T Update<T>(T entity) where T : class, IEntity;

        T Update<T, TResult>(T entity, Expression<Func<T, TResult>> selectedProperties) where T : class, IEntity;

        T UpdateGraph<T>(T entityWithRelations) where T : class, IEntity;

        void Delete<T>(T entity) where T : class, IEntity;

        T RetrieveById<T>(int id, IPropertyProjector<T> selector) where T : class, IEntity;

    }
}
