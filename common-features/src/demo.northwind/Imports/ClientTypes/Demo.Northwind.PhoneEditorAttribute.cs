namespace Serenity.Demo.Northwind;

public partial class PhoneEditorAttribute : CustomEditorAttribute
{
    public const string Key = "Serenity.Demo.Northwind.PhoneEditor";

    public PhoneEditorAttribute()
        : base(Key)
    {
    }

    public bool Multiple
    {
        get { return GetOption<bool>("multiple"); }
        set { SetOption("multiple", value); }
    }
}