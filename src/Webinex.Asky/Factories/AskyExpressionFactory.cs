using System.Linq.Expressions;
using static Webinex.Asky.FilterOperator;

namespace Webinex.Asky;

public static class AskyExpressionFactory
{
    public static Expression<Func<T, bool>> Create<T>(
        IAskyFieldMap<T> fieldMap,
        FilterRule filter,
        FilterOptions options = FilterOptions.None)
    {
        fieldMap = fieldMap ?? throw new ArgumentNullException(nameof(fieldMap));
        filter = filter ?? throw new ArgumentNullException(nameof(filter));

        return filter switch
        {
            BoolFilterRule boolFilterRule => Create(fieldMap, boolFilterRule, options),
            ValueFilterRule filterRule => Create(fieldMap, filterRule, options),
            CollectionFilterRule collectionFilterRule => Create(fieldMap, collectionFilterRule, options),
            ChildCollectionFilterRule childCollectionFilterRule => Create(fieldMap, childCollectionFilterRule, options),
            _ => throw new ArgumentOutOfRangeException(nameof(filter)),
        };
    }

    private static Expression<Func<T, bool>> Create<T>(
        IAskyFieldMap<T> fieldMap,
        BoolFilterRule filter,
        FilterOptions options)
    {
        var expressions = filter.Children.Select(x => Create(fieldMap, x, options)).ToArray();

        return filter.Operator switch
        {
            OR => BoolExpressions.Or(expressions),
            AND => BoolExpressions.And(expressions),
            _ => throw new ArgumentOutOfRangeException(nameof(filter)),
        };
    }

    private static Expression<Func<T, bool>> Create<T>(
        IAskyFieldMap<T> fieldMap,
        ValueFilterRule filter,
        FilterOptions options)
    {
        var selector = Selector(fieldMap, filter.FieldId, options);
        var value = filter.Value;

        return filter.Operator switch
        {
            EQ => FilterExpressions.Eq(selector, value),
            NOT_EQ => FilterExpressions.NotEq(selector, value),
            GTE => FilterExpressions.Gte(selector, value),
            GT => FilterExpressions.Gt(selector, value),
            LTE => FilterExpressions.Lte(selector, value),
            LT => FilterExpressions.Lt(selector, value),
            CONTAINS => FilterExpressions.Contains(selector, value),
            NOT_CONTAINS => FilterExpressions.NotContains(selector, value),
            _ => throw new ArgumentOutOfRangeException(nameof(filter.Operator)),
        };
    }

    private static Expression<Func<T, bool>> Create<T>(
        IAskyFieldMap<T> fieldMap,
        CollectionFilterRule filter,
        FilterOptions options)
    {
        var selector = Selector(fieldMap, filter.FieldId, options);
        var values = filter.Values;

        return filter.Operator switch
        {
            IN => FilterExpressions.In(selector, values),
            NOT_IN => FilterExpressions.NotIn(selector, values),
            _ => throw new ArgumentOutOfRangeException(nameof(filter.Operator)),
        };
    }

    private static Expression<Func<T, bool>> Create<T>(
        IAskyFieldMap<T> fieldMap,
        ChildCollectionFilterRule rule,
        FilterOptions options)
    {
        var selector = Selector(fieldMap, rule.FieldId, options);
        var collectionValueType = LambdaExpressions.ReturnCollectionValueType(selector);
        var factoryType =
            typeof(AskyChildCollectionExpressionFactory<,>).MakeGenericType(typeof(T), collectionValueType);
        var factory = (IAskyChildCollectionExpressionFactory<T>)Activator.CreateInstance(factoryType, fieldMap);

        return rule.Operator switch
        {
            ANY => factory.Any(selector, rule.Rule, options),
            ALL => factory.All(selector, rule.Rule, options),
            _ => throw new ArgumentOutOfRangeException(nameof(rule.Operator)),
        };
    }

    private static Expression<Func<T, object>> Selector<T>(
        IAskyFieldMap<T> fieldMap,
        string fieldId,
        FilterOptions options)
    {
        var selector = fieldMap.Resolve(fieldId);
        return options.HasFlag(FilterOptions.RemoveNotNullChecks) ? NullConditionReplacer.Remove(selector) : selector;
    }
}