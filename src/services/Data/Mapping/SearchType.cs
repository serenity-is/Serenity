
namespace Serenity.Data.Mapping;

/// <summary>
/// Quick search type enumeration
/// </summary>
public enum SearchType
{
    /// <summary>
    /// Automatic mode. Equals for integer types, Contains for other.
    /// </summary>
    Auto = 0,

    /// <summary>
    /// Search with equality
    /// </summary>
    Equals = 1,

    /// <summary>
    /// Search with contains
    /// </summary>
    Contains = 2,

    /// <summary>
    /// Search with STARTS WITH
    /// </summary>
    StartsWith = 3,

    /// <summary>
    /// Use full text CONTAINS
    /// </summary>
    FullTextContains = 4
}