namespace Serenity.Localization;

public class EnumLocalTextRegistrationTests
{    [EnumKey("My.CoolEnumKey")]
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

    [EnumKey("My.TextKeyEnum")]
    public enum EnumWithTextKeyDescriptions
    {
        [DescriptionText("Invariant description", TextKey = "Site.EnumDescription")]
        ExplicitTextKey,
        [DescriptionText("Site.LiteralDescription")]
        KeyShapedLiteral,
        [Description("Site.LegacyDescription")]
        LegacyKeyCandidate
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

        Assert.Equal((LocalText.InvariantLanguageID, expectedKey, "Description for WithDescriptionNoKey"), Assert.Single(registry.AddedList));
    }

    [Fact]
    public void AddEnumTexts_UsesKey_IfKeyAttributeIsPresent()
    {
        var registry = new MockLocalTextRegistry();
        EnumLocalTextRegistration.AddEnumTexts(registry, new MockTypeSource(typeof(EnumWithKey)));

        string expectedKey = "Enums.My.CoolEnumKey." + EnumWithKey.WithDescriptionKey.GetName();

        Assert.Equal((LocalText.InvariantLanguageID, expectedKey, "Description for WithDescriptionKey"),
            Assert.Single(registry.AddedList));
    }

    [Fact]
    public void AddEnumTexts_Uses_Explicit_DescriptionText_Key()
    {
        var registry = new MockLocalTextRegistry();
        EnumLocalTextRegistration.AddEnumTexts(registry,
            new MockTypeSource(typeof(EnumWithTextKeyDescriptions)));

        Assert.Collection(registry.AddedList,
            item => Assert.Equal((LocalText.InvariantLanguageID, "Site.EnumDescription", "Invariant description"), item),
            item => Assert.Equal((LocalText.InvariantLanguageID,
                "Enums.My.TextKeyEnum.KeyShapedLiteral", "Site.LiteralDescription"), item),
            item => Assert.Equal((LocalText.InvariantLanguageID,
                "Site.LegacyDescription", null), item));
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

    [Fact]
    public void AddEnumTexts_Adds_StringEnum_Members_With_Enums_Key()
    {
        var registry = new MockLocalTextRegistry();
        EnumLocalTextRegistration.AddEnumTexts(registry, new MockTypeSource(typeof(WidgetCategory)));

        Assert.Collection(registry.AddedList,
            item => Assert.Equal((LocalText.InvariantLanguageID,
                "Enums.My.WidgetCategory.Charts", "Charts"), item),
            item => Assert.Equal((LocalText.InvariantLanguageID,
                "Site.Outlines", "Outlines"), item));
    }

    [Fact]
    public void AddEnumTexts_Uses_FullName_For_StringEnum_Without_EnumKey()
    {
        var registry = new MockLocalTextRegistry();
        EnumLocalTextRegistration.AddEnumTexts(registry, new MockTypeSource(typeof(EnumKeyLessStringEnum)));

        Assert.Equal((LocalText.InvariantLanguageID,
            "Enums." + typeof(EnumKeyLessStringEnum).FullName + ".FirstMember", "First"),
            Assert.Single(registry.AddedList));
    }

    [Fact]
    public void AddEnumTexts_Skips_StringEnum_Members_Without_Text()
    {
        var registry = new MockLocalTextRegistry();
        EnumLocalTextRegistration.AddEnumTexts(registry, new MockTypeSource(typeof(WidgetCategory)));

        Assert.DoesNotContain(registry.AddedList, x => x.key.EndsWith(".Tables", StringComparison.Ordinal));
        Assert.DoesNotContain(registry.AddedList, x => x.key.EndsWith(".Internal", StringComparison.Ordinal));
    }

    [Fact]
    public void AddEnumTexts_Throws_For_Key_Collision_Between_Enum_And_StringEnum()
    {
        var registry = new MockLocalTextRegistry();

        var ex = Assert.Throws<InvalidOperationException>(() => EnumLocalTextRegistration.AddEnumTexts(registry,
            new MockTypeSource(typeof(WidgetCategory), typeof(CollidingWidgetCategory))));

        Assert.Contains(typeof(CollidingWidgetCategory).FullName!, ex.Message);
        Assert.Contains(typeof(WidgetCategory).FullName!, ex.Message);
    }

    [EnumKey("My.WidgetCategory")]
    public enum CollidingWidgetCategory
    {
        [Description("Colliding charts")]
        Charts,
        [Description("Unrelated")]
        Other
    }

    public abstract class EnumKeyLessStringEnum : StringEnum
    {
        [Description("First")]
        public const string FirstMember = "first";
        public const string SecondMember = "second";
    }
}