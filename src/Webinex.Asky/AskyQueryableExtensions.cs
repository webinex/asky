using System.Linq.Expressions;
using System.Reflection;

namespace Webinex.Asky;

public static class AskyQueryableExtensions
{
    public static IQueryable<TEntity> Where<TEntity>(
        this IQueryable<TEntity> queryable,
        IAskyFieldMap<TEntity> fieldMap,
        FilterRule filter)
    {
        queryable = queryable ?? throw new ArgumentNullException(nameof(queryable));
        fieldMap = fieldMap ?? throw new ArgumentNullException(nameof(fieldMap));
        filter = filter ?? throw new ArgumentNullException(nameof(filter));

        var expression = AskyExpressionFactory.Create(fieldMap, filter);
        return queryable.Where(expression);
    }

    public static IQueryable<TEntity> PageBy<TEntity>(
        this IQueryable<TEntity> queryable,
        PagingRule rule)
    {
        queryable = queryable ?? throw new ArgumentNullException(nameof(queryable));
        rule = rule ?? throw new ArgumentNullException(nameof(rule));

        return queryable.Skip(rule.Skip).Take(rule.Take);
    }

    public static IOrderedQueryable<TEntity> SortBy<TEntity>(
        this IQueryable<TEntity> queryable,
        IAskyFieldMap<TEntity> fieldMap,
        IEnumerable<SortRule> rules)
    {
        queryable = queryable ?? throw new ArgumentNullException(nameof(queryable));
        rules = rules?.ToArray() ?? throw new ArgumentNullException(nameof(rules));

        if (!rules.Any())
            throw new ArgumentException("Might contain at least one item", nameof(rules));

        var rootSort = queryable.SortBy(fieldMap.Required(rules.ElementAt(0).FieldId), rules.ElementAt(0).Dir);
        return rules.Aggregate(rootSort, (q, arg) => q.ThenSortBy(fieldMap.Required(arg.FieldId), arg.Dir));
    }

    public static IOrderedQueryable<TEntity> SortBy<TEntity>(
        this IQueryable<TEntity> queryable,
        IAskyFieldMap<TEntity> fieldMap,
        SortRule rule)
    {
        queryable = queryable ?? throw new ArgumentNullException(nameof(queryable));
        rule = rule ?? throw new ArgumentNullException(nameof(rule));

        return SortBy(queryable, fieldMap, new[] { rule });
    }

    public static IOrderedQueryable<TEntity> SortBy<TEntity>(
        this IQueryable<TEntity> queryable,
        Expression<Func<TEntity, object>> selector,
        SortDir sortDir)
    {
        queryable = queryable ?? throw new ArgumentNullException(nameof(queryable));
        selector = selector ?? throw new ArgumentNullException(nameof(selector));

        if (!Enum.IsDefined(typeof(SortDir), sortDir))
        {
            throw new ArgumentException($"Unknown sort dir {sortDir}", nameof(sortDir));
        }

        return SortByInternal(queryable, selector, sortDir);
    }

    private static IOrderedQueryable<TEntity> SortByInternal<TEntity>(
        IQueryable<TEntity> queryable,
        Expression<Func<TEntity, object>> selector,
        SortDir sortDir)
    {
        return (IOrderedQueryable<TEntity>)typeof(AskyQueryableExtensions).GetMethod(nameof(___SortBy),
                BindingFlags.Static | BindingFlags.NonPublic)!
            .MakeGenericMethod(typeof(TEntity), LambdaExpressions.ReturnType(selector)).Invoke(null,
                new[] { queryable, LambdaExpressions.ReplaceReturnTypeToTyped(selector), sortDir })!;
    }

    private static IOrderedQueryable<TEntity> ThenSortBy<TEntity>(
        this IOrderedQueryable<TEntity> queryable,
        Expression<Func<TEntity, object>> selector,
        SortDir sortDir)
    {
        queryable = queryable ?? throw new ArgumentNullException(nameof(queryable));
        selector = selector ?? throw new ArgumentNullException(nameof(selector));

        if (!Enum.IsDefined(typeof(SortDir), sortDir))
        {
            throw new ArgumentException($"Unknown sort dir {sortDir}", nameof(sortDir));
        }

        return ThenSortByInternal(queryable, selector, sortDir);
    }

    private static IOrderedQueryable<TEntity> ThenSortByInternal<TEntity>(
        IOrderedQueryable<TEntity> queryable,
        Expression<Func<TEntity, object>> selector,
        SortDir sortDir)
    {
        return (IOrderedQueryable<TEntity>)typeof(AskyQueryableExtensions).GetMethod(nameof(___ThenSortBy),
                BindingFlags.Static | BindingFlags.NonPublic)!
            .MakeGenericMethod(typeof(TEntity), LambdaExpressions.ReturnType(selector)).Invoke(null,
                new[] { queryable, LambdaExpressions.ReplaceReturnTypeToTyped(selector), sortDir })!;
    }

    private static IOrderedQueryable<TEntity> ___SortBy<TEntity, TKey>(
        IQueryable<TEntity> queryable,
        Expression<Func<TEntity, TKey>> selector,
        SortDir sortDir)
    {
        return sortDir == SortDir.Asc
            ? queryable.OrderBy(selector)
            : queryable.OrderByDescending(selector);
    }

    private static IOrderedQueryable<TEntity> ___ThenSortBy<TEntity, TKey>(
        IOrderedQueryable<TEntity> queryable,
        Expression<Func<TEntity, TKey>> selector,
        SortDir sortDir)
    {
        return sortDir == SortDir.Asc
            ? queryable.ThenBy(selector)
            : queryable.ThenByDescending(selector);
    }
}