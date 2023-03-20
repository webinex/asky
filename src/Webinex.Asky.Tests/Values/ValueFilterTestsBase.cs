using System;
using System.Linq;
using NUnit.Framework;

namespace Webinex.Asky.Tests.Values;

internal class ValueFilterTestsBase<T>
{
    protected const string FieldId = nameof(Tuple<T>.Item1);
    protected T[] Values;
    protected Tuple<T>[] Source => Values.Select(x => Tuple.Create(x)).ToArray();
    protected TupleFieldMap<T> FieldMap = new();
    protected T[] Result;

    protected void WithValues(params T[] values)
    {
        Values = values;
    }

    protected void Run(FilterRule filter)
    {
        var result = Source.AsQueryable().Where(new TupleFieldMap<T>(), filter).ToArray();
        Result = result.Select(x => x.Item1).ToArray();
    }

    [SetUp]
    public void SetUp()
    {
        Values = null;
        Result = null;
    }
}