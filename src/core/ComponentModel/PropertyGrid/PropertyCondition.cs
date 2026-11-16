namespace Serenity.ComponentModel;

/// <summary>
/// A condition on the value of another field of a form, which a field is shown, made read only or required by, see
/// <see cref="VisibleWhenAttribute"/>, <see cref="ReadOnlyWhenAttribute"/> and <see cref="RequiredWhenAttribute"/>.
/// </summary>
public class PropertyCondition
{
    /// <summary>
    /// Gets or sets the name of the field whose value is checked, as the form has it, e.g. a property name.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("field")]
    [JsonPropertyName("field")]
    public string Field { get; set; } = "";

    /// <summary>
    /// Gets or sets the values that meet the condition, any of which the field can have. When there are none, the
    /// condition is met when the field has a value, which is one that is not null, an empty string, an empty list or false.
    /// Enum values are given as their numbers, as the editors have them.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("values", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    [JsonPropertyName("values")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object?[]? Values { get; set; }

    /// <summary>
    /// Gets or sets whether the condition is the opposite, e.g. met when the field has none of the values, or has no value.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("isNot", DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    [JsonPropertyName("isNot")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool IsNot { get; set; }

    /// <summary>
    /// Gets whether a value meets the condition, by the same rules as the forms: a value matches one of the values when
    /// they are the same as text, e.g. 2 and "2", or true and "true", and a list matches when any of its items does.
    /// It can be used to check on the server what a form checks, e.g. that a field required by a condition has a value.
    /// </summary>
    /// <param name="value">The value of the field.</param>
    public bool IsMet(object? value)
    {
        var met = Values is { Length: > 0 } ? Matches(value, Values) : HasValue(value);
        return met != IsNot;
    }

    /// <summary>
    /// Gets whether a value is one, i.e. it is not null, an empty string, an empty list or false.
    /// </summary>
    /// <param name="value">The value.</param>
    public static bool HasValue(object? value)
    {
        return value switch
        {
            null => false,
            string s => s.Length > 0,
            bool b => b,
            System.Collections.IEnumerable list => list.Cast<object?>().Any(),
            _ => true
        };
    }

    private static bool Matches(object? value, object?[] values)
    {
        if (value is System.Collections.IEnumerable list and not string)
            return list.Cast<object?>().Any(x => Matches(x, values));

        var text = ToText(value);
        return values.Any(x => ToText(x) == text);
    }

    /// <summary>
    /// Gets the text a value is compared by, as the forms compare them: enum values by their numbers, booleans as
    /// true and false, and numbers in the invariant culture.
    /// </summary>
    /// <param name="value">The value.</param>
    public static string? ToText(object? value)
    {
        return value switch
        {
            null => null,
            "" => null,
            bool b => b ? "true" : "false",
            Enum e => Convert.ToInt64(e, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture),
            IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString()
        };
    }
}
