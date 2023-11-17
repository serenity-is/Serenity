using System.Text.Json;
using System.Text.Unicode;

namespace Serenity;

/// <summary>
/// Contains Serenity JSON serialization defaults.
/// </summary>
public static class JsonDefaults
{
    /// <summary>
    /// The stricter settings, raises error on missing members / reference loops, skips nulls when serializing
    /// </summary>
    public static readonly JsonSerializerOptions Strict;

    /// <summary>
    /// The stricter settings, raises error on missing members / reference loops, writes nulls
    /// </summary>
    public static readonly JsonSerializerOptions StrictWriteNulls;

    /// <summary>
    /// The tolerant settings, ignores missing members, reference loops on deserialization, skips nulls when serializing
    /// </summary>
    public static readonly JsonSerializerOptions Tolerant;

    /// <summary>
    /// The tolerant settings, ignores missing members, reference loops on deserialization, writes nulls
    /// </summary>
    public static readonly JsonSerializerOptions TolerantWriteNulls;
    
    static JsonDefaults()
    {
        Strict = CreateDefaults();
        Strict.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        Strict.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow;

        StrictWriteNulls = CreateDefaults();
        StrictWriteNulls.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow;

        Tolerant = CreateDefaults();
        Tolerant.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        Tolerant.ReferenceHandler = ReferenceHandler.IgnoreCycles;

        TolerantWriteNulls = CreateDefaults();
        TolerantWriteNulls.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    }

    /// <summary>
    /// Creates a JsonSerializerSettings object with common values and converters.
    /// </summary>
    /// <returns></returns>
    public static JsonSerializerOptions CreateDefaults()
    {
        return new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All),
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals | JsonNumberHandling.AllowReadingFromString,
            Converters = {
                JsonConverters.JsonLargeLongConverter.Instance
            }
        };
    }
}
