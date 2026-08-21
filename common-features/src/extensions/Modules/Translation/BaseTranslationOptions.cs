namespace Serenity.Extensions;

/// <summary>
/// Options for the translation feature.
/// </summary>
[DefaultSectionKey(SectionKey)]
public class BaseTranslationOptions
{
    /// <summary>
    /// The configuration section key for translation options.
    /// </summary>
    public const string SectionKey = "Translation";

    /// <summary>
    /// Gets or sets a value indicating whether the translation feature is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the number of parallel translation requests to make.
    /// </summary>
    public int ParallelRequests { get; set; } = 1;

    /// <summary>
    /// Gets or sets the number of texts to translate in a single batch.
    /// </summary>
    public int BatchSize { get; set; } = 1;
}
