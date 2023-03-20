using System.Linq.Expressions;
using System.Reflection;

namespace Webinex.Asky;

internal interface IAskyChildCollectionExpressionFactory<TEntity>
{
    Expression<Func<TEntity, bool>> Any(Expression<Func<TEntity, object>> selector, FilterRule rule);
    Expression<Func<TEntity, bool>> All(Expression<Func<TEntity, object>> selector, FilterRule rule);
}

internal abstract class AskyChildCollectionExpressionFactory
{
    protected static readonly MethodInfo SELECT_METHOD_INFO;

    static AskyChildCollectionExpressionFactory()
    {
        Expression<Func<IEnumerable<object>, object>> expr = x => x.Select(o => o);
        var methodCallExpression = (MethodCallExpression)expr.Body;
        SELECT_METHOD_INFO = methodCallExpression.Method.GetGenericMethodDefinition();
    }
}

internal class AskyChildCollectionExpressionFactory<TEntity, TCollectionValue> : AskyChildCollectionExpressionFactory,
    IAskyChildCollectionExpressionFactory<TEntity>,
    IAskyFieldMap<TCollectionValue>
{
    private readonly IAskyFieldMap<TEntity> _entityFieldMap;

    public AskyChildCollectionExpressionFactory(IAskyFieldMap<TEntity> entityFieldMap)
    {
        _entityFieldMap = entityFieldMap;
    }

    public Expression<Func<TEntity, bool>> Any(Expression<Func<TEntity, object>> selector, FilterRule rule)
    {
        var predicate = AskyExpressionFactory.Create(this, rule);
        return FilterExpressions.Any(selector, predicate);
    }

    public Expression<Func<TEntity, bool>> All(Expression<Func<TEntity, object>> selector, FilterRule rule)
    {
        var predicate = AskyExpressionFactory.Create(this, rule);
        return FilterExpressions.All(selector, predicate);
    }

    public Expression<Func<TCollectionValue, object>>? this[string fieldId]
    {
        get
        {
            var field = _entityFieldMap[fieldId];
            return field != null ? Extract(fieldId, field) : null;
        }
    }

    private Expression<Func<TCollectionValue, object>> Extract(
        string fieldId,
        Expression<Func<TEntity, object>> expression)
    {
        if (expression.Body is not MethodCallExpression methodCallExpression)
            throw new InvalidOperationException($"{fieldId} might Enumerable.Select method call expression");

        if (methodCallExpression.Method.GetGenericMethodDefinition() != SELECT_METHOD_INFO)
            throw new InvalidOperationException(
                $"{fieldId} might Enumerable.Select method call expression. For example, x => x.Values.Select(v => v.Name)");

        var result = (LambdaExpression)methodCallExpression.Arguments[1];
        return Expression.Lambda<Func<TCollectionValue, object>>(result.Body, result.Parameters);
    }
}