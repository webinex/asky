using System.Linq.Expressions;

namespace Webinex.Asky;

public class AskyFieldMap<T> : IAskyFieldMap<T>
{
    private readonly IDictionary<string, Expression<Func<T, object>>> _fields;

    public AskyFieldMap(IDictionary<string, Expression<Func<T, object>>> fields)
    {
        _fields = fields ?? throw new ArgumentNullException(nameof(fields));
    }

    public Expression<Func<T, object>>? this[string fieldId] =>
        _fields.TryGetValue(fieldId, out var result) ? result : null;
}