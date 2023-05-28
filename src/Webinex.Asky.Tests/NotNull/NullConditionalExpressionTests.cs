using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using NUnit.Framework;

namespace Webinex.Asky.Tests.NotNull;

public class NullConditionalExpressionTests
{
    private Entity1[] _data = null!;
    
    [Test]
    public void WhenEqAndOptionsRemoveNotNullChecksAnd1LevelDepth_ShouldThrow()
    {
        var filter = FilterRule.Eq("entity2.value", 1);
        var predicate = Asky.Predicate(new Entity1AskyFieldMap(), filter, FilterOptions.RemoveNotNullChecks);

        Assert.Throws<NullReferenceException>(() => _data.AsQueryable().Where(predicate).ToArray());
    }

    [Test]
    public void WhenEqAndDefaultOptionsAnd1LevelDepth_ShouldThrow()
    {
        var filter = FilterRule.Eq("entity2.value", 1);
        var predicate = Asky.Predicate(new Entity1AskyFieldMap(), filter);

        var result = _data.AsQueryable().Where(predicate).ToArray();
        result.Length.Should().Be(1);

        result.First().E2!.Value.Should().Be(1);
    }

    [Test]
    public void WhenEqAndOptionsRemoveNotNullChecksAnd2LevelDepth_ShouldThrow()
    {
        var filter = FilterRule.Eq("entity1.entity2.value", 1);
        var predicate = Asky.Predicate(new Entity1AskyFieldMap(), filter, FilterOptions.RemoveNotNullChecks);

        Assert.Throws<NullReferenceException>(() => _data.AsQueryable().Where(predicate).ToArray());
    }

    [SetUp]
    public void SetUp()
    {
        _data = new[]
        {
            new Entity1
            {
                E1 = new Entity1
                {
                    E2 = new Entity2
                    {
                        Value = 2,
                    },
                },
                E2 = new Entity2
                {
                    Value = 1,
                },
            },
            new Entity1(),
        };
    }

    public class Entity1AskyFieldMap : IAskyFieldMap<Entity1>
    {
        public Expression<Func<Entity1, object?>>? this[string fieldId] => fieldId switch
        {
            "entity1.entity2.value" => x => x.E1 != null ? x.E1.E2 != null ? x.E1.E2.Value : default(int?) : default,
            "entity2.value" => x => x.E2 != null ? x.E2.Value : default(int?),
            _ => null,
        };
    }

    public class Entity1
    {
        public Entity1? E1 { get; init; }
        public Entity2? E2 { get; init; }
    }
    
    public class Entity2
    {
        public int Value { get; init; }
    }
}