namespace Serenity.PropertyGrid;

/// <summary>
/// Property item generation options for BasicPropertyProcessor
/// </summary>
[DefaultSectionKey(SectionKey)]
public partial class PropertyProcessorOptions
{
    /// <summary>
    /// Gets or sets a the default summary type for numeric fields if no SummaryTypeAttribute is present.
    /// This only applies if the property is of a numeric type and is not a primary key, identity, 
    /// foreign key, unbound or has a LeftJoin attribute. Default is SummaryType.Sum.
    /// </summary>
    public SummaryType? DefaultSummaryType { get; set; }

    /// <summary>
    /// Default section key
    /// </summary>
    public const string SectionKey = "PropertyProcessor";
}