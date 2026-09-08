using System.Text.Json;

namespace Serenity.ComponentModel;

public class EditorAddonItemTests
{
    [Fact]
    public void Type_GetSet_Works()
    {
        var item = new EditorAddonItem { Type = "MyAddon" };
        Assert.Equal("MyAddon", item.Type);
    }

    [Fact]
    public void Params_GetSet_Works()
    {
        var item = new EditorAddonItem { Params = new Dictionary<string, object?> { ["key"] = "value" } };
        Assert.Equal("value", item.Params["key"]);
    }

    [Fact]
    public void Serializes_WithTypeAndParams()
    {
        var item = new EditorAddonItem { Type = "MyAddon", Params = new Dictionary<string, object?> { ["key"] = "value" } };
        var json = JsonSerializer.Serialize(item);
        Assert.Contains("MyAddon", json);
        Assert.Contains("key", json);
    }
}