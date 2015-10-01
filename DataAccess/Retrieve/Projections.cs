using DataAccess.Interaces;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DataAccess
{
    public class Projections : IProjections
    {
        public Projections()
        {
            Projection = new List<Expression>();
            NavigationPropertiesProjections = new List<INavigationProperty>();
        }

        public List<INavigationProperty> NavigationPropertiesProjections { get; set; }

        public string CacheKey
        {
            get
            {
                return CreateCacheKey();
            }
        }

        public List<Expression> Projection { get; set; }

        #region Private

        private string CreateCacheKey()
        {
            StringBuilder sb = new StringBuilder();
            foreach (LambdaExpression le in Projection)
            {
                MemberExpression member = ParameterHelper.GetMemberExpression(le);

                var propertyInfo = (PropertyInfo)member.Member;
                var declaringType = propertyInfo.DeclaringType;
                var propertyName = propertyInfo.Name;
                var propertyType = propertyInfo.PropertyType;

                sb.Append(":" + declaringType + ":" + propertyName + ":" + propertyType + ":");
            }

            sb.Append(":");
            foreach (INavigationProperty np in NavigationPropertiesProjections)
            {
                sb.Append(np.Name + ":" + np.Type);

                foreach (LambdaExpression le in np.Projections)
                {
                    MemberExpression member = ParameterHelper.GetMemberExpression(le);

                    var propertyInfo = (PropertyInfo)member.Member;
                    var declaringType = propertyInfo.DeclaringType;
                    var propertyName = propertyInfo.Name;
                    var propertyType = propertyInfo.PropertyType;

                    sb.Append(":" + declaringType + ":" + propertyName + ":" + propertyType + ":");
                }
            }

            return sb.ToString();
        }

        #endregion Private
    }
}