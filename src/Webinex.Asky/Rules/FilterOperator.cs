﻿namespace Webinex.Asky;

public static class FilterOperator
{
    public const string EQ = "=";
    public const string NOT_EQ = "!=";
    public const string GTE = ">=";
    public const string GT = ">";
    public const string LTE = "<=";
    public const string LT = "<";
    public const string CONTAINS = "contains";
    public const string NOT_CONTAINS = "!contains";
    public const string AND = "and";
    public const string OR = "or";
    public const string IN = "in";
    public const string NOT_IN = "!in";
    public const string ANY = "any";
    public const string ALL = "all";
    public const string STARTS_WITH = "startsWith";
    public const string NOT_STARTS_WITH = "!startsWith";

    public static readonly HashSet<string> ALL_VALUES = new()
    {
        EQ, NOT_EQ, GTE, GT, LTE, LT, CONTAINS, NOT_CONTAINS, STARTS_WITH, NOT_STARTS_WITH,
    };

    public static readonly HashSet<string> ALL_BOOL = new()
    {
        AND, OR,
    };

    public static readonly HashSet<string> ALL_COLLECTION = new()
    {
        IN, NOT_IN,
    };

    public static readonly HashSet<string> ALL_CHILD_COLLECTION = new()
    {
        ANY, ALL,
    };

    public static readonly HashSet<string> ALL_OPERATORS =
        new(ALL_VALUES.Concat(ALL_BOOL).Concat(ALL_COLLECTION).Concat(ALL_CHILD_COLLECTION));

    public static readonly HashSet<string> ALL_COLLECTION_FIELD = new()
    {
        CONTAINS, NOT_CONTAINS,
    };
}