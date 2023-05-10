
namespace Serenity.IO;

/// <summary>
///   File deletion type.</summary>
public enum DeleteType
{
    /// <summary>
    ///   Force delete</summary>
    Delete,
    /// <summary>
    ///   Try to delete</summary>
    TryDelete,
    /// <summary>
    ///   Try to delete and mark if can't delete</summary>
    TryDeleteOrMark
}
