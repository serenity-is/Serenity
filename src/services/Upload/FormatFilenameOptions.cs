namespace Serenity.Web;

/// <summary>
/// Options for formatting a file name
/// </summary>
public class FormatFilenameOptions
{
    /// <summary>
    /// Gets or sets the entity ID.
    /// </summary>
    public object EntityId { get; set; }

    /// <summary>
    /// Gets or sets the file name format.
    /// </summary>
    public string Format { get; set; }

    /// <summary>
    /// Gets or sets the original name of the file.
    /// </summary>
    public string OriginalName { get; set; }

    /// <summary>
    /// Gets or sets a callback that will be executed after formatting.
    /// </summary>
    public Func<string, string> PostFormat { get; set; }
}