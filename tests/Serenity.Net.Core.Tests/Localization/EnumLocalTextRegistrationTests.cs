using Serenity.ComponentModel;
using Serenity.Localization;

namespace Serenity.Tests.Localization;

public class EnumLocalTextRegistrationTests
{
    [EnumKey("My.CoolEnumKey")]
    public enum EnumWithKey
    {
        NoDescriptionKey,
        [Description("Description for WithDescriptionKey")]
        WithDescriptionKey
    }

    public enum EnumWithoutKey
    {
        [Description("Description for WithDescriptionNoKey")]
        WithDescriptionNoKey,
        NoDescriptionNoKey,
    }

    [Fact]
    public void AddEnumTexts_ThrowsArgumentNull_If_Registry_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            EnumLocalTextRegistration.AddEnumTexts(registry: null, new MockTypeSource(Array.Empty<Type>())));
    }

    [Fact]
    public void AddEnumTexts_ThrowsArgumentNull_If_TypeSource_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            EnumLocalTextRegistration.AddEnumTexts(registry: new LocalTextRegistry(), typeSource: null));
    }

    [Fact]
    public void AddEnumTexts_UsesFullName_If_EnumKeyAttribute_IsNotPresent()
    {
        var registry = new LocalTextRegistry();
        EnumLocalTextRegistration.AddEnumTexts(registry, new MockTypeSource(typeof(EnumWithoutKey)));

        string expectedKey = "Enums." + typeof(EnumWithoutKey).FullName + "." +
            EnumWithoutKey.WithDescriptionNoKey.GetName();

        var actualText = registry.TryGet(LocalText.InvariantLanguageID, expectedKey, pending: false);
        Assert.Equal("Description for WithDescriptionNoKey", actualText);
    }

    [Fact]
    public void AddEnumTexts_UsesKey_IfKeyAttributeIsPresent()
    {
        var registry = new LocalTextRegistry();
        EnumLocalTextRegistration.AddEnumTexts(registry, new MockTypeSource(typeof(EnumWithKey)));

        string expectedKey = "Enums.My.CoolEnumKey." + EnumWithKey.WithDescriptionKey.GetName();

        var actualText = registry.TryGet(LocalText.InvariantLanguageID, expectedKey, pending: false);
        Assert.Equal("Description for WithDescriptionKey", actualText);
    }

    [Fact]
    public void AddEnumTexts_Skips_EnumValues_Without_DescriptionAttribute()
    {
        var registry = new LocalTextRegistry();
        EnumLocalTextRegistration.AddEnumTexts(registry, new MockTypeSource(typeof(EnumWithKey), typeof(EnumWithoutKey)));

        string unexpectedKey1 = "Enums.My.CoolEnumKey." +
            EnumWithKey.NoDescriptionKey.GetName();

        string unexpectedKey2 = "Enums." + typeof(EnumWithoutKey).FullName + "." +
            EnumWithoutKey.NoDescriptionNoKey.GetName();

        Assert.Null(registry.TryGet(LocalText.InvariantLanguageID, unexpectedKey1, pending: false));
        Assert.Null(registry.TryGet(LocalText.InvariantLanguageID, unexpectedKey2, pending: false));
    }

    [Fact]
    public void AddEnumTexts_Uses_InvariantLanguageId_ByDefault()
    {
        var registry = new LocalTextRegistry();
        EnumLocalTextRegistration.AddEnumTexts(registry, new MockTypeSource(typeof(EnumWithKey)));
        string expectedKey = "Enums.My.CoolEnumKey." + EnumWithKey.WithDescriptionKey.GetName();
        Assert.Equal("Description for WithDescriptionKey", registry.TryGet(LocalText.InvariantLanguageID, expectedKey, pending: false));
    }

    [Fact]
    public void AdEnumTexts_UsesLanguageSpecified()
    {
        var registry = new LocalTextRegistry();
        EnumLocalTextRegistration.AddEnumTexts(registry, new MockTypeSource(typeof(EnumWithKey)), languageID: "es");
        string expectedKey = "Enums.My.CoolEnumKey." + EnumWithKey.WithDescriptionKey.GetName();
        Assert.Null(registry.TryGet(LocalText.InvariantLanguageID, expectedKey, pending: false));
        Assert.Equal("Description for WithDescriptionKey", registry.TryGet("es", expectedKey, pending: false));
    }
}