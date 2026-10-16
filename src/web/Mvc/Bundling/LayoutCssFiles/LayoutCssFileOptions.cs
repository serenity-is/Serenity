namespace Serenity.Web;

/// <summary>
/// Options for layout CSS files declared with <see cref="LayoutCssFileAttribute"/>.
/// </summary>
public class LayoutCssFileOptions
{
    /// <summary>
    /// Gets or sets an optional filter that can be used to exclude some of the
    /// declared layout CSS files. Returning <c>false</c> skips the file.
    /// </summary>
    public Func<LayoutCssFileAttribute, bool>? Filter { get; set; }
}
