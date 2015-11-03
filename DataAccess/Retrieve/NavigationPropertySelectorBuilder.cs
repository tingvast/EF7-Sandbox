using System;
using Core;
using DataAccess.Interaces;

namespace DataAccess
{
    internal class NavigationPropertySelectorBuilder<T> : INavigationPropertySelectorBuilder<T> where T : class, IEntity
    {
        public INavigationPropertySelector<T> Build()
        {
            throw new NotImplementedException();
        }

        public INavigationPropertySelectorBuilder<T> Include(Func<T, object> navigationProperty)
        {
            throw new NotImplementedException();
        }
    }
}