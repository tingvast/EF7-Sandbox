using Core;
using EF7;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DataAccess.Interaces
{
    public interface INavigationProperty
    {
        string ReferingPropertyName { get; set; }
        string Name { get; set; }
        Type Type { get; set; }

        List<Expression> Projections { get; set; }
    }

    public interface IProjections
    {
        List<Expression> Projection { get; set; }

        List<INavigationProperty> NavigationPropertiesProjections { get; set; }

        string CreateCacheKey { get; }
    }

    public interface IPropertyProjectorBuilder<TEntity> where TEntity : class, IEntity
    {
        IProjections AllProjections { get; }

        IPropertyProjectorBuilder<TEntity> Select(params Expression<Func<TEntity, dynamic>>[] p1);

        IPropertyProjectorBuilder<TEntity> Include<TProperty>(
            Expression<Func<TEntity, dynamic>> navigationPropery,
            params Expression<Func<TProperty, dynamic>>[] selectedProperties) where TProperty : class, IEntity;


        IPropertyProjectorBuilder<TEntity> IncludeNew<TProperty>(
            Expression<Func<TEntity, dynamic>> navigationPropery,
            params Expression<Func<TProperty, dynamic>>[] selectedProperties) where TProperty : class, IEntity;

        IPropertyProjector<TEntity> Build();
    }

    public interface IPropertyProjector<T>
    {
        Expression<Func<T, dynamic>> Expression { get; set; }
        IProjections AllProjections { get; set; }
        Type ProjectedEntityAnonymousType { get; set; }
    }

    public interface IIncludePropertySelector<TEntity> : IPropertyProjectorBuilder<TEntity> where TEntity : class, IEntity
    {
        //IIncludePropertySelector<TEntity> ThenInclude<TProperty>(params Expression<Func<TProperty, dynamic>>[] p);
    }

    public interface IRepository
    {
        IPropertyProjectorBuilder<T> CreatePropertyProjectorBuilder<T>(T blog) where T : class, IEntity;

        #region Create

        T Create<T>(T entity) where T : class, IEntity;

        List<T> CreateMany<T>(params T[] entities) where T : class, IEntity;

        T CreateGraph<T>(T entityWithRelations) where T : class, IEntity;

        #endregion Create

        #region Retrieve

        T RetrieveById<T>(int id, IPropertyProjectorBuilder<T> selectedProperties) where T : class, IEntity;

        T RetrieveByIdNew<T>(int id, IPropertyProjector<T> projection) where T : class, IEntity;

        IEnumerable<T> Retrieve<T>(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy = null) where T : class, IEntity;

        T RetrieveReadonly<T, TResult>(int id, Func<T, TResult> selectedProperties) where T : class, IEntity;

        #endregion Retrieve

        #region Update

        T Update<T>(T entity) where T : class, IEntity;

        T Update<T, TResult>(T entity, Expression<Func<T, TResult>> selectedProperties) where T : class, IEntity;

        T UpdateGraph<T>(T entityWithRelations) where T : class, IEntity;

        #endregion Update

        #region Delete

        void Delete<T>(T entity) where T : class, IEntity;

        #endregion Delete

        #region Obsolete

        T RetrieveObsolete<T, TResult>(int id, Expression<Func<T, TResult>> selectedProperties) where T : class, IEntity;

        Blog RetrieveBlogNonGeneric(int id);

        #endregion Obsolete
    }
}