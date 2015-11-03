using DataAccess.Interaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Core;

namespace DataAccess
{
    public class NavigationPropertySelector<T> : INavigationPropertySelector<T>  where T : class, IEntity
    {
        public Expression<Func<T, object>> Navs
        {
            get
            {
                return MyProperty;
            }
        }

        internal Expression<Func<T, object>> MyProperty { get; set; }     


    }
}
