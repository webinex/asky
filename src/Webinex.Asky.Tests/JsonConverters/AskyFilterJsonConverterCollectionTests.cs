using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using NUnit.Framework;

namespace Webinex.Asky.Tests.JsonConverters;

public class AskyFilterJsonConverterCollectionTests
{
    [Test]
    public void WhenFilterByCollection_ShouldProperlyResolveType()
    {
        var filterRuleJson = """
                             {
                                 "fieldId": "array.name",
                                 "operator": "contains",
                                 "value": "item-1-1"
                             }
                             """;
        var filterRule = FilterRule.FromJson(filterRuleJson, new EntityFieldMap());

        filterRule.Should().NotBeNull();
        filterRule.Should().BeEquivalentTo(FilterRule.Contains("array.name", "item-1-1"));
    }

    [Test]
    public void WhenOperatorIn_FilterByCollection_ShouldProperlyResolveType()
    {
        var filterRuleJson = """
                             {
                                 "fieldId": "property.name",
                                 "operator": "in",
                                 "values": ["item-1-1"]
                             }
                             """;
        var filterRule = FilterRule.FromJson(filterRuleJson, new EntityFieldMap());

        filterRule.Should().NotBeNull();
        filterRule.Should().BeEquivalentTo(FilterRule.In("property.name", new[] { "item-1-1" }));
    }

    [Test]
    public void WhenOperatorNotIn_FilterByCollection_ShouldProperlyResolveType()
    {
        var filterRuleJson = """
                             {
                                 "fieldId": "property.name",
                                 "operator": "!in",
                                 "values": ["item-1-1"]
                             }
                             """;
        var filterRule = FilterRule.FromJson(filterRuleJson, new EntityFieldMap());

        filterRule.Should().NotBeNull();
        filterRule.Should().BeEquivalentTo(FilterRule.NotIn("property.name", new[] { "item-1-1" }));
    }

    [TestCase("any", FilterOperator.ANY)]
    [TestCase("all", FilterOperator.ALL)]
    public void WhenChildCollection_ShouldBeOk(string text, string value)
    {
        var json = $$"""
                     {
                         "fieldId": "array",
                         "operator": "{{text}}",
                         "rule": {
                             "operator": "=",
                             "fieldId": "array.name",
                             "value": "1-1"
                         }
                     }
                     """;

        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
        var jElement = JsonElement.ParseValue(ref reader);

        var filterRule = FilterRule.FromJson(jElement, new EntityFieldMap());

        filterRule.Should().NotBeNull();
        filterRule.Should().BeOfType<ChildCollectionFilterRule>();

        var childCollectionFilterRule = (ChildCollectionFilterRule)filterRule;
        childCollectionFilterRule!.FieldId.Should().Be("array");
        childCollectionFilterRule.Operator.Should().Be(value);
        childCollectionFilterRule!.Rule.Should().BeOfType<ValueFilterRule>();

        var valueFilterRule = (ValueFilterRule)childCollectionFilterRule.Rule;
        valueFilterRule!.FieldId.Should().Be("array.name");
        valueFilterRule.Operator.Should().Be(FilterOperator.EQ);
        valueFilterRule.Value.Should().Be("1-1");
    }

    private class EntityFieldMap : IAskyFieldMap<Entity>
    {
        public Expression<Func<Entity, object>> this[string fieldId] => fieldId switch
        {
            "array" => x => x.Items,
            "array.name" => x => x.Items.Select(i => i.Name),
            "property.name" => x => x.Property,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    private record Entity(IEnumerable<Item> Items, string Property);

    private record Item(string Name);
}