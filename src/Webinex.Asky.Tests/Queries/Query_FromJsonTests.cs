using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using NUnit.Framework;

namespace Webinex.Asky.Tests.Queries;

public class Query_FromJsonTests
{
    [Test]
    public void WhenRulesNull_ShouldBeOk()
    {
        var jElement = CreateJsonElement("""
                                         {
                                            "sortRule": null,
                                            "pagingRule": null,
                                            "filterRule": null
                                         }
                                         """);
        
        var query = Query.FromJson(jElement, TestValueAskyFieldMap.Instance);
        query.Should().NotBeNull();

        query!.FilterRule.Should().BeNull();
        query.SortRule.Should().BeNull();
        query.PagingRule.Should().BeNull();
    }
    
    [Test]
    public void WhenSortRuleCollection_ShouldBeOk()
    {
        var jElement = CreateJsonElement("""
                                         {
                                            "sortRule": [
                                                { "fieldId": "name", "dir": "asc" },
                                                { "fieldId": "id", "dir": "desc" }
                                            ]
                                         }
                                         """);
        
        var query = Query.FromJson(jElement, TestValueAskyFieldMap.Instance);
        query.Should().NotBeNull();

        query!.SortRule.Should().NotBeNull();
        query.SortRule!.Count.Should().Be(2);

        query.SortRule.ElementAt(0).FieldId.Should().Be("name");
        query.SortRule.ElementAt(0).Dir.Should().Be(SortDir.Asc);
        
        query.SortRule.ElementAt(1).FieldId.Should().Be("id");
        query.SortRule.ElementAt(1).Dir.Should().Be(SortDir.Desc);
    }
    
    [Test]
    public void WhenNullJson_ShouldBeOk()
    {
        Query.FromJson(CreateJsonElement("null"), TestValueAskyFieldMap.Instance).Should().BeNull();
    }

    [Test]
    public void WhenNull_ShouldBeOk()
    {
        Query.FromJson(null, TestValueAskyFieldMap.Instance).Should().BeNull();
    }
    
    [Test]
    public void WhenWellFormedObject_ShouldBeOk()
    {
        var queryString = """
                    {
                       "filterRule": {
                           "fieldId": "name",
                           "operator": "=",
                           "value": "John"
                       },
                       "sortRule": {
                            "fieldId": "name",
                            "dir": "asc"
                       },
                       "pagingRule": {
                            "skip": 0,
                            "take": 10
                       }
                    }
                    """;

        var query = Query.FromJson(CreateJsonElement(queryString), TestValueAskyFieldMap.Instance);
        query.Should().NotBeNull();
        
        query!.FilterRule.Should().NotBeNull();
        var valueFilterRule = query.FilterRule as ValueFilterRule;

        valueFilterRule.Should().NotBeNull();
        valueFilterRule!.Operator.Should().Be(FilterOperator.EQ);
        valueFilterRule.FieldId.Should().Be("name");
        valueFilterRule.Value.Should().Be("John");
        
        query.SortRule.Should().NotBeNull();
        query.SortRule.Should().HaveCount(1);
        var sortRule = query.SortRule!.First();
        sortRule.FieldId.Should().Be("name");
        sortRule.Dir.Should().Be(SortDir.Asc);
        
        query.PagingRule.Should().NotBeNull();
        query.PagingRule!.Skip.Should().Be(0);
        query.PagingRule!.Take.Should().Be(10);
    }

    public record TestValue(string Name);
    
    private JsonElement CreateJsonElement(string jsonString)
    {
        var utfReader = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonString));
        return JsonElement.ParseValue(ref utfReader);
    }
    
    public class TestValueAskyFieldMap : IAskyFieldMap<TestValue>
    {
        public static TestValueAskyFieldMap Instance { get; } = new();
        
        public Expression<Func<TestValue, object>>? this[string fieldId] => fieldId switch
        {
            "name" => x => x.Name,
            _ => null,
        };
    }
}