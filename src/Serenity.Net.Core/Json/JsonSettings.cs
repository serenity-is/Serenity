using Newtonsoft.Json.Converters;

namespace Serenity;

/// <summary>
/// Contains default Serenity JSON serialization settings.
/// </summary>
public static class JsonSettings
{
    /// <summary>
    /// The tolerant settings, ignores missing members, reference loops on deserialization, ignores nulls
    /// </summary>
    public static JsonSerializerSettings Tolerant;

    /// <summary>
    /// The tolerant settings, ignores missing members, reference loops on deserialization, includes nulls
    /// </summary>
    public static JsonSerializerSettings TolerantIncludeNulls;

    /// <summary>
    /// The stricter settings, raises error on missing members / reference loops, ignores nulls.
    /// </summary>
    public static JsonSerializerSettings Strict;

    /// <summary>
    /// The stricter settings, raises error on missing members / reference loops, includes nulls.
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
    /// Creates a JsonSerializerSettings object with common values and converters.
    /// </summary>
    /// <returns></returns>
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
