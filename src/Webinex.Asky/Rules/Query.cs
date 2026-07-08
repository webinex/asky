#if NET8_0
using System.Diagnostics.CodeAnalysis;
#endif
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Webinex.Asky;

public class Query
{
    private static readonly IReadOnlyCollection<string> FILTER_RULE_PROP_NAMES = ["filterRule", "FilterRule", "filter", "Filter"];
    private static readonly IReadOnlyCollection<string> SORT_RULE_PROP_NAMES = ["sortRule", "sortRules", "SortRule", "SortRules", "sort", "Sort"];
    private static readonly IReadOnlyCollection<string> PAGING_RULE_PROP_NAMES = ["pagingRule", "PagingRule", "paging", "Paging"];
    
    public FilterRule? FilterRule { get; }
    public IReadOnlyCollection<SortRule>? SortRule { get; }
    public PagingRule? PagingRule { get; }

    public Query(
        FilterRule? filterRule = null,
        IReadOnlyCollection<SortRule>? sortRule = null,
        PagingRule? pagingRule = null)
    {
        FilterRule = filterRule;
        SortRule = sortRule;
        PagingRule = pagingRule;
    }

    public bool IsEmpty() => FilterRule == null && (SortRule == null || !SortRule.Any()) && PagingRule == null;

    public static Query? FromJson<T>(
        JsonElement? jElement,
        IAskyFieldMap<T> fieldMap,
        JsonSerializerOptions? options = null)
    {
        if (jElement == null || jElement.Value.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
            return null;
        
        if (jElement.Value.ValueKind != JsonValueKind.Object)
            throw new InvalidOperationException($"Expected JSON object for query, but got {jElement.Value.ValueKind}.");

        var filterRule = TryGetJsonElementProp(jElement.Value, FILTER_RULE_PROP_NAMES, out var jFilterRule)
            ? FilterRule.FromJson(jFilterRule, fieldMap)
            : null;
        
        var sortRule = TryGetJsonElementProp(jElement.Value, SORT_RULE_PROP_NAMES, out var jSortRule)
            ? Webinex.Asky.SortRule.FromJson(jSortRule, options)
            : null;
        
        var pagingRule = TryGetJsonElementProp(jElement.Value, PAGING_RULE_PROP_NAMES, out var jPagingRule)
            ? PagingRule.FromJson(jPagingRule, options)
            : null;

        return new Query(filterRule, sortRule?.ToArray(), pagingRule);
    }
    
    
    public static Query? FromJson<T>(
        JsonNode? jNode,
        IAskyFieldMap<T> fieldMap,
        JsonSerializerOptions? options = null)
    {
        if (jNode is null)
            return null;
        
        if (jNode is not JsonObject jObject)
            throw new InvalidOperationException($"Expected {nameof(JsonObject)} but got {jNode.GetType().Name}");
        
        var filterRule = TryGetJsonObjectProp(jObject, FILTER_RULE_PROP_NAMES, out var jFilterRule)
            ? FilterRule.FromJson(jFilterRule, fieldMap)
            : null;
        
        var sortRule = TryGetJsonObjectProp(jObject, SORT_RULE_PROP_NAMES, out var jSortRule)
            ? Webinex.Asky.SortRule.FromJson(jSortRule, options)
            : null;
        
        var pagingRule = TryGetJsonObjectProp(jObject, PAGING_RULE_PROP_NAMES, out var jPagingRule)
            ? PagingRule.FromJson(jPagingRule, options)
            : null;
        
        return new Query(filterRule, sortRule?.ToArray(), pagingRule);
    }

    private static bool TryGetJsonObjectProp(JsonObject jObject, IEnumerable<string> propNames, out JsonNode? result)
    {
        foreach (var propName in propNames)
        {
            if (jObject.TryGetPropertyValue(propName, out var jNode))
            {
                result = jNode;
                return true;
            }
        }

        result = null;
        return false;
    }

    private static bool TryGetJsonElementProp(JsonElement jElement, IEnumerable<string> propNames, out JsonElement? result)
    {
        foreach (var propName in propNames)
        {
            if (jElement.TryGetProperty(propName, out var jProp))
            {
                result = jProp;
                return true;
            }
        }

        result = null;
        return false;
    }

    /// <summary>
    ///     Combines the specified query and filter using logical <c>AND</c>, ignoring <c>null</c> values.
    /// </summary>
    /// <returns>
    ///     A combined <see cref="Query"/> when query or non-null rule is provided;
    ///     otherwise, <c>null</c>.
    /// </returns>
#if NET8_0
    [return:NotNullIfNotNull(nameof(query))]
    [return:NotNullIfNotNull(nameof(filterRule))]
#endif
    public static Query? AndSafe(Query? query, FilterRule? filterRule)
    {
        if (query is null && filterRule is null)
            return null;
        
        filterRule = FilterRule.AndSafe(query?.FilterRule, filterRule);
        return new Query(filterRule, query?.SortRule, query?.PagingRule);
    }
}