using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DataAccess.Interaces
{
    public interface INavigationProperty
    {
        string ReferingPropertyName { get; set; }
        string Name { get; set; }
        Type Type { get; set; }
        List<Expression> Projections { get; set; }
        int Id { get; set; }
    }
}