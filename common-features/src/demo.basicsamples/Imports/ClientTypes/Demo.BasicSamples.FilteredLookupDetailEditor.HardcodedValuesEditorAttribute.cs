namespace Serenity.Demo.BasicSamples.FilteredLookupDetailEditor;

public partial class HardcodedValuesEditorAttribute : CustomEditorAttribute
{
    public const string Key = "Serenity.Demo.BasicSamples.FilteredLookupDetailEditor.HardcodedValuesEditor";

    public HardcodedValuesEditorAttribute()
        : base(Key)
    {
    }
}
