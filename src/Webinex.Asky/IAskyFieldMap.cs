using System.Linq.Expressions;

namespace Webinex.Asky;

public interface IAskyFieldMap<T>
{
    Expression<Func<T, object>>? this[string fieldId] { get; }
}

public static class AskyFieldMapExtensions
{
    public static Expression<Func<T, object>> Required<T>(this IAskyFieldMap<T> fieldMap, string fieldId)
    {
        fieldMap = fieldMap ?? throw new ArgumentNullException(nameof(fieldMap));
        fieldId = fieldId ?? throw new ArgumentNullException(nameof(fieldId));

        return fieldMap[fieldId] ??
               throw new InvalidOperationException($"{fieldId} not found in field map {fieldMap.GetType().FullName}");
    }
}