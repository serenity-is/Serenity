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
    /// Should the Params property be serialized by JSON.NET, used to reduce JSON size.
    /// </summary>
    public bool ShouldSerializeParams()
    {
        return Params != null && Params.Count > 0;
    }
}
