using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Webinex.Asky.Tests;

public abstract class TupleFieldMapBase<T> : IAskyFieldMap<T>
{
    public abstract IDictionary<string, Expression<Func<T, object>>> Fields { get; }

    public Expression<Func<T, object>> this[string fieldId]
    {
        get
        {
            if (!Fields.ContainsKey(fieldId))
                throw new ArgumentOutOfRangeException(nameof(fieldId), $"Doesn't exist in {nameof(Fields)}");

            return Fields[fieldId];
        }
    }
}

public class TupleFieldMap<T1> : TupleFieldMapBase<Tuple<T1>>
{
    public override IDictionary<string, Expression<Func<Tuple<T1>, object>>> Fields { get; } =
        new Dictionary<string, Expression<Func<Tuple<T1>, object>>>
        {
            [nameof(Tuple<T1>.Item1)] = x => x.Item1,
        };
}

public class TupleFieldMap<T1, T2> : TupleFieldMapBase<Tuple<T1, T2>>
{
    public override IDictionary<string, Expression<Func<Tuple<T1, T2>, object>>> Fields { get; } =
        new Dictionary<string, Expression<Func<Tuple<T1, T2>, object>>>
        {
            [nameof(Tuple<T1, T2>.Item1)] = x => x.Item1,
            [nameof(Tuple<T1, T2>.Item2)] = x => x.Item2,
        };
}

public class TupleFieldMap<T1, T2, T3> : TupleFieldMapBase<Tuple<T1, T2, T3>>
{
    public override IDictionary<string, Expression<Func<Tuple<T1, T2, T3>, object>>> Fields { get; } =
        new Dictionary<string, Expression<Func<Tuple<T1, T2, T3>, object>>>
        {
            [nameof(Tuple<T1, T2, T3>.Item1)] = x => x.Item1,
            [nameof(Tuple<T1, T2, T3>.Item2)] = x => x.Item2,
            [nameof(Tuple<T1, T2, T3>.Item3)] = x => x.Item3,
        };
}

public class TupleFieldMap<T1, T2, T3, T4> : TupleFieldMapBase<Tuple<T1, T2, T3, T4>>
{
    public override IDictionary<string, Expression<Func<Tuple<T1, T2, T3, T4>, object>>> Fields { get; } =
        new Dictionary<string, Expression<Func<Tuple<T1, T2, T3, T4>, object>>>
        {
            [nameof(Tuple<T1, T2, T3, T4>.Item1)] = x => x.Item1,
            [nameof(Tuple<T1, T2, T3, T4>.Item2)] = x => x.Item2,
            [nameof(Tuple<T1, T2, T3, T4>.Item3)] = x => x.Item3,
            [nameof(Tuple<T1, T2, T3, T4>.Item4)] = x => x.Item4,
        };
}