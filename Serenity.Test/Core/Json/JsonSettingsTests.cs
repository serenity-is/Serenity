using Newtonsoft.Json.Converters;
using Serenity.Data;
using Xunit;

namespace Serenity.Test
{
    [Collection("AvoidParallel")]
    public partial class JsonSettingsTests
    {
        [Fact]
        public void JsonSettings_Tolerant_Uses_NullValueHandling_Ignore()
        {
            Assert.Equal(JsonSettings.Tolerant.NullValueHandling, Newtonsoft.Json.NullValueHandling.Ignore);
        }

        [Fact]
        public void JsonSettings_TolerantIncludeNulls_Uses_NullValueHandling_Include()
        {
            Assert.Equal(JsonSettings.TolerantIncludeNulls.NullValueHandling, Newtonsoft.Json.NullValueHandling.Include);
        }

        [Fact]
        public void JsonSettings_Strict_Uses_NullValueHandling_Ignore()
        {
            Assert.Equal(JsonSettings.Strict.NullValueHandling, Newtonsoft.Json.NullValueHandling.Ignore);
        }

        [Fact]
        public void JsonSettings_StrictIncludeNulls_Uses_NullValueHandling_Include()
        {
            Assert.Equal(JsonSettings.StrictIncludeNulls.NullValueHandling, Newtonsoft.Json.NullValueHandling.Include);
        }

        [Fact]
        public void JsonSettings_Tolerant_Uses_MissingMemberHandling_Ignore()
        {
            Assert.Equal(JsonSettings.Tolerant.MissingMemberHandling, Newtonsoft.Json.MissingMemberHandling.Ignore);
        }

        [Fact]
        public void JsonSettings_TolerantIncludeNulls_Uses_MissingMemberHandling_Ignore()
        {
            Assert.Equal(JsonSettings.TolerantIncludeNulls.MissingMemberHandling, Newtonsoft.Json.MissingMemberHandling.Ignore);
        }

        [Fact]
        public void JsonSettings_Strict_Uses_MissingMemberHandling_Error()
        {
            Assert.Equal(JsonSettings.Strict.MissingMemberHandling, Newtonsoft.Json.MissingMemberHandling.Error);
        }

        [Fact]
        public void JsonSettings_StrictIncludeNulls_Uses_MissingMemberHandling_Error()
        {
            Assert.Equal(JsonSettings.StrictIncludeNulls.MissingMemberHandling, Newtonsoft.Json.MissingMemberHandling.Error);
        }

        [Fact]
        public void JsonSettings_Tolerant_Uses_TypeNameHandling_None()
        {
            Assert.Equal(JsonSettings.Tolerant.TypeNameHandling, Newtonsoft.Json.TypeNameHandling.None);
        }

        [Fact]
        public void JsonSettings_TolerantIncludeNulls_Uses_TypeNameHandling_None()
        {
            Assert.Equal(JsonSettings.TolerantIncludeNulls.TypeNameHandling, Newtonsoft.Json.TypeNameHandling.None);
        }

        [Fact]
        public void JsonSettings_Strict_Uses_TypeNameHandling_None()
        {
            Assert.Equal(JsonSettings.Strict.TypeNameHandling, Newtonsoft.Json.TypeNameHandling.None);
        }

        [Fact]
        public void JsonSettings_StrictIncludeNulls_Uses_TypeNameHandling_None()
        {
            Assert.Equal(JsonSettings.StrictIncludeNulls.TypeNameHandling, Newtonsoft.Json.TypeNameHandling.None);
        }

        [Fact]
        public void JsonSettings_Tolerant_Uses_ReferenceLoopHandling_Ignore()
        {
            Assert.Equal(JsonSettings.Tolerant.ReferenceLoopHandling, Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }

        [Fact]
        public void JsonSettings_TolerantIncludeNulls_Uses_ReferenceLoopHandling_Ignore()
        {
            Assert.Equal(JsonSettings.TolerantIncludeNulls.ReferenceLoopHandling, Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }

        [Fact]
        public void JsonSettings_Strict_Uses_ReferenceLoopHandling_Error()
        {
            Assert.Equal(JsonSettings.Strict.ReferenceLoopHandling, Newtonsoft.Json.ReferenceLoopHandling.Error);
        }

        [Fact]
        public void JsonSettings_StrictIncludeNulls_Uses_ReferenceLoopHandling_Error()
        {
            Assert.Equal(JsonSettings.StrictIncludeNulls.ReferenceLoopHandling, Newtonsoft.Json.ReferenceLoopHandling.Error);
        }

        [Fact]
        public void JsonSettings_Tolerant_Uses_PreserveReferencesHandling_None()
        {
            Assert.Equal(JsonSettings.Tolerant.PreserveReferencesHandling, Newtonsoft.Json.PreserveReferencesHandling.None);
        }

        [Fact]
        public void JsonSettings_TolerantIncludeNulls_Uses_PreserveReferencesHandling_None()
        {
            Assert.Equal(JsonSettings.TolerantIncludeNulls.PreserveReferencesHandling, Newtonsoft.Json.PreserveReferencesHandling.None);
        }

        [Fact]
        public void JsonSettings_Strict_Uses_PreserveReferencesHandling_None()
        {
            Assert.Equal(JsonSettings.Strict.PreserveReferencesHandling, Newtonsoft.Json.PreserveReferencesHandling.None);
        }

        [Fact]
        public void JsonSettings_StrictIncludeNulls_Uses_PreserveReferencesHandling_None()
        {
            Assert.Equal(JsonSettings.StrictIncludeNulls.PreserveReferencesHandling, Newtonsoft.Json.PreserveReferencesHandling.None);
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
            Assert.Equal(settings.NullValueHandling, Newtonsoft.Json.NullValueHandling.Include);
            Assert.Equal(settings.MissingMemberHandling, Newtonsoft.Json.MissingMemberHandling.Ignore);
            Assert.Equal(settings.TypeNameHandling, Newtonsoft.Json.TypeNameHandling.None);
            Assert.Equal(settings.ReferenceLoopHandling, Newtonsoft.Json.ReferenceLoopHandling.Error);
            Assert.Equal(settings.PreserveReferencesHandling, Newtonsoft.Json.PreserveReferencesHandling.None);
            Assert.NotNull(settings.Converters);
            Assert.Equal(2, settings.Converters.Count);
            Assert.IsType<IsoDateTimeConverter>(settings.Converters[0]);
            Assert.IsType<JsonSafeInt64Converter>(settings.Converters[1]);
        }
    }
}