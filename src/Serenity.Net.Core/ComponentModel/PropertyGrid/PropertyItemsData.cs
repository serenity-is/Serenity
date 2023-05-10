namespace Serenity.ComponentModel;

/// <summary>
/// The data format of a property items script (form or columns)
/// </summary>
public class PropertyItemsData
{
    /// <summary>
    /// List of items
    /// </summary>
    [JsonProperty("items")]
    public List<PropertyItem>? Items { get; set; }

    /// <summary>
    /// List of additional items that are not directly included in the form / columns
    /// These usually include join key fields
    /// </summary>
    [JsonProperty("additionalItems")]
    public List<PropertyItem>? AdditionalItems { get; set; }
}