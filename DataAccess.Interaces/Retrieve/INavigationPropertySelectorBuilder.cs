using Core;
using System;

namespace DataAccess.Interaces
{
    public interface INavigationPropertySelectorBuilder<T> where T : class, IEntity
    {
        INavigationPropertySelectorBuilder<T> Include(Func<T, object> navigationProperty);
        INavigationPropertySelector<T> Build();
    }
}