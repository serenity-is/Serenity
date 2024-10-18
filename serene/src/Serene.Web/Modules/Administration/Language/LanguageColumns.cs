namespace Serene.Administration.Forms;

[ColumnsScript("Administration.Language")]
[BasedOnRow(typeof(LanguageRow), CheckNames = true)]
public class LanguageColumns
{
    [EditLink]
    public string LanguageId { get; set; }
    [EditLink, SortOrder(1)]
    public string LanguageName { get; set; }
}