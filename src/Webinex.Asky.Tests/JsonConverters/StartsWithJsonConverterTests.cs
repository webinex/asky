using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentAssertions;
using NUnit.Framework;

namespace Webinex.Asky.Tests.JsonConverters;

public class StartsWithJsonConverterTests
{
    [Test]
    public void WhenStartsWith_ShouldDeserializeCorrectly()
    {
        var json = """
                   {
                       "fieldId": "name",
                       "operator": "startsWith",
                       "value": "abc"
                   }
                   """;

        var filterRule = FilterRule.FromJson(json, new TestFieldMap());

        filterRule.Should().NotBeNull();
        filterRule.Should().BeEquivalentTo(FilterRule.StartsWith("name", "abc"));
    }

    [Test]
    public void WhenNotStartsWith_ShouldDeserializeCorrectly()
    {
        var json = """
                   {
                       "fieldId": "name",
                       "operator": "!startsWith",
                       "value": "abc"
                   }
                   """;

        var filterRule = FilterRule.FromJson(json, new TestFieldMap());

        filterRule.Should().NotBeNull();
        filterRule.Should().BeEquivalentTo(FilterRule.NotStartsWith("name", "abc"));
    }

    [Test]
    public void WhenStartsWith_InAndFilter_ShouldDeserializeCorrectly()
    {
        var json = """
                   {
                       "operator": "and",
                       "children": [
                           {
                               "fieldId": "name",
                               "operator": "startsWith",
                               "value": "abc"
                           },
                           {
                               "fieldId": "name",
                               "operator": "=",
                               "value": "abcdef"
                           }
                       ]
                   }
                   """;

        var filterRule = FilterRule.FromJson(json, new TestFieldMap());

        filterRule.Should().NotBeNull();
        filterRule.Should().BeOfType<BoolFilterRule>();

        var boolRule = (BoolFilterRule)filterRule;
        boolRule.Operator.Should().Be(FilterOperator.AND);
        boolRule.Children.Should().HaveCount(2);
        boolRule.Children[0].Should().BeEquivalentTo(FilterRule.StartsWith("name", "abc"));
        boolRule.Children[1].Should().BeEquivalentTo(FilterRule.Eq("name", "abcdef"));
    }

    [Test]
    public void WhenStartsWith_EmptyValue_ShouldDeserializeCorrectly()
    {
        var json = """
                   {
                       "fieldId": "name",
                       "operator": "startsWith",
                       "value": ""
                   }
                   """;

        var filterRule = FilterRule.FromJson(json, new TestFieldMap());

        filterRule.Should().NotBeNull();
        filterRule.Should().BeEquivalentTo(FilterRule.StartsWith("name", ""));
    }

    private class TestFieldMap : IAskyFieldMap<TestEntity>
    {
        public Expression<Func<TestEntity, object>> this[string fieldId] => fieldId switch
        {
            "name" => x => x.Name,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    private record TestEntity(string Name);
}
