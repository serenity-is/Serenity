namespace Serenity.Localization;

public partial class PropertyItemsLocalTextRegistrationTests
{
    private class TestAddonAttribute : EditorAddonAttribute
    {
        public TestAddonAttribute(string text) : base("TestAddon")
        {
            SetOption("text", text);
        }
    }

    [FormScript]
    private class TypeWithAddon
    {
        [TestAddon("Db.SomeEntity.SomeField")]
        public string Name { get; set; }
    }

    [ColumnsScript("MyColumns")]
    private class ColumnsWithoutPrefix
    {
        [DisplayName("Some Display")]
        public string Name { get; set; }
    }

    [Fact]
    public void AddPropertyItemsTexts_ThrowsArgumentNull_If_TypeSource_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            PropertyItemsLocalTextRegistration.AddPropertyItemsTexts(new LocalTextRegistry(), null));
    }

    [Fact]
    public void AddPropertyItemsTexts_ThrowsArgumentNull_If_Registry_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            PropertyItemsLocalTextRegistration.AddPropertyItemsTexts(null, new MockTypeSource()));
    }

    [Fact]
    public void Registers_EditorAddon_Localizable_Text()
    {
        var registry = new LocalTextRegistry();
        var typeSource = new MockTypeSource(typeof(TypeWithAddon));

        PropertyItemsLocalTextRegistration.AddPropertyItemsTexts(registry, typeSource);

        var texts = registry.GetAllTexts(false);
        Assert.Contains(texts, x => x.Key.Key == "Db.SomeEntity.SomeField" && x.Value == null);
    }

    [Fact]
    public void GetPropertyItemsTextPrefix_ForColumns_WithoutPrefix_UsesColumnsPrefix()
    {
        var prefix = PropertyItemsLocalTextRegistration.GetPropertyItemsTextPrefix(typeof(ColumnsWithoutPrefix));
        Assert.Equal("Columns.MyColumns.", prefix);
    }

    [Fact]
    public void IsLocalTextKeyCandidate_ReturnsExpected()
    {
        Assert.True(PropertyItemsLocalTextRegistration.IsLocalTextKeyCandidate("Db.SomeEntity.SomeField"));
        Assert.False(PropertyItemsLocalTextRegistration.IsLocalTextKeyCandidate("not a key"));
        Assert.False(PropertyItemsLocalTextRegistration.IsLocalTextKeyCandidate(""));
        Assert.False(PropertyItemsLocalTextRegistration.IsLocalTextKeyCandidate(null));
    }
}