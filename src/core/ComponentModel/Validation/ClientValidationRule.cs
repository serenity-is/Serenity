namespace Serenity.ComponentModel;

/// <summary>
/// A validation rule as the forms get it, from a validation attribute of a property, see
/// <see cref="ValidationRules.GetClientRule"/>.
/// </summary>
public class ClientValidationRule
{
    /// <summary>
    /// Gets or sets the name of the rule on the client side, e.g. <c>range</c>.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("rule")]
    [JsonPropertyName("rule")]
    public string Rule { get; set; } = "";

    /// <summary>
    /// Gets or sets the parameters of the rule, e.g. the minimum and the maximum of a range.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("params", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    [JsonPropertyName("params")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Params { get; set; }

    /// <summary>
    /// Gets or sets the local text key of the message, which is used when it has a text.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("textKey", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    [JsonPropertyName("textKey")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TextKey { get; set; }

    /// <summary>
    /// Gets or sets the message as it is, used when there is no text of the <see cref="TextKey"/>. The default message of the
    /// rule is used when neither is set.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("message", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    [JsonPropertyName("message")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; set; }
}
