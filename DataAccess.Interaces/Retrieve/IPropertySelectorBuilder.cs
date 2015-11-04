using Core;
using System;
using System.Linq.Expressions;

namespace DataAccess.Interaces
{
    public interface ISelectPropertyBuilder<TEntity> where TEntity : class, IEntity
    {
        IProjections AllProjections { get; }

        ISelectPropertyBuilder<TEntity> Select(params Expression<Func<TEntity, dynamic>>[] p1);

        ISelectPropertyBuilder<TEntity> Include<TProperty>(
            Expression<Func<TEntity, dynamic>> navigationPropery,
            params Expression<Func<TProperty, dynamic>>[] selectedProperties) where TProperty : class, IEntity;

        IPropertySecletor<TEntity> Build();

        #region Obsolete

        ISelectPropertyBuilder<TEntity> IncludeOld<TProperty>(
         Expression<Func<TEntity, dynamic>> navigationPropery,
         params Expression<Func<TProperty, dynamic>>[] selectedProperties) where TProperty : class, IEntity;

        #endregion Obsolete
    }
}