namespace Serenity.Web;

/// <summary>
/// Options for layout script files declared with <see cref="LayoutScriptFileAttribute"/>.
/// </summary>
public class LayoutScriptFileOptions
{
    /// <summary>
    /// Gets or sets an optional filter that can be used to exclude some of the
    /// declared layout script files. Returning <c>false</c> skips the file.
    /// </summary>
    public Func<LayoutScriptFileAttribute, bool>? Filter { get; set; }
}
