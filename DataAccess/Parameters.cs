using DataAccess.Interaces;
using EF7;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace DataAccess
{
    public class NavigationProperty : INavigationProperty
    {
        public NavigationProperty()
        {
            Projections = new List<Expression>();
        }

        public NavigationProperty(string referingPropertName, string name, Type type)
            : this()
        {
            ReferingPropertyName = referingPropertName;
            Name = name;
            Type = type;
        }

        public string ReferingPropertyName { get; set; }

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
                    "Dummy",
                    typeof(TProperty).Name,
                    typeof(TProperty));

            navigationProperty.Projections.AddRange(p);

            AllProjections.NavigationPropertiesProjections.Add(navigationProperty);

            return this;
        }

        IIncludePropertySelector<T> IPropertyProjector<T>.Include<TProperty>(Expression<Func<T, dynamic>> navigationPropery, params Expression<Func<TProperty, dynamic>>[] properties)
        {
            var navigationPropertyProjection = navigationPropery as LambdaExpression;

            var member = navigationPropertyProjection.Body as MemberExpression;

            var propertyInfo = (PropertyInfo)member.Member;

            var propertyName = propertyInfo.Name;

            var navigationProperty = new NavigationProperty(
                   propertyName,
                    typeof(TProperty).Name,
                    typeof(TProperty));

            navigationProperty.Projections.AddRange(properties);

            AllProjections.NavigationPropertiesProjections.Add(navigationProperty);

            return this;
        }

        public IPropertyProjector<T> Select(params Expression<Func<T, dynamic>>[] p1)
        {
            AllProjections.Projection.AddRange(p1);

            return this;
        }
    }
}