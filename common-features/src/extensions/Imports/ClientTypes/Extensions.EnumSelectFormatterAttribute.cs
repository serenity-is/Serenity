namespace Serenity.Extensions;

public partial class EnumSelectFormatterAttribute : CustomFormatterAttribute
{
    public const string Key = "Serenity.Extensions.EnumSelectFormatter";

    public EnumSelectFormatterAttribute()
        : base(Key)
    {
    }

    public bool AllowClear
    {
        get { return GetOption<bool>("allowClear"); }
        set { SetOption("allowClear", value); }
    }

    public string EmptyItemText
    {
        get { return GetOption<string>("emptyItemText"); }
        set { SetOption("emptyItemText", value); }
    }

    public string EnumKey
    {
        get { return GetOption<string>("enumKey"); }
        set { SetOption("enumKey", value); }
    }
}