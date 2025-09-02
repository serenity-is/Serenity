namespace Serenity.Services;

/// <summary>
/// Column selection types for List services
/// </summary>
public enum ColumnSelection
{
    /// <summary>
    /// List, e.g. only the table columns
    /// </summary>
    List = 0,
    /// <summary>
    /// Key Only, e.g. the primary key(s) and ID of the table
    /// </summary>
    KeyOnly = 1,
    /// <summary>
    /// Details, e.g. all the fields
    /// </summary>
    Details = 2,
    /// <summary>
    /// None, don't select any property by default
    /// </summary>
    None = 3,
    /// <summary>
    /// Only select Id property by default
    /// </summary>
    IdOnly = 4,
    /// <summary>
    /// Lookup, e.g. ID, Name and fields with LookupInclude attribute
    /// </summary>
    Lookup = 5
}