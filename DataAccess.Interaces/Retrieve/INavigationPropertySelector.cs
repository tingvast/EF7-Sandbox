using Core;
using System;
using System.Linq.Expressions;

namespace DataAccess.Interaces
{
    public interface INavigationPropertySelector<T> where T : class, IEntity
    {
        Expression<Func<T, object>> Navs { get; }
    }
}