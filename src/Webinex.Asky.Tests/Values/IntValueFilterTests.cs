using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Webinex.Asky.Tests.Values;

internal class IntValueFilterTests : ValueFilterTestsBase<int>
{
    [Test]
    public void WhenEq()
    {
        WithValues(1, 2, 3);
        
        Run(FilterRule.Eq(FieldId, 2));

        Result.First().Should().Be(2);
    }

    [Test]
    public void WhenNotEq()
    {
        WithValues(1, 2, 3);
        
        Run(FilterRule.NotEq(FieldId, 2));

        Result.Should().BeEquivalentTo(new[] { 1, 3 });
    }

    [Test]
    public void WhenGt()
    {
        WithValues(2, 3, 1, 0);
        
        Run(FilterRule.Gt(FieldId, 2));

        Result.Should().BeEquivalentTo(new[] { 3 });
    }

    [Test]
    public void WhenGte()
    {
        WithValues(2, 1, 3, 0);
        
        Run(FilterRule.Gte(FieldId, 2));

        Result.Should().BeEquivalentTo(new[] { 2, 3 });
    }

    [Test]
    public void WhenLt()
    {
        WithValues(2, -1, 3, 0);
        
        Run(FilterRule.Lt(FieldId, 1));

        Result.Should().BeEquivalentTo(new[] { -1, 0 });
    }

    [Test]
    public void WhenLte()
    {
        WithValues(2, -1, 3, 0);
        
        Run(FilterRule.Lte(FieldId, 2));

        Result.Should().BeEquivalentTo(new[] { -1, 0, 2});
    }

    [Test]
    public void WhenIn()
    {
        WithValues(1, -2, 3, 4);
        
        Run(FilterRule.In(FieldId, new[] { Values[1], 6, Values[3] }));

        Result.Should().BeEquivalentTo(new[] { Values[1], Values[3] });
    }

    [Test]
    public void WhenNotIn()
    {
        WithValues(1, 0, -8, 4);
        
        Run(FilterRule.NotIn(FieldId, new[] { Values[1], 22, Values[3] }));

        Result.Should().BeEquivalentTo(new[] { Values[0], Values[2] });
    }
}