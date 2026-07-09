using FluentAssertions;
using NUnit.Framework;

namespace Webinex.Asky.Tests.Values;

internal class StartsWithEdgeCaseTests : ValueFilterTestsBase<string>
{
    [Test]
    public void WhenStartsWith_EmptyPrefix_ShouldMatchAll()
    {
        WithValues("abc", "def", "ghi");

        Run(FilterRule.StartsWith(FieldId, ""));

        Result.Should().BeEquivalentTo(Values);
    }

    [Test]
    public void WhenNotStartsWith_EmptyPrefix_ShouldMatchNone()
    {
        WithValues("abc", "def", "ghi");

        Run(FilterRule.NotStartsWith(FieldId, ""));

        Result.Should().BeEmpty();
    }

    [Test]
    public void WhenStartsWith_NoMatch()
    {
        WithValues("abc", "def", "ghi");

        Run(FilterRule.StartsWith(FieldId, "z"));

        Result.Should().BeEmpty();
    }

    [Test]
    public void WhenNotStartsWith_NoMatch_ShouldReturnAll()
    {
        WithValues("abc", "def", "ghi");

        Run(FilterRule.NotStartsWith(FieldId, "z"));

        Result.Should().BeEquivalentTo(Values);
    }

    [Test]
    public void WhenStartsWith_AllMatch()
    {
        WithValues("abc", "abdef", "abghi");

        Run(FilterRule.StartsWith(FieldId, "ab"));

        Result.Should().BeEquivalentTo(Values);
    }

    [Test]
    public void WhenNotStartsWith_AllMatch_ShouldReturnNone()
    {
        WithValues("abc", "abdef", "abghi");

        Run(FilterRule.NotStartsWith(FieldId, "ab"));

        Result.Should().BeEmpty();
    }

    [Test]
    public void WhenStartsWith_ExactMatch()
    {
        WithValues("hello", "help", "world");

        Run(FilterRule.StartsWith(FieldId, "hello"));

        Result.Should().BeEquivalentTo("hello");
    }

    [Test]
    public void WhenNotStartsWith_ExactMatch()
    {
        WithValues("hello", "help", "world");

        Run(FilterRule.NotStartsWith(FieldId, "hello"));

        Result.Should().BeEquivalentTo("help", "world");
    }

    [Test]
    public void WhenStartsWith_CaseSensitive_UpperVsLower()
    {
        WithValues("Apple", "apple", "APPLE", "application");

        Run(FilterRule.StartsWith(FieldId, "app"));

        Result.Should().BeEquivalentTo("apple", "application");
    }

    [Test]
    public void WhenNotStartsWith_CaseSensitive_UpperVsLower()
    {
        WithValues("Apple", "apple", "APPLE", "application");

        Run(FilterRule.NotStartsWith(FieldId, "app"));

        Result.Should().BeEquivalentTo("Apple", "APPLE");
    }

    [Test]
    public void WhenStartsWith_PrefixLongerThanValue()
    {
        WithValues("ab", "abc", "abcd", "a");

        Run(FilterRule.StartsWith(FieldId, "abcdef"));

        Result.Should().BeEmpty();
    }

    [Test]
    public void WhenNotStartsWith_PrefixLongerThanValue()
    {
        WithValues("ab", "abc", "abcd", "a");

        Run(FilterRule.NotStartsWith(FieldId, "abcdef"));

        Result.Should().BeEquivalentTo(Values);
    }

    [Test]
    public void WhenStartsWith_SpecialCharacters()
    {
        WithValues("hello world", "hello!", "help me", "world hello");

        Run(FilterRule.StartsWith(FieldId, "hello"));

        Result.Should().BeEquivalentTo("hello world", "hello!");
    }

    [Test]
    public void WhenStartsWith_WithSpacePrefix()
    {
        WithValues(" leading", "leading", " also leading");

        Run(FilterRule.StartsWith(FieldId, " "));

        Result.Should().BeEquivalentTo(" leading", " also leading");
    }

    [Test]
    public void WhenStartsWith_SingleCharacterValues()
    {
        WithValues("a", "b", "c", "d");

        Run(FilterRule.StartsWith(FieldId, "a"));

        Result.Should().BeEquivalentTo("a");
    }

    [Test]
    public void WhenStartsWith_MultibytePrefix()
    {
        WithValues("prefix-one", "prefix-two", "other");

        Run(FilterRule.StartsWith(FieldId, "prefix-"));

        Result.Should().BeEquivalentTo("prefix-one", "prefix-two");
    }
}
