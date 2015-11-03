using Core;
using System;
using System.Linq.Expressions;

namespace DataAccess.Interaces
{
    public interface INavigationPropertySelectorBuilder<T> where T : class, IEntity
    {
        
        INavigationPropertySelectorBuilder<T> Include(Expression<Func<T, object>> navigationProperty);
        INavigationPropertySelector<T> Build();
    }
}