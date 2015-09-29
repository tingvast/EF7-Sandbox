using System.Linq.Expressions;

namespace DataAccess
{
    internal static class ParameterHelper
    {
        internal static MemberExpression GetMemberExpression(LambdaExpression projectionLambda)
        {
            MemberExpression member;
            switch (projectionLambda.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    var ue = projectionLambda.Body as UnaryExpression;
                    member = ((ue != null) ? ue.Operand : null) as MemberExpression;
                    break;

                default:
                    member = projectionLambda.Body as MemberExpression;
                    break;
            }

            return member;
        }
    }
}