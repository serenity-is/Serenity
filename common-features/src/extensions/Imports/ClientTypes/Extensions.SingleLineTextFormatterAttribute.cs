namespace Serenity.Extensions;

public partial class SingleLineTextFormatterAttribute : CustomFormatterAttribute
{
    public const string Key = "Serenity.Extensions.SingleLineTextFormatter";

    public SingleLineTextFormatterAttribute()
        : base(Key)
    {
    }
}