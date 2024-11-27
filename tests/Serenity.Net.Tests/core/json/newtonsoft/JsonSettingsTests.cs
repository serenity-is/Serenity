using Newtonsoft.Json.Converters;

namespace Serenity.Tests.Json;

public partial class JsonSettingsTests
{
    [Fact]
    public void JsonSettings_Tolerant_Uses_NullValueHandling_Ignore()
    {
        Assert.Equal(Newtonsoft.Json.NullValueHandling.Ignore, JsonSettings.Tolerant.NullValueHandling);
    }

    [Fact]
    public void JsonSettings_TolerantIncludeNulls_Uses_NullValueHandling_Include()
    {
        Assert.Equal(Newtonsoft.Json.NullValueHandling.Include, JsonSettings.TolerantIncludeNulls.NullValueHandling);
    }

    [Fact]
    public void JsonSettings_Strict_Uses_NullValueHandling_Ignore()
    {
        Assert.Equal(Newtonsoft.Json.NullValueHandling.Ignore, JsonSettings.Strict.NullValueHandling);
    }

    [Fact]
    public void JsonSettings_StrictIncludeNulls_Uses_NullValueHandling_Include()
    {
        Assert.Equal(Newtonsoft.Json.NullValueHandling.Include, JsonSettings.StrictIncludeNulls.NullValueHandling);
    }

    [Fact]
    public void JsonSettings_Tolerant_Uses_MissingMemberHandling_Ignore()
    {
        Assert.Equal(Newtonsoft.Json.MissingMemberHandling.Ignore, JsonSettings.Tolerant.MissingMemberHandling);
    }

    [Fact]
    public void JsonSettings_TolerantIncludeNulls_Uses_MissingMemberHandling_Ignore()
    {
        Assert.Equal(Newtonsoft.Json.MissingMemberHandling.Ignore, JsonSettings.TolerantIncludeNulls.MissingMemberHandling);
    }

    [Fact]
    public void JsonSettings_Strict_Uses_MissingMemberHandling_Error()
    {
        Assert.Equal(Newtonsoft.Json.MissingMemberHandling.Error, JsonSettings.Strict.MissingMemberHandling);
    }

    [Fact]
    public void JsonSettings_StrictIncludeNulls_Uses_MissingMemberHandling_Error()
    {
        Assert.Equal(Newtonsoft.Json.MissingMemberHandling.Error, JsonSettings.StrictIncludeNulls.MissingMemberHandling);
    }

    [Fact]
    public void JsonSettings_Tolerant_Uses_TypeNameHandling_None()
    {
        Assert.Equal(Newtonsoft.Json.TypeNameHandling.None, JsonSettings.Tolerant.TypeNameHandling);
    }

    [Fact]
    public void JsonSettings_TolerantIncludeNulls_Uses_TypeNameHandling_None()
    {
        Assert.Equal(Newtonsoft.Json.TypeNameHandling.None, JsonSettings.TolerantIncludeNulls.TypeNameHandling);
    }

    [Fact]
    public void JsonSettings_Strict_Uses_TypeNameHandling_None()
    {
        Assert.Equal(Newtonsoft.Json.TypeNameHandling.None, JsonSettings.Strict.TypeNameHandling);
    }

    [Fact]
    public void JsonSettings_StrictIncludeNulls_Uses_TypeNameHandling_None()
    {
        Assert.Equal(Newtonsoft.Json.TypeNameHandling.None, JsonSettings.StrictIncludeNulls.TypeNameHandling);
    }

    [Fact]
    public void JsonSettings_Tolerant_Uses_ReferenceLoopHandling_Ignore()
    {
        Assert.Equal(Newtonsoft.Json.ReferenceLoopHandling.Ignore, JsonSettings.Tolerant.ReferenceLoopHandling);
    }

    [Fact]
    public void JsonSettings_TolerantIncludeNulls_Uses_ReferenceLoopHandling_Ignore()
    {
        Assert.Equal(Newtonsoft.Json.ReferenceLoopHandling.Ignore, JsonSettings.TolerantIncludeNulls.ReferenceLoopHandling);
    }

    [Fact]
    public void JsonSettings_Strict_Uses_ReferenceLoopHandling_Error()
    {
        Assert.Equal(Newtonsoft.Json.ReferenceLoopHandling.Error, JsonSettings.Strict.ReferenceLoopHandling);
    }

