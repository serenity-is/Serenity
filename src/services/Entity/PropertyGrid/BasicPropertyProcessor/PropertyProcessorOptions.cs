namespace Serenity.PropertyGrid;

/// <summary>
/// Property item generation options for BasicPropertyProcessor
/// </summary>
[DefaultSectionKey(SectionKey)]
public partial class PropertyProcessorOptions
{
    /// <summary>
    /// Gets or sets the default summary type for numeric fields if no <see cref="SummaryTypeAttribute"/> is present.
    /// This only applies if the property is of a numeric type and is not a primary key, identity,
    /// foreign key, unbound or has a <see cref="LeftJoinAttribute"/>. Default is <see cref="SummaryType.Sum"/>.
    /// </summary>
    public SummaryType? DefaultSummaryType { get; set; }

    /// <summary>
    /// Default section key
    /// </summary>
    public const string SectionKey = "PropertyProcessor";
}