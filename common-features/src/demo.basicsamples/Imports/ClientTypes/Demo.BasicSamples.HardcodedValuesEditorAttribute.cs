namespace Serenity.Demo.BasicSamples;

public partial class HardcodedValuesEditorAttribute : CustomEditorAttribute
{
    public const string Key = "Serenity.Demo.BasicSamples.HardcodedValuesEditor";

    public HardcodedValuesEditorAttribute()
        : base(Key)
    {
    }
}