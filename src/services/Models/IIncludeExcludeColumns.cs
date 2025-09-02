namespace Serenity.Services;

/// <summary>
/// Abstraction for service objects containing
/// include / exclude column sets. 
/// See <see cref="ListRequest"/> and <see cref="RetrieveRequest"/>.
/// </summary>
public interface IIncludeExcludeColumns
{
    /// <summary>
    /// Set of include columns. These columns are supposed
    /// to be additionally selected.
    /// </summary>
    HashSet<string> IncludeColumns { get; set; }
    /// <summary>
    /// Set of exclude columns. These columns should not be
    /// selected even if they are selected by default by 
    /// the select level.
    /// </summary>
    HashSet<string> ExcludeColumns { get; set; }
}