using DataAccess.Interaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{


    public static class PropertyProjectorFactory<T>
    {
        public static IPropertyProjector<T> Create()
        {
            return new PropertyProjector<T>();
        }
    }


    public class PropertyProjector<T> : IPropertyProjector<T>, IIncludePropertySelector<T>
    {

        public PropertyProjector()
        {
            Projections = new List<Expression<Func<T, dynamic>>>();
        }

        public List<Expression<Func<T, dynamic>>> Projections
        {
            get;
            private set;            
        }
        
        public IIncludePropertySelector<T> Include<TProperty>(params Expression<Func<TProperty, dynamic>>[] p)
        {
            return this;
        }

        public IPropertyProjector<T> Include<TProperty>(IPropertyProjector<TProperty> propertySelector)
        {
            throw new NotImplementedException();
        }

        public IPropertyProjector<T> Select(params Expression<Func<T, dynamic>>[] p1)
        {
            Projections.AddRange(p1);

            return this;
        }

        public IPropertyProjector<T> Select<TProperty>(IPropertyProjector<TProperty> propertySelector, params Expression<Func<T, dynamic>>[] p1)
        {
            throw new NotImplementedException();
        }

        public IPropertyProjector<TProperty> SelectNavigation<TProperty>(Expression<Func<T, IEnumerable<TProperty>>> p)
        {
            throw new NotImplementedException();
        }

        public IPropertyProjector<T> SelectNavigation<TProperty>(Expression<Func<T, IEnumerable<TProperty>>> p, Expression<Func<TProperty, dynamic>> q)
        {
            throw new NotImplementedException();
        }

        public IPropertyProjector<T> SelectSimple(Expression<Func<T, dynamic>> f)
        {
            throw new NotImplementedException();
        }

        public IIncludePropertySelector<T> ThenInclude<TProperty>(params Expression<Func<TProperty, dynamic>>[] p)
        {
            throw new NotImplementedException();
        }

        public IPropertyProjector<T> Where(params Expression<Func<T, dynamic>>[] p1)
        {
            throw new NotImplementedException();
        }
    }
}
