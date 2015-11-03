using System;
using System.Linq.Expressions;

namespace DataAccess.Interaces
{
    public interface IPropertySecletor<T>
    {
        Expression<Func<T, dynamic>> Expression { get; set; }
        IProjections AllProjections { get; set; }
        Type ProjectedEntityAnonymousType { get; set; }
    }
}