namespace Serenity.Demo.BasicSamples;

public partial class FilteredLookupDetailEditorAttribute : CustomEditorAttribute
{
    public const string Key = "Serenity.Demo.BasicSamples.FilteredLookupDetailEditor";

    public FilteredLookupDetailEditorAttribute()
        : base(Key)
    {
    }
}