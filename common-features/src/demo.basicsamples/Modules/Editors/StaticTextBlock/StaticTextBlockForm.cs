namespace Serenity.Demo.BasicSamples.Forms;

[FormScript("BasicSamples.StaticTextBlock")]
public class StaticTextBlockForm
{
    [DisplayName("Static Text")]
    [StaticTextBlock(Text = "Here is some static text")]
    public string? StaticText { get; set; }

    [StringEditor]
    public string? SomeInput { get; set; }

    [DisplayName("Static Html")]
    [StaticTextBlock(Text = "<h4 class='mt-0'>Here is an HTML list:</h2><ul><li>Item 1</li><li>Item 2</li><li>Item 3</li></ul>", IsHtml = true)]
    public string? HtmlList { get; set; }

    [DisplayName("From a Local Text")]
    [StaticTextBlock(Text = "Site.Dialogs.PendingChangesConfirmation", IsLocalText = true, IsHtml = true)]
    public string? FromLocalText { get; set; }

    [DisplayName("Display Field Value")]
    [StaticTextBlock(IsHtml = true)]
    public string? DisplayFieldValue { get; set; }
}