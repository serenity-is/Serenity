namespace Serenity.ComponentModel;

/// <summary>
/// Editor addon item used within PropertyItem class
/// </summary>
public class EditorAddonItem
{
    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("type")]
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the params.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("params")]
    [JsonPropertyName("params")]
    public Dictionary<string, object?>? Params { get; set; }
}