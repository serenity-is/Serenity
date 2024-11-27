namespace Serenity.Extensions;

[ScriptInclude]
public class TranslationItem
{
    public string Key { get; set; }
    public string SourceText { get; set; }
    public string TargetText { get; set; }
    public string CustomText { get; set; }
    public bool HasTranslation { get; set; }
    public bool UserTranslated { get; set; }
}