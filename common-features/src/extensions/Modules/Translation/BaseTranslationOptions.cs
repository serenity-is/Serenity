namespace Serenity.Extensions;

[DefaultSectionKey(SectionKey)]
public class BaseTranslationOptions
{
    public const string SectionKey = "Translation";
    public bool Enabled { get; set; }
    public int ParallelRequests { get; set; } = 1;
    public int BatchSize { get; set; } = 1;
}
