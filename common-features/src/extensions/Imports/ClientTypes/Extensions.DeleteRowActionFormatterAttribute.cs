namespace Serenity.Extensions;

public partial class DeleteRowActionFormatterAttribute : CustomFormatterAttribute
{
    public const string Key = "Serenity.Extensions.DeleteRowActionFormatter";

    public DeleteRowActionFormatterAttribute()
        : base(Key)
    {
    }
}