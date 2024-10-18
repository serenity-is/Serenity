namespace Serenity.Extensions;

public partial class StaticTextBlockAttribute : CustomEditorAttribute
{
    public const string Key = "Serenity.Extensions.StaticTextBlock";

    public StaticTextBlockAttribute()
        : base(Key)
    {
    }

    public bool HideLabel
    {
        get { return GetOption<bool>("hideLabel"); }
        set { SetOption("hideLabel", value); }
    }

    public bool IsHtml
    {
        get { return GetOption<bool>("isHtml"); }
        set { SetOption("isHtml", value); }
    }

    public bool IsLocalText
    {
        get { return GetOption<bool>("isLocalText"); }
        set { SetOption("isLocalText", value); }
    }

    public string Text
    {
        get { return GetOption<string>("text"); }
        set { SetOption("text", value); }
    }
}