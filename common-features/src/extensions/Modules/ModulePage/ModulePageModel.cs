namespace Serenity.Web;

/// <summary>
/// The model for a module page.
/// </summary>
public class ModulePageModel
{
    /// <summary>
    /// The HTML markup to render in the page body.
    /// </summary>
    public string HtmlMarkup { get; set; }
    /// <summary>
    /// Optional options passed to the module script.
    /// </summary>
    public object Options { get; set; }
    /// <summary>
    /// The layout to use for the page.
    /// </summary>
    public string Layout { get; set; }
    /// <summary>
    /// The module name or script path.
    /// </summary>
    public string Module { get; set; }
    /// <summary>
    /// The page ID.
    /// </summary>
    public string PageId { get; set; }
    /// <summary>
    /// The page title.
    /// </summary>
    public LocalText PageTitle { get; set; }
}