using DataAccess.Interaces;
using EF7;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DataAccess
{
    public class NavigationProperty : INavigationProperty
    {
        public NavigationProperty()
        {
            Projections = new List<Expression>();
        }

        public NavigationProperty(string name, Type type)
            : this()
        {
            Name = name;
            Type = type;
        }

        public string Name { get; set; }
        public Type Type { get; set; }

        public List<Expression> Projections { get; set; }
    }

    public class TheDate : IProjections
    {
        public TheDate()
        {
            Projection = new List<Expression>();
            NavigationPropertiesProjections = new List<INavigationProperty>();
        }

        public List<INavigationProperty> NavigationPropertiesProjections { get; set; }

        public List<Expression> Projection { get; set; }
    }

    public static class PropertyProjectorFactory<T> where T : class, IEntity
    {
        public static IPropertyProjector<T> Create()
        {
            return new PropertyProjector<T>();
        }
    }

    public class PropertyProjector<T> : IPropertyProjector<T>, IIncludePropertySelector<T> where T : class, IEntity
    {
        public PropertyProjector()
        {
            AllProjections = new TheDate();
        }

        public IProjections AllProjections
        {
            get; private set;
        }

        public IIncludePropertySelector<T> Include<TProperty>(params Expression<Func<TProperty, dynamic>>[] p) where TProperty : class, IEntity
        {
            var navigationProperty = new NavigationProperty(
                    typeof(TProperty).Name,
                    typeof(TProperty));

            navigationProperty.Projections.AddRange(p);

            AllProjections.NavigationPropertiesProjections.Add(navigationProperty);

            return this;
        }

        //public IPropertyProjector<T> Include<TProperty>(IPropertyProjector<TProperty> propertySelector)
        //{
        //    throw new NotImplementedException();
        //}

        public IPropertyProjector<T> Select(params Expression<Func<T, dynamic>>[] p1)
        {
            AllProjections.Projection.AddRange(p1);

            return this;
        }

        //public IPropertyProjector<T> SelectSimple(Expression<Func<T, dynamic>> f)
        //{
        //    throw new NotImplementedException();
        //}

        //public IPropertyProjector<T> Select<TProperty>(IPropertyProjector<TProperty> propertySelector, params Expression<Func<T, dynamic>>[] p1)
        //{
        //    throw new NotImplementedException();
        //}

        //public IPropertyProjector<TProperty> SelectNavigation<TProperty>(Expression<Func<T, IEnumerable<TProperty>>> p)
        //{
        //    throw new NotImplementedException();
        //}

        //public IPropertyProjector<T> SelectNavigation<TProperty>(Expression<Func<T, IEnumerable<TProperty>>> p, Expression<Func<TProperty, dynamic>> q)
        //{
        //    throw new NotImplementedException();
        //}

        //public IPropertyProjector<T> SelectSimple(Expression<Func<T, dynamic>> f)
        //{
        //    throw new NotImplementedException();
        //}

        //public IIncludePropertySelector<T> ThenInclude<TProperty>(params Expression<Func<TProperty, dynamic>>[] p)
        //{
        //    throw new NotImplementedException();
        //}

        //public IPropertyProjector<T> Where(params Expression<Func<T, dynamic>>[] p1)
        //{
        //    throw new NotImplementedException();
        //}
    }
}