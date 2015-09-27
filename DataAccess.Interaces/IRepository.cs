using Core;
using EF7;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

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

        IPropertyProjector<TEntity> Select(params Expression<Func<TEntity, dynamic>>[] p1);

        //IPropertyProjector<TEntity> Where(params Expression<Func<TEntity, dynamic>>[] p1);

        IIncludePropertySelector<TEntity> Include<TProperty>(params Expression<Func<TProperty, dynamic>>[] p) where TProperty : class, IEntity;
    }

    public interface IIncludePropertySelector<TEntity> : IPropertyProjector<TEntity> where TEntity : class, IEntity
    {
        //IIncludePropertySelector<TEntity> ThenInclude<TProperty>(params Expression<Func<TProperty, dynamic>>[] p);
    }

    public interface IRepository
    {
        #region Create
        T Create<T>(T entity) where T : class, IEntity;

        List<T> CreateMany<T>(params T[] entities) where T : class, IEntity;

        T CreateGraph<T>(T entityWithRelations) where T : class, IEntity;

        #endregion

        #region Retrieve
        T RetrieveById<T>(int id, IPropertyProjector<T> selector) where T : class, IEntity;

        T Retrieve<T>(int id) where T : class, IEntity;
        
        T RetrieveReadonly<T, TResult>(int id, Func<T, TResult> selectedProperties) where T : class, IEntity;

        #endregion

        #region Update
        T Update<T>(T entity) where T : class, IEntity;

        T Update<T, TResult>(T entity, Expression<Func<T, TResult>> selectedProperties) where T : class, IEntity;

        T UpdateGraph<T>(T entityWithRelations) where T : class, IEntity;

        #endregion

        #region Delete
        void Delete<T>(T entity) where T : class, IEntity;

        #endregion

        #region Obsolete

        T RetrieveObsolete<T, TResult>(int id, Expression<Func<T, TResult>> selectedProperties) where T : class, IEntity;
        Blog RetrieveBlogNonGeneric(int id);

        #endregion
    }
}