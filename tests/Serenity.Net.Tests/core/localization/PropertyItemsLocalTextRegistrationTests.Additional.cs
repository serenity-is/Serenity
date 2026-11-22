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

    [FormScript("MyForm")]
    private class TypeWithLiteralTexts
    {
        [DisplayText("Some display", TextKey = LocalText.Literal)]
        [Placeholder("Choose", TextKey = LocalText.Literal)]
        [Hint("A hint", TextKey = LocalText.Literal)]
        public string? Name { get; set; }

        [DisplayText("Other display")]
        public string? Other { get; set; }
    }

    [Fact]
    public void Registers_Nothing_For_A_Text_With_An_Empty_Key()
    {
        var registry = new LocalTextRegistry();
        PropertyItemsLocalTextRegistration.AddPropertyItemsTexts(registry, new MockTypeSource(typeof(TypeWithLiteralTexts)));

        var texts = registry.GetAllTexts(false).Select(x => x.Key.Key).ToList();

        // a text with an empty key is shown as it is: neither its own key nor the implied one is registered
        Assert.DoesNotContain("Forms.MyForm.Name", texts);
        Assert.DoesNotContain("Forms.MyForm.Name_Placeholder", texts);
        Assert.DoesNotContain("Forms.MyForm.Name_Hint", texts);
        // a text without a key takes the one that its form implies
        Assert.Contains("Forms.MyForm.Other", texts);
    }

    [FormScript("MyForm")]
    private class TypeWithSelectItems
    {
        [SelectItem("low", Text = "Low")]
        [SelectItem("high", Text = "High", TextKey = "Controls.Priority.High")]
        [SelectItem("none")]
        [SelectItem("acme", Text = "Acme Corporation", TextKey = LocalText.Literal)]
        [SelectItem("exotic", TextKey = "Controls.Supplier.Exotic")]
        public string? Priority { get; set; }
    }

    [Fact]
    public void Registers_SelectItem_Texts_With_Their_Implied_Keys()
    {
        var registry = new LocalTextRegistry();
        PropertyItemsLocalTextRegistration.AddPropertyItemsTexts(registry, new MockTypeSource(typeof(TypeWithSelectItems)));

        var texts = registry.GetAllTexts(false)
            .ToDictionary(x => x.Key.Key, x => x.Value);

        // an item with a text, and no key of its own, takes the key that its form implies for it
        Assert.Equal("Low", texts["Forms.MyForm.Priority_SelectItem_low"]);
        // an item with a key of its own is registered with that key
        Assert.Equal("High", texts["Controls.Priority.High"]);
        // an item without a text has nothing to translate, as its value is its text
        Assert.False(texts.ContainsKey("Forms.MyForm.Priority_SelectItem_none"));
        // an item with an empty key is left as it is, e.g. one whose text is a name
        Assert.False(texts.ContainsKey("Forms.MyForm.Priority_SelectItem_acme"));
        // an item with a key of its own is registered with it, even when it has no text yet
        Assert.True(texts.ContainsKey("Controls.Supplier.Exotic"));
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