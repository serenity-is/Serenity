namespace Serenity.ComponentModel;

public class EditorAddonAttributeTests
{
    private class TestAddonAttribute(string type) : EditorAddonAttribute(type)
    {
        public void SetTestOption(string key, object? value) => SetOption(key, value);
        public TType GetTestOption<TType>(string key) => GetOption<TType>(key);
    }

    [Fact]
    public void Ctor_ThrowsArgumentNullException_WhenTypeIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new EditorAddonAttribute(null));
    }

    [Fact]
    public void AddonType_ReturnsType()
    {
        var attr = new EditorAddonAttribute("MyAddon");
        Assert.Equal("MyAddon", attr.AddonType);
    }

    [Fact]
    public void SetParams_ThrowsArgumentNullException_WhenParamsIsNull()
    {
        var attr = new EditorAddonAttribute("MyAddon");
        Assert.Throws<ArgumentNullException>(() => attr.SetParams(null));
    }

    [Fact]
    public void SetParams_SetsOptions()
    {
        var attr = new TestAddonAttribute("MyAddon");
        attr.SetTestOption("key", "value");

        var dict = new Dictionary<string, object?>();
        attr.SetParams(dict);

        Assert.Equal("value", dict["key"]);
    }

    [Fact]
    public void SetParams_WithNoOptions_DoesNothing()
    {
        var attr = new EditorAddonAttribute("MyAddon");
        var dict = new Dictionary<string, object?>();
        attr.SetParams(dict);
        Assert.Empty(dict);
    }

    [Fact]
    public void GetOption_ReturnsDefault_WhenNotSet()
    {
        var attr = new TestAddonAttribute("MyAddon");
        Assert.Null(attr.GetTestOption<string>("missing"));
    }

    [Fact]
    public void IsLocalizableOption_ReturnsTrue_ForTextAndHint()
    {
        var attr = new EditorAddonAttribute("MyAddon");
        Assert.True(attr.IsLocalizableOption("text"));
        Assert.True(attr.IsLocalizableOption("hint"));
    }

    [Fact]
    public void IsLocalizableOption_ReturnsFalse_ForOtherKeys()
    {
        var attr = new EditorAddonAttribute("MyAddon");
        Assert.False(attr.IsLocalizableOption("other"));
    }

    [Fact]
    public void GetOption_ReturnsDefault_WhenValueIsNull()
    {
        var attr = new TestAddonAttribute("MyAddon");
        attr.SetTestOption("key", null);
        Assert.Null(attr.GetTestOption<string>("key"));
    }

    [Fact]
    public void GetOption_ReturnsValue_WhenSet()
    {
        var attr = new TestAddonAttribute("MyAddon");
        attr.SetTestOption("key", "value");
        Assert.Equal("value", attr.GetTestOption<string>("key"));
    }

    [Fact]
    public void AddonKey_GetSet_Works()
    {
        var attr = new EditorAddonAttribute("MyAddon") { AddonKey = "addon-key" };
        Assert.Equal("addon-key", attr.AddonKey);
    }
}