using Core;
using DataAccess.Interaces;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace DataAccess
{
    public class UpdatePropertyBuilder<T> : IUpdatePropertyBuilder<T> where T : class, IEntity
    {
        public IProjections AllProjections
        {
            get;
            private set;
        }

        public UpdatePropertyBuilder()
        {
            AllProjections = new Projections();
        }

        public IPropertyUpdater<T> Build()
        {
            return new PropertyUpdater<T>()
            {
                AllProjections = AllProjections
            };
        }

        public IUpdatePropertyBuilder<T> PropertiesToUpdate(Expression<Func<T, dynamic>>[] p1)
        {
            AllProjections.Projection.AddRange(p1);

            return this;
        }

        IUpdatePropertyBuilder<T> IUpdatePropertyBuilder<T>.IncludeNavigationProperyUpdate<TProperty>(
            int id,
            Expression<Func<T, dynamic>> navigationPropery,
            params Expression<Func<TProperty, dynamic>>[] selectedProperties)
        {
            var propertyInfo = (PropertyInfo)ParameterHelper.GetMemberExpression(navigationPropery as LambdaExpression).Member;

            var propertyNameOfReferer = propertyInfo.Name;

            var navigationProperty = new NavigationProperty(
                   propertyNameOfReferer,
                   typeof(TProperty).Name,
                   typeof(TProperty))
            {
                Id = id
            };

            navigationProperty.Projections.AddRange(selectedProperties);

            AllProjections.NavigationPropertiesProjections.Add(navigationProperty);
            return this;
        }
    }
}