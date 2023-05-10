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
        var registry = new MockLocalTextRegistry();
        EnumLocalTextRegistration.AddEnumTexts(registry, new MockTypeSource(typeof(EnumWithoutKey)));

        string expectedKey = "Enums." + typeof(EnumWithoutKey).FullName + "." +
            EnumWithoutKey.WithDescriptionNoKey.GetName();

        Assert.Collection(registry.AddedList,
            item1 => Assert.Equal((LocalText.InvariantLanguageID, expectedKey, "Description for WithDescriptionNoKey"), item1));
    }

    [Fact]
    public void AddEnumTexts_UsesKey_IfKeyAttributeIsPresent()
    {
        var registry = new MockLocalTextRegistry();
        EnumLocalTextRegistration.AddEnumTexts(registry, new MockTypeSource(typeof(EnumWithKey)));

        string expectedKey = "Enums.My.CoolEnumKey." + EnumWithKey.WithDescriptionKey.GetName();

        Assert.Collection(registry.AddedList,
            item1 => Assert.Equal((LocalText.InvariantLanguageID, expectedKey, "Description for WithDescriptionKey"), item1));
    }

    [Fact]
    public void AddEnumTexts_Skips_EnumValues_Without_DescriptionAttribute()
    {
        var registry = new MockLocalTextRegistry();
        EnumLocalTextRegistration.AddEnumTexts(registry, new MockTypeSource(typeof(EnumWithKey), typeof(EnumWithoutKey)));

        string unexpectedKey1 = "Enums.My.CoolEnumKey." +
            EnumWithKey.NoDescriptionKey.GetName();

        string unexpectedKey2 = "Enums." + typeof(EnumWithoutKey).FullName + "." +
            EnumWithoutKey.NoDescriptionNoKey.GetName();

        Assert.Collection(registry.AddedList,
            item1 => Assert.Equal("Description for WithDescriptionKey", item1.text),
            item2 => Assert.Equal("Description for WithDescriptionNoKey", item2.text));
    }

    [Fact]
    public void AddEnumTexts_Uses_InvariantLanguageId_ByDefault()
    {
        var registry = new MockLocalTextRegistry();
        EnumLocalTextRegistration.AddEnumTexts(registry, new MockTypeSource(typeof(EnumWithKey)));
        Assert.NotEmpty(registry.AddedList);
        Assert.All(registry.AddedList, x => Assert.Equal(LocalText.InvariantLanguageID, x.languageID));
    }

    [Fact]
    public void AdEnumTexts_UsesLanguageSpecified()
    {
        var registry = new MockLocalTextRegistry();
        EnumLocalTextRegistration.AddEnumTexts(registry, new MockTypeSource(typeof(EnumWithKey)), languageID: "es");
        Assert.NotEmpty(registry.AddedList);
        Assert.All(registry.AddedList, x => Assert.Equal("es", x.languageID));
    }
}