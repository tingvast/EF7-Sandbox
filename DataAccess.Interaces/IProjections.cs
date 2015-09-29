using System.Collections.Generic;
using System.Linq.Expressions;

namespace DataAccess.Interaces
{
    public interface IProjections
    {
        List<Expression> Projection { get; set; }

        List<INavigationProperty> NavigationPropertiesProjections { get; set; }

        string CacheKey { get; }
    }
}