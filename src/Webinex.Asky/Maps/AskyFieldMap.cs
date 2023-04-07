using System.Linq.Expressions;

namespace Webinex.Asky;

public static class AskyFieldMap
{
    public static Expression<Func<TParent, object>>? Forward<TParent, TChild>(
        Expression<Func<TParent, TChild>> selector,
        IAskyFieldMap<TChild> childMap,
        string fieldId)
    {
        var childFieldSelector = childMap[fieldId];
        if (childFieldSelector == null)
            return null;
        
        return LambdaExpressions.Child(selector, childFieldSelector);
    }
}