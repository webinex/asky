using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Webinex.Asky.Tests.Bool;

internal class StartsWithBoolFilterTests
{
    private const string ITEM1 = nameof(Tuple<string, int>.Item1);
    private const string ITEM2 = nameof(Tuple<string, int>.Item2);

    private Tuple<string, int>[] _values;
    private Tuple<string, int>[] _result;

    [Test]
    public void WhenStartsWith_AndEq()
    {
        WithValues(("abc", 1), ("abd", 2), ("bcd", 1));

        var filter = FilterRule.And(
            FilterRule.StartsWith(ITEM1, "ab"),
            FilterRule.Eq(ITEM2, 1));

        Run(filter);

        _result.Should().BeEquivalentTo(new[] { Tuple.Create("abc", 1) });
    }

    [Test]
    public void WhenStartsWith_OrEq()
    {
        WithValues(("abc", 1), ("abd", 2), ("bcd", 3));

        var filter = FilterRule.Or(
            FilterRule.StartsWith(ITEM1, "ab"),
            FilterRule.Eq(ITEM2, 3));

        Run(filter);

        _result.Should().BeEquivalentTo(new[]
            { Tuple.Create("abc", 1), Tuple.Create("abd", 2), Tuple.Create("bcd", 3) });
    }

    [Test]
    public void WhenNotStartsWith_AndEq()
    {
        WithValues(("abc", 1), ("abd", 2), ("bcd", 1));

        var filter = FilterRule.And(
            FilterRule.NotStartsWith(ITEM1, "ab"),
            FilterRule.Eq(ITEM2, 1));

        Run(filter);

        _result.Should().BeEquivalentTo(new[] { Tuple.Create("bcd", 1) });
    }

    [Test]
    public void WhenStartsWith_AndContains()
    {
        WithValues(("abcde", 1), ("abxyz", 2), ("xyzab", 3));

        var filter = FilterRule.And(
            FilterRule.StartsWith(ITEM1, "ab"),
            FilterRule.Contains(ITEM1, "cd"));

        Run(filter);

        _result.Should().BeEquivalentTo(new[] { Tuple.Create("abcde", 1) });
    }

    [Test]
    public void WhenStartsWith_OrNotStartsWith()
    {
        WithValues(("abc", 1), ("def", 2), ("abd", 3), ("ghi", 4));

        var filter = FilterRule.Or(
            FilterRule.StartsWith(ITEM1, "ab"),
            FilterRule.NotStartsWith(ITEM1, "d"));

        Run(filter);

        _result.Should().BeEquivalentTo(new[]
            { Tuple.Create("abc", 1), Tuple.Create("abd", 3), Tuple.Create("ghi", 4) });
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
