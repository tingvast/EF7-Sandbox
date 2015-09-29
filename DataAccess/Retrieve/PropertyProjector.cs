using DataAccess.Interaces;
using System;
using System.Linq.Expressions;

namespace DataAccess
{
    public class PropertyProjector<T> : IPropertyProjector<T>
    {
        public Expression<Func<T, dynamic>> Expression { get; set; }
        public IProjections AllProjections { get; set; }
        public Type ProjectedEntityAnonymousType { get; set; }
    }
}