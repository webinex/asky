using System;
using FluentAssertions;
using NUnit.Framework;

namespace Webinex.Asky.Tests.Values;

internal class DateTimeOffsetValueFilterTests : ValueFilterTestsBase<DateTimeOffset>
{
    private static readonly DateTimeOffset NOW = DateTimeOffset.UtcNow;
    
    [Test]
    public void WhenEq()
    {
        WithValues(NOW.AddSeconds(1), NOW, NOW.AddSeconds(-1), NOW);
        
        Run(FilterRule.Eq(FieldId, NOW));

        Result.Should().BeEquivalentTo(new[] { NOW, NOW });
    }
    
    [Test]
    public void WhenEq_NoMatch()
    {
        WithValues(NOW.AddSeconds(1), NOW, NOW.AddSeconds(-1), NOW);
        
        Run(FilterRule.Eq(FieldId, NOW.AddMilliseconds(1)));

        Result.Should().BeEmpty();
    }

    [Test]
    public void WhenNotEq()
    {
        WithValues(NOW.AddSeconds(1), NOW, NOW.AddSeconds(-1), NOW);
        
        Run(FilterRule.NotEq(FieldId, NOW));

        Result.Should().BeEquivalentTo(new[] { NOW.AddSeconds(1), NOW.AddSeconds(-1) });
    }

    [Test]
    public void WhenGt()
    {
        WithValues(NOW.AddSeconds(1), NOW, NOW.AddSeconds(-1), NOW.AddMilliseconds(1));
        
        Run(FilterRule.Gt(FieldId, NOW));

        Result.Should().BeEquivalentTo(new[] { NOW.AddSeconds(1), NOW.AddMilliseconds(1) });
    }

    [Test]
    public void WhenGte()
    {
        WithValues(NOW.AddSeconds(1), NOW, NOW.AddSeconds(-1), NOW.AddMilliseconds(1));
        
        Run(FilterRule.Gte(FieldId, NOW));

        Result.Should().BeEquivalentTo(new[] { NOW.AddSeconds(1), NOW, NOW.AddMilliseconds(1) });
    }

    [Test]
    public void WhenLt()
    {
        WithValues(NOW.AddSeconds(1), NOW, NOW.AddSeconds(-1), NOW.AddMilliseconds(-1));
        
        Run(FilterRule.Lt(FieldId, NOW));

        Result.Should().BeEquivalentTo(new[] { NOW.AddSeconds(-1), NOW.AddMilliseconds(-1) });
    }

    [Test]
    public void WhenLte()
    {
        WithValues(NOW.AddSeconds(1), NOW, NOW.AddSeconds(-1), NOW.AddMilliseconds(-1));
        
        Run(FilterRule.Lte(FieldId, NOW));

        Result.Should().BeEquivalentTo(new[] { NOW.AddSeconds(-1), NOW, NOW.AddMilliseconds(-1) });
    }

    [Test]
    public void WhenIn()
    {
        WithValues(NOW.AddSeconds(1), NOW, NOW.AddSeconds(-1), NOW.AddMilliseconds(-1));
        
        Run(FilterRule.In(FieldId, new[] { NOW.AddMilliseconds(1), NOW, NOW.AddSeconds(-1) }));

        Result.Should().BeEquivalentTo(new[] { NOW, NOW.AddSeconds(-1) });
    }

    [Test]
    public void WhenNotIn()
    {
        WithValues(NOW.AddSeconds(1), NOW, NOW.AddSeconds(-1), NOW.AddMilliseconds(-1));
        
        Run(FilterRule.NotIn(FieldId, new[] { NOW.AddMilliseconds(1), NOW, NOW.AddSeconds(-1) }));

        Result.Should().BeEquivalentTo(new[] { NOW.AddSeconds(1), NOW.AddMilliseconds(-1) });
    }
}