using FluentAssertions;
using NUnit.Framework;

namespace Webinex.Asky.Tests.Queries;

public class Query_AndSafeTests
{
    [Test]
    public void WhenQueryAndFilterNull_ShouldReturnNull()
    {
        Query.AndSafe(null, null).Should().BeNull();
    }

    [Test]
    public void WhenQueryNullAndFilterProvided_ShouldReturnQueryWithFilter()
    {
        var filterRule = FilterRule.Eq("name", "John");

        var query = Query.AndSafe(null, filterRule);

        query.Should().NotBeNull();
        query!.FilterRule.Should().Be(filterRule);
        query.SortRule.Should().BeNull();
        query.PagingRule.Should().BeNull();
    }

    [Test]
    public void WhenQueryHasFilter_ShouldCombineFiltersAndKeepOtherRules()
    {
        var initialFilter = FilterRule.Eq("name", "John");
        var extraFilter = FilterRule.NotEq("name", "Jane");
        var sortRule = new[] { SortRule.Asc("name") };
        var pagingRule = new PagingRule(0, 10);
        var query = new Query(initialFilter, sortRule, pagingRule);

        var result = Query.AndSafe(query, extraFilter);

        result.Should().NotBeNull();
        result!.FilterRule.Should().BeEquivalentTo(FilterRule.And(initialFilter, extraFilter));
        result.SortRule.Should().BeSameAs(sortRule);
        result.PagingRule.Should().BeSameAs(pagingRule);
    }
}
