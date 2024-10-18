namespace Serenity.Web;

public class ModulePageModel
{
    public string HtmlMarkup { get; set; }
    public object Options { get; set; }
    public string Layout { get; set; }
    public string Module { get; set; }
    public string PageId { get; set; }
    public LocalText PageTitle { get; set; }
}