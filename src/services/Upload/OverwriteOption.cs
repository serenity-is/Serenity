namespace Serenity.Web;

/// <summary>
/// Enum that contains what to do when a file at desired path exists
/// </summary>
public enum OverwriteOption
{
    /// <summary>
    /// Raise an error
    /// </summary>
    Disallowed = 0,

    /// <summary>
    /// Overwrite the target file
    /// </summary>
    Overwrite = 1,

    /// <summary>
    /// Try to find a suitable name for the source file to be written
    /// </summary>
    AutoRename = 2
}
