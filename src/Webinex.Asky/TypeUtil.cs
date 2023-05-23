namespace Webinex.Asky;

internal static class TypeUtil
{
    public static Type GetGenericEnumerableImplValueType(Type collectionType)
    {
        if (collectionType.IsInterface && collectionType.IsConstructedGenericType &&
            collectionType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            return collectionType.GetGenericArguments()[0];
        
        return collectionType
            .GetInterfaces()
            .Where(t => t.IsGenericType
                        && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            .Select(t => t.GetGenericArguments()[0])
            .First();
    }
}