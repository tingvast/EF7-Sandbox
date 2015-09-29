using DataAccess.Interaces;
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

        public NavigationProperty(
            string referingPropertName,
            string name,
            Type type)
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
        public int Id { get; set; }
    }
}