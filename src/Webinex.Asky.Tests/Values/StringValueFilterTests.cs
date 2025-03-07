using FluentAssertions;
using NUnit.Framework;

namespace Webinex.Asky.Tests.Values;

internal class StringValueFilterTests : ValueFilterTestsBase<string>
{
    [Test]
    public void WhenEq()
    {
        WithValues("1", "2", "3");
        
        Run(FilterRule.Eq(FieldId, "2"));

        Result.Should().BeEquivalentTo("2");
    }

    [Test]
    public void WhenNotEq()
    {
        WithValues("1", "2", "3");
        
        Run(FilterRule.NotEq(FieldId, "2"));

        Result.Should().BeEquivalentTo("1", "3");
    }

    [Test]
    public void WhenContains()
    {
        WithValues("abc", "bcd", "cde");
        
        Run(FilterRule.Contains(FieldId, "bc"));

        Result.Should().BeEquivalentTo("abc", "bcd");
    }

    [Test]
    public void WhenNotContains()
    {
        WithValues("abc", "bcd", "cde");
        
        Run(FilterRule.NotContains(FieldId, "bc"));

        Result.Should().BeEquivalentTo("cde");
    }

    [Test]
    public void WhenIn()
    {
        WithValues("1", "-2", "3", "4");
        
        Run(FilterRule.In(FieldId, new[] { Values[1], "6", Values[3] }));

        Result.Should().BeEquivalentTo(Values[1], Values[3]);
    }

    [Test]
    public void WhenNotIn()
    {
        WithValues("1", "0", "-8", "4");
        
        Run(FilterRule.NotIn(FieldId, new[] { Values[1], "22", Values[3] }));

        Result.Should().BeEquivalentTo(Values[0], Values[2]);
    }

    [Test]
    public void WhenStartsWith()
    {
        WithValues("a", "b", "aa", "bb");

        Run(FilterRule.StartsWith(FieldId, "a"));

        Result.Should().BeEquivalentTo(Values[0], Values[2]);
    }

    [Test]
    public void WhenNotStartsWith()
    {
        WithValues("a", "b", "aa", "bb");

        Run(FilterRule.NotStartsWith(FieldId, "a"));

        Result.Should().BeEquivalentTo(Values[1], Values[3]);
    }
}