﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

    private class EntityFieldMap : IAskyFieldMap<Entity>
    {
        public Expression<Func<Entity, object>> this[string fieldId] => fieldId switch
        {
            "array.name" => x => x.Items.Select(i => i.Name),
            "property.name" => x => x.Property,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    private record Entity(IEnumerable<Item> Items, string Property);
    private record Item(string Name);
}