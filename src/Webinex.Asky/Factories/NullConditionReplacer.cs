using System.Linq.Expressions;

namespace Webinex.Asky;

internal static class NullConditionReplacer
{
    public static Expression<Func<T, object>> Remove<T>(Expression<Func<T, object>> expression)
    {
        var body = expression.Body;
        body = new Visitor().Visit(body)!;

        return Expression.Lambda<Func<T, object>>(body, expression.Parameters);
    }

    private class Visitor : ExpressionVisitor
    {
        protected override Expression VisitConditional(ConditionalExpression node)
        {
            return TryVisit(node, out var result) ? result! : base.VisitConditional(node);
        }

        private bool TryVisit(ConditionalExpression conditionalExpression, out Expression? expression)
        {
            expression = null;

            if (conditionalExpression.Test is not BinaryExpression binaryExpression)
                return false;

            if (!IsNullConstant(binaryExpression.Left) && !IsNullConstant(binaryExpression.Right))
                return false;

            switch (binaryExpression.NodeType)
            {
                case ExpressionType.Equal:
                    expression = new Visitor().Visit(conditionalExpression.IfFalse);
                    return true;
                case ExpressionType.NotEqual:
                    expression = new Visitor().Visit(conditionalExpression.IfTrue);
                    return true;
                default:
                    return false;
            }
        }

        private bool IsNullConstant(Expression expression)
        {
            return expression is ConstantExpression { Value: null };
        }
    }
}