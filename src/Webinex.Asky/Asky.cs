using System.Linq.Expressions;

namespace Webinex.Asky;

public static class Asky
{
    public static Expression<Func<T, bool>> Predicate<T>(
        IAskyFieldMap<T> fieldMap,
        FilterRule filterRule,
        FilterOptions options = FilterOptions.None)
    {
        return AskyExpressionFactory.Create(fieldMap, filterRule, options);
    }
}