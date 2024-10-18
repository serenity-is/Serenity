namespace Serenity.Demo.BasicSamples;

public partial class ChangingLookupTextEditorAttribute : LookupEditorBaseAttribute
{
    public const string Key = "Serenity.Demo.BasicSamples.ChangingLookupTextEditor";

    public ChangingLookupTextEditorAttribute()
        : base(Key)
    {
    }
}