namespace Serenity.Demo.BasicSamples;

public partial class InlineImageFormatterAttribute : CustomFormatterAttribute
{
    public const string Key = "Serenity.Demo.BasicSamples.InlineImageFormatter";

    public InlineImageFormatterAttribute()
        : base(Key)
    {
    }

    public string FileProperty
    {
        get { return GetOption<string>("fileProperty"); }
        set { SetOption("fileProperty", value); }
    }

    public bool Thumb
    {
        get { return GetOption<bool>("thumb"); }
        set { SetOption("thumb", value); }
    }
}