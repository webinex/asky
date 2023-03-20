namespace Webinex.Asky;

public abstract class EqualityComparable
{
    protected abstract IEnumerable<object?> GetEqualityComponents();
    
    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (EqualityComparable)obj;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    public static bool operator ==(EqualityComparable? left, EqualityComparable? right)
    {
        return EqualOperator(left, right);
    }

    public static bool operator !=(EqualityComparable? left, EqualityComparable? right)
    {
        return NotEqualOperator(left, right);
    }

    protected static bool EqualOperator(EqualityComparable? left, EqualityComparable? right)
    {
        if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
        {
            return false;
        }

        return ReferenceEquals(left, null) || left.Equals(right);
    }

    protected static bool NotEqualOperator(EqualityComparable? left, EqualityComparable? right)
    {
        return !EqualOperator(left, right);
    }
}