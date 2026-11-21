namespace Serenity.ComponentModel;

/// <summary>
/// A record view of a <see cref="PropertyItem"/>, see <see cref="RecordViewTypeAttribute"/>.
/// </summary>
public class RecordViewInfo
{
    /// <summary>
    /// Gets or sets the key of the view type.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("type")]
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets where the view is shown, in camel case, e.g. "trigger", see <see cref="RecordViewPlacement"/>.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("placement")]
    [JsonPropertyName("placement")]
    public string? Placement { get; set; }

    /// <summary>
    /// Gets or sets the options of the view, including its "idField".
    /// </summary>
    [Newtonsoft.Json.JsonProperty("params")]
    [JsonPropertyName("params")]
    public Dictionary<string, object?> Params
    {
        get;
        set => field = value ?? [];
    } = [];

    /// <summary>
    /// Creates the info of a record view from its attribute.
    /// </summary>
    /// <param name="attribute">The attribute.</param>
    /// <param name="idField">The property that has the id of the record the view shows, when the attribute does not
    /// say it, e.g. the one that the join of the property tells.</param>
    public static RecordViewInfo FromAttribute(RecordViewTypeAttribute attribute, string? idField = null)
    {
        if (attribute is null)
            throw new ArgumentNullException(nameof(attribute));

        string? placement = null;
        if (attribute.Placement != RecordViewPlacement.Trigger)
        {
            placement = attribute.Placement.ToString();
            placement = placement[..1].ToLowerInvariant() + placement[1..];
        }

        var info = new RecordViewInfo
        {
            Type = attribute.ViewType,
            Placement = placement
        };

        attribute.SetParams(info.Params);

        idField = attribute.IdField ?? idField;
        if (idField != null)
            info.Params["idField"] = idField;

        return info;
    }

    /// <summary>
    /// Should the Params property be serialized by JSON.NET, used to reduce JSON size.
    /// </summary>
    public bool ShouldSerializeParams()
    {
        return Params != null && Params.Count > 0;
    }
}
