using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Webinex.Asky.Tests.Bool;

internal class AndFilterTests
{
    private const string ITEM1 = nameof(Tuple<int, int>.Item1);
    private const string ITEM2 = nameof(Tuple<int, int>.Item2);

    private Tuple<string, int>[] _values;
    private Tuple<string, int>[] _result;

    [Test]
    public void When2Match()
    {
        WithValues(("123", 1), ("123", 2), ("123", 1));

        var filter = FilterRule.And(
            FilterRule.Eq(ITEM1, "123"),
            FilterRule.Eq(ITEM2, 1));

        Run(filter);

        _result.Should().BeEquivalentTo(new[] { Tuple.Create("123", 1), Tuple.Create("123", 1) });
    }

    [Test]
    public void When2NoMatch()
    {
        WithValues(("123", 1), ("123", 2), ("123", 1));

        var filter = FilterRule.And(
            FilterRule.Eq(ITEM1, "123"),
            FilterRule.Eq(ITEM2, 3));

        Run(filter);

        _result.Should().BeEmpty();
    }

    [Test]
    public void When3()
    {
        WithValues(("1243", 1), ("133", 1), ("123", 2), ("1223", 1), ("132", 1));

        var filter = FilterRule.And(
            FilterRule.Contains(ITEM1, "12"),
            FilterRule.Contains(ITEM1, "3"),
            FilterRule.Eq(ITEM2, 1));

        Run(filter);

        _result.Should().BeEquivalentTo(new[] { Tuple.Create("1243", 1), Tuple.Create("1223", 1) });
    }

    [Test]
    public void WhenNested()
    {
        WithValues(("1243", 1), ("133", 1), ("123", 2), ("1223", 1), ("132", 1));

        var filter = FilterRule.And(
            FilterRule.And(
                FilterRule.Contains(ITEM1, "12"),
                FilterRule.Contains(ITEM1, "3")),
            FilterRule.Eq(ITEM2, 1));

        Run(filter);

        _result.Should().BeEquivalentTo(new[] { Tuple.Create("1243", 1), Tuple.Create("1223", 1) });
    }

    [Test]
    public void When1_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => FilterRule.And(new[] { FilterRule.Eq(ITEM1, "1243") }));
    }

    private void WithValues(params (string val1, int val2)[] values)
    {
        _values = values.Select(x => Tuple.Create(x.val1, x.val2)).ToArray();
    }

    private void Run(FilterRule filter)
    {
        _result = _values.AsQueryable().Where(new TupleFieldMap<string, int>(), filter).ToArray();
    }

    [SetUp]
    public void SetUp()
    {
        _values = null;
        _result = null;
    }
}