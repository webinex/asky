using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using NUnit.Framework;

namespace Webinex.Asky.Tests.JsonConverters;

public class AskyFilterJsonConverterCollectionTests
{
    private Entity[] _entities = null!;

    [Test]
    public void WhenFilterByCollection_ShouldProperlyResolveType()
    {
        var filterRuleJson = @"
{ ""fieldId"": ""item.name"", ""operator"": ""contains"", ""value"": ""item-1-1"" }
";
        var filterRule = FilterRule.FromJson(filterRuleJson, new EntityFieldMap());

        filterRule.Should().NotBeNull();
        filterRule.Should().BeEquivalentTo(FilterRule.Contains("item.name", "item-1-1"));
    }

    private class EntityFieldMap : IAskyFieldMap<Entity>
    {
        public Expression<Func<Entity, object>> this[string fieldId] => fieldId switch
        {
            "item.name" => x => x.Items.Select(i => i.Name),
            _ => null,
        };
    }

    [SetUp]
    public void SetUp()
    {
        _entities = new[]
        {
            new Entity
            {
                Items = new[]
                {
                    new Item
                    {
                        Name = "item-1-1",
                    },
                    new Item
                    {
                        Name = "item-1-2",
                    },
                },
            },
            new Entity
            {
                Items = new[]
                {
                    new Item
                    {
                        Name = "item-2-1",
                    },
                    new Item
                    {
                        Name = "item-2-2",
                    },
                },
            },
        };
    }

    public class Entity
    {
        public IEnumerable<Item> Items { get; init; }
    }

    public class Item
    {
        public string Name { get; init; }
    }
}