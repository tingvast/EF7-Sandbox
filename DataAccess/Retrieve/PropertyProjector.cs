using DataAccess.Interaces;
using System;
using System.Linq.Expressions;

namespace DataAccess
{
    public class PropertyProjector<T> : IPropertySecletor<T>
    {
        private PropertyProjector()
        {
        }

        public PropertyProjector(
            Expression<Func<T, dynamic>> e,
            IProjections a,
            Type t)
        {
            Expression = e;
            AllProjections = a;
            ProjectedEntityAnonymousType = t;
        }

        public Expression<Func<T, dynamic>> Expression { get; set; }
        public IProjections AllProjections { get; set; }
        public Type ProjectedEntityAnonymousType { get; set; }
    }
}