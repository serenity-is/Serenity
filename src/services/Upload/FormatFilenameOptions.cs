namespace Serenity.Web;

/// <summary>
/// Options for formatting a file name
/// </summary>
public class FormatFilenameOptions
{
    /// <summary>
    /// Entity ID
    /// </summary>
    public object EntityId { get; set; }

    /// <summary>
    /// File name format
    /// </summary>
    public string Format { get; set; }

    /// <summary>
    /// The original name of the file
    /// </summary>
    public string OriginalName { get; set; }

    /// <summary>
    /// A call back that will be executed after formatting
    /// </summary>
    public Func<string, string> PostFormat { get; set; }
}