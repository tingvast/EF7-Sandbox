using Core;
using System;
using System.Linq.Expressions;

namespace DataAccess.Interaces
{
    public interface IUpdatePropertyBuilder<T> where T : class, IEntity
    {
        IProjections AllProjections { get; }

        IUpdatePropertyBuilder<T> PropertiesToUpdate(params Expression<Func<T, dynamic>>[] p1);

        IUpdatePropertyBuilder<T> IncludeNavigationProperyUpdate<TProperty>(
            int id,
            Expression<Func<T, dynamic>> navigationPropery,
            params Expression<Func<TProperty, dynamic>>[] selectedProperties) where TProperty : class, IEntity;

        IPropertyUpdater<T> Build();
    }
}