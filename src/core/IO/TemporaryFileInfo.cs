#nullable enable
namespace Serenity;

/// <summary>
/// Represents basic information about a temporary file, as used by <see cref="Serenity.IO.TemporaryFileHelper"/>.
/// </summary>
public class TemporaryFileInfo
{
    /// <summary>
    /// Gets or sets the full name of the file, including its directory.
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// Gets or sets the name of the file.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the creation time of the file.
    /// </summary>
    public DateTime CreationTime { get; set; }
}