    [Fact]
    public void JsonSettings_StrictIncludeNulls_Uses_ReferenceLoopHandling_Error()
    {
        Assert.Equal(Newtonsoft.Json.ReferenceLoopHandling.Error, JsonSettings.StrictIncludeNulls.ReferenceLoopHandling);
    }

    [Fact]
    public void JsonSettings_Tolerant_Uses_PreserveReferencesHandling_None()
    {
        Assert.Equal(Newtonsoft.Json.PreserveReferencesHandling.None, JsonSettings.Tolerant.PreserveReferencesHandling);
    }

    [Fact]
    public void JsonSettings_TolerantIncludeNulls_Uses_PreserveReferencesHandling_None()
    {
        Assert.Equal(Newtonsoft.Json.PreserveReferencesHandling.None, JsonSettings.TolerantIncludeNulls.PreserveReferencesHandling);
    }

    [Fact]
    public void JsonSettings_Strict_Uses_PreserveReferencesHandling_None()
    {
        Assert.Equal(Newtonsoft.Json.PreserveReferencesHandling.None, JsonSettings.Strict.PreserveReferencesHandling);
    }

    [Fact]
    public void JsonSettings_StrictIncludeNulls_Uses_PreserveReferencesHandling_None()
    {
        Assert.Equal(Newtonsoft.Json.PreserveReferencesHandling.None, JsonSettings.StrictIncludeNulls.PreserveReferencesHandling);
    }

    [Fact]
    public void JsonSettings_Tolerant_Has_IsoDateTime_And_JsonSafeInt64_Converters()
    {
        Assert.NotNull(JsonSettings.Tolerant.Converters);
        Assert.Equal(2, JsonSettings.Tolerant.Converters.Count);
        Assert.IsType<IsoDateTimeConverter>(JsonSettings.Tolerant.Converters[0]);
        Assert.IsType<JsonSafeInt64Converter>(JsonSettings.Tolerant.Converters[1]);
    }

    [Fact]
    public void JsonSettings_TolerantIncludeNulls_Has_IsoDateTime_And_JsonSafeInt64_Converters()
    {
        Assert.NotNull(JsonSettings.TolerantIncludeNulls.Converters);
        Assert.Equal(2, JsonSettings.TolerantIncludeNulls.Converters.Count);
        Assert.IsType<IsoDateTimeConverter>(JsonSettings.TolerantIncludeNulls.Converters[0]);
        Assert.IsType<JsonSafeInt64Converter>(JsonSettings.TolerantIncludeNulls.Converters[1]);
    }

    [Fact]
    public void JsonSettings_Strict_Has_IsoDateTime_And_JsonSafeInt64_Converters()
    {
        Assert.NotNull(JsonSettings.Strict.Converters);
        Assert.Equal(2, JsonSettings.Strict.Converters.Count);
        Assert.IsType<IsoDateTimeConverter>(JsonSettings.Strict.Converters[0]);
        Assert.IsType<JsonSafeInt64Converter>(JsonSettings.Strict.Converters[1]);
    }

    [Fact]
    public void JsonSettings_StrictIncludeNulls_Has_IsoDateTime_And_JsonSafeInt64_Converters()
    {
        Assert.NotNull(JsonSettings.StrictIncludeNulls.Converters);
        Assert.Equal(2, JsonSettings.StrictIncludeNulls.Converters.Count);
        Assert.IsType<IsoDateTimeConverter>(JsonSettings.StrictIncludeNulls.Converters[0]);
        Assert.IsType<JsonSafeInt64Converter>(JsonSettings.StrictIncludeNulls.Converters[1]);
    }

    [Fact]
    public void JsonSettings_CreateDefault_Has_Expected_Settings()
    {
        var settings = JsonSettings.CreateDefaults();
        Assert.Equal(Newtonsoft.Json.NullValueHandling.Include, settings.NullValueHandling);
        Assert.Equal(Newtonsoft.Json.MissingMemberHandling.Ignore, settings.MissingMemberHandling);
        Assert.Equal(Newtonsoft.Json.TypeNameHandling.None, settings.TypeNameHandling);
        Assert.Equal(Newtonsoft.Json.ReferenceLoopHandling.Error, settings.ReferenceLoopHandling);
        Assert.Equal(Newtonsoft.Json.PreserveReferencesHandling.None, settings.PreserveReferencesHandling);
        Assert.NotNull(settings.Converters);
        Assert.Equal(2, settings.Converters.Count);
        Assert.IsType<IsoDateTimeConverter>(settings.Converters[0]);
        Assert.IsType<JsonSafeInt64Converter>(settings.Converters[1]);
    }
}