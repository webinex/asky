using System.Text.Json;
using System.Text.Json.Nodes;

namespace Webinex.Asky;

public class Query
{
    public FilterRule? FilterRule { get; }
    public IReadOnlyCollection<SortRule>? SortRule { get; }
    public PagingRule? PagingRule { get; }

    public Query(FilterRule? filterRule, IReadOnlyCollection<SortRule>? sortRule, PagingRule? pagingRule)
    {
        FilterRule = filterRule;
        SortRule = sortRule;
        PagingRule = pagingRule;
    }

    public bool IsEmpty() => FilterRule == null && (SortRule == null || !SortRule.Any()) && PagingRule == null;

    public static Query? FromJson<T>(JsonElement? jElement, IAskyFieldMap<T> fieldMap,
        JsonSerializerOptions? options = null)
    {
        if (jElement == null)
            return null;

        var jNode = JsonNode.Parse(jElement.Value.GetRawText());
        if (jNode == null)
            return null;
        
        var jObject = jNode.AsObject();

        var filterRule = ParseFilterRule(jObject, fieldMap);
        var sortRule = ParseSortRule(jObject, options);
        var pagingRule = ParsePagingRule(jObject, options);
        
        return new Query(filterRule, sortRule?.ToArray(), pagingRule);
    }

    private static FilterRule? ParseFilterRule<T>(JsonObject jObject, IAskyFieldMap<T> fieldMap)
    {
        var jFilterRule = jObject["filterRule"] ?? jObject["FilterRule"] ?? jObject["filter"] ?? jObject["Filter"];
        return FilterRule.FromJson(jFilterRule, fieldMap);
    }

    private static IEnumerable<SortRule>? ParseSortRule(JsonObject jObject, JsonSerializerOptions? options)
    {
        var jSortRule = jObject["sortRule"] ?? jObject["sortRules"] ??
            jObject["SortRule"] ?? jObject["SortRules"] ?? jObject["sort"] ?? jObject["Sort"];
        return Webinex.Asky.SortRule.FromJson(jSortRule, options);
    }
    
    private static PagingRule? ParsePagingRule(JsonObject jObject, JsonSerializerOptions? options)
    {
        var jPagingRule = jObject["pagingRule"] ?? jObject["PagingRule"] ?? jObject["paging"] ?? jObject["Paging"];
        return PagingRule.FromJson(jPagingRule, options);
    }
}