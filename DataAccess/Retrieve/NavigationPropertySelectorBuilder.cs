using System;
using Core;
using DataAccess.Interaces;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace DataAccess
{
    internal class NavigationPropertySelectorBuilder<T> : INavigationPropertySelectorBuilder<T> where T : class, IEntity
    {
        internal Expression<Func<T, object>> _navigationProperties;

        public NavigationPropertySelectorBuilder()
        {
            //_navigationProperties = new List<Expression<Func<T, object>>>();
        }

        public Expression<Func<T, object>> Navs
        {
            get
            {
                return _navigationProperties;
            }
        }

        public INavigationPropertySelector<T> Build()
        {
            var selector = new NavigationPropertySelector<T>();
            selector.MyProperty = _navigationProperties;

            return selector;
        }

        public INavigationPropertySelectorBuilder<T> Include(Expression<Func<T, object>> navigationProperty)
        {
            LabelExpression l = navigationProperty as LabelExpression;
            var propertyInfo = (PropertyInfo)ParameterHelper.GetMemberExpression(navigationProperty as LambdaExpression).Member;

            var propertyNameOfReferer = propertyInfo.Name;

            _navigationProperties = navigationProperty;

            //var navigationProperty = new NavigationProperty(
            //       propertyNameOfReferer,
            //       typeof(TProperty).Name,
            //       typeof(TProperty));

            //navigationProperty.Projections.AddRange(selectedProperties);

            return this;
        }
    }
}