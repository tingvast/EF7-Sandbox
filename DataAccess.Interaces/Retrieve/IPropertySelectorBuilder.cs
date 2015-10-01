using Core;
using System;
using System.Linq.Expressions;

namespace DataAccess.Interaces
{
    public interface IPropertyProjectorBuilder<TEntity> where TEntity : class, IEntity
    {
        IProjections AllProjections { get; }

        IPropertyProjectorBuilder<TEntity> Select(params Expression<Func<TEntity, dynamic>>[] p1);

        IPropertyProjectorBuilder<TEntity> Include<TProperty>(
            Expression<Func<TEntity, dynamic>> navigationPropery,
            params Expression<Func<TProperty, dynamic>>[] selectedProperties) where TProperty : class, IEntity;

        IPropertySeletor<TEntity> Build();

        #region Obsolete

        IPropertyProjectorBuilder<TEntity> IncludeOld<TProperty>(
         Expression<Func<TEntity, dynamic>> navigationPropery,
         params Expression<Func<TProperty, dynamic>>[] selectedProperties) where TProperty : class, IEntity;

        #endregion Obsolete
    }
}