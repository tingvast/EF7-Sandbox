using Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DataAccess.Interaces
{
    public interface IRepository
    {
        #region Properties

        ISelectPropertyBuilder<T> PropertySelectBuilder<T>(T blog) where T : class, IEntity;

        IUpdatePropertyBuilder<T> PropertyUpdateBuilder<T>(T blog) where T : class, IEntity;

        INavigationPropertySelectorBuilder<T> NavigationPropertySelectorBuilder<T>() where T : class, IEntity;

        #endregion Properties

        #region Add

        T Add<T>(T entity) where T : class, IEntity;

        List<T> Add<T>(params T[] entities) where T : class, IEntity;

        T AddWithRelations<T>(T entityWithRelations) where T : class, IEntity;

        #endregion Add

        #region Retrieve

        
        T RetrieveById<T>(int id, IPropertySecletor<T> projection) where T : class, IEntity;
        T RetrieveById<T>(int id, INavigationPropertySelector<T> projection) where T : class, IEntity;

        T RetrieveById<T>(int id, params Expression<Func<T, dynamic>>[] selectedNavigationProperties) where T : class, IEntity;

        IEnumerable<T> Retrieve<T>(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy = null) where T : class, IEntity;

        T RetrieveReadonly<T, TResult>(int id, Func<T, TResult> selectedProperties) where T : class, IEntity;

        #endregion Retrieve

        #region Update

        T Update<T>(T entity) where T : class, IEntity;

        T Update<T, TResult>(T entity, params Expression<Func<T, TResult>>[] selectedProperties) where T : class, IEntity;

        T UpdateGraph<T>(T entityWithRelations) where T : class, IEntity;

        T UpdateGraph<T>(T entity, IPropertyUpdater<T> projection) where T : class, IEntity;

        #endregion Update

        #region Delete

        void Delete<T>(T entity) where T : class, IEntity;

        #endregion Delete

        #region Obsolete

        T RetrieveObsolete<T, TResult>(int id, Expression<Func<T, TResult>> selectedProperties) where T : class, IEntity;

        Blog RetrieveBlogNonGeneric(int id);

        Blog UpdateNonGeneric(Blog entity, Expression<Func<Blog, dynamic>> selectedProperties);

        T RetrieveByIdOld<T>(int id, ISelectPropertyBuilder<T> selectedSelectProperties) where T : class, IEntity;
        

        #endregion Obsolete
    }
}