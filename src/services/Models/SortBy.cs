namespace Serenity.Services;

/// <summary>
/// A sort column. In JSON this is deserialized from strings
/// in the format "field" or "field desc"
/// </summary>
[Newtonsoft.Json.JsonConverter(typeof(JsonSortByConverter))]
[JsonConverter(typeof(JsonConverters.SortByJsonConverter))]
public class SortBy
{
    /// <summary>
    /// Creates an empty SortBy object
    /// </summary>
    public SortBy()
    {
    }

    /// <summary>
    /// Creates a SortBy object for the specified field name.
    /// </summary>
    /// <param name="field">The field name.</param>
    public SortBy(string field)
    {
        Field = field;
    }

    /// <summary>
    /// Creates an instance of the SortBy object containing the
    /// specified field name and descending flag.
    /// </summary>
    /// <param name="field">The field name.</param>
    /// <param name="descending">Whether to sort in descending order.</param>
    public SortBy(string field, bool descending)
    {
        Field = field;
        Descending = descending;
    }

    /// <summary>
    /// Gets/sets the field name
    /// </summary>
    public string Field { get; set; }

    /// <summary>
    /// Gets/sets the descending flag
    /// </summary>
    public bool Descending { get; set; }
}
