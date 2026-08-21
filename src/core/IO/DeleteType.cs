
namespace Serenity.IO;

/// <summary>
/// Specifies how a file should be deleted.
/// </summary>
public enum DeleteType
{
    /// <summary>
    /// Force delete the file.
    /// </summary>
    Delete,
    /// <summary>
    /// Try to delete the file, ignoring any errors.
    /// </summary>
    TryDelete,
    /// <summary>
    /// Try to delete the file, or mark it for deletion if it cannot be deleted.
    /// </summary>
    TryDeleteOrMark
}
