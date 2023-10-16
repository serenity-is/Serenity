#nullable enable
namespace Serenity;

/// <summary>
/// Base file info used by TemporaryFileHelper
/// </summary>
public class TemporaryFileInfo
{
    /// <summary>
    /// Full name of the file including directory
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// Name of the file
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Creation time of the file
    /// </summary>
    public DateTime CreationTime { get; set; }
}
