namespace Serenity.Data;

/// <summary>
/// Type of SQL UNIONs.
/// </summary>
public enum SqlUnionType
{
    /// <summary>
    /// UNION
    /// </summary>
    Union = 1,
    /// <summary>
    /// UNION ALL
    /// </summary>
    UnionAll = 2,
    /// <summary>
    /// INTERSECT
    /// </summary>
    Intersect = 3,
    /// <summary>
    /// INTERSECT ALL
    /// </summary>
    IntersectAll = 4,
    /// <summary>
    /// EXCEPT
    /// </summary>
    Except = 5,
    /// <summary>
    /// EXCEPT ALL
    /// </summary>
    ExceptAll = 6
}