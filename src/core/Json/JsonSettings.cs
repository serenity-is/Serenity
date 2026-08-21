using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Serenity;

/// <summary>
/// Contains the default Serenity JSON serialization settings for Newtonsoft.Json.
/// </summary>
public static class JsonSettings
{
    /// <summary>
    /// The tolerant settings, which ignore missing members and reference loops on deserialization, and ignore nulls.
    /// </summary>
    public static JsonSerializerSettings Tolerant;

    /// <summary>
    /// The tolerant settings, which ignore missing members and reference loops on deserialization, and include nulls.
    /// </summary>
    public static JsonSerializerSettings TolerantIncludeNulls;

    /// <summary>
    /// The stricter settings, which raise an error on missing members and reference loops, and ignore nulls.
    /// </summary>
    public static JsonSerializerSettings Strict;

    /// <summary>
    /// The stricter settings, which raise an error on missing members and reference loops, and include nulls.
    /// </summary>
    public static JsonSerializerSettings StrictIncludeNulls;

    static JsonSettings()
    {
        Tolerant = CreateDefaults();
        Tolerant.NullValueHandling = NullValueHandling.Ignore;
        Tolerant.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

        TolerantIncludeNulls = CreateDefaults();
        TolerantIncludeNulls.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

        Strict = CreateDefaults();
        Strict.NullValueHandling = NullValueHandling.Ignore;
        Strict.MissingMemberHandling = MissingMemberHandling.Error;

        StrictIncludeNulls = CreateDefaults();
        StrictIncludeNulls.MissingMemberHandling = MissingMemberHandling.Error;
    }

    /// <summary>
    /// Creates a <see cref="JsonSerializerSettings"/> object with the common Serenity values and converters.
    /// </summary>
    /// <returns>A <see cref="JsonSerializerSettings"/> instance with the default Serenity converters and options.</returns>
    public static JsonSerializerSettings CreateDefaults()
    {
        return new JsonSerializerSettings
        {
            DateParseHandling = DateParseHandling.DateTimeOffset,
            Converters = {
                new IsoDateTimeConverter(),
                new JsonSafeInt64Converter()
            }
        };
    }
}
