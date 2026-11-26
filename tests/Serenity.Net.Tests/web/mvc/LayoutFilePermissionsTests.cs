namespace Serenity.Web;

public class LayoutFilePermissionsTests
{
    [Theory]
    [InlineData("*", true)]
    [InlineData(" * ", true)]
    [InlineData("?", false)]
    [InlineData("A", false)]
    [InlineData(null, false)]
    [InlineData("", false)]
    public void IsPublic_Only_For_Asterisk(string? permission, bool expected)
    {
        Assert.Equal(expected, LayoutFilePermissions.IsPublic(permission));
    }

    [Fact]
    public void IsGranted_Denies_Null_Or_Empty_And_Checks_Others()
    {
        var permissions = new MockPermissions(p => p == "A");
        Assert.True(LayoutFilePermissions.IsGranted(permissions, "*"));
        Assert.True(LayoutFilePermissions.IsGranted(permissions, "A"));
        Assert.False(LayoutFilePermissions.IsGranted(permissions, "B"));
        Assert.False(LayoutFilePermissions.IsGranted(permissions, null));
        Assert.False(LayoutFilePermissions.IsGranted(permissions, ""));
        // without a permission service, only public files
        Assert.True(LayoutFilePermissions.IsGranted(null, "*"));
        Assert.False(LayoutFilePermissions.IsGranted(null, "A"));
    }

    [Fact]
    public void ByPath_Public_Wins_Otherwise_Joins_With_Or()
    {
        var result = LayoutFilePermissions.ByPath([
            ("~/a.js", "A"), ("~/A.js", "*"),
            ("~/b.js", "B"), ("~/b.js", "C"),
            ("~/c.js", null),
            ("  ", "X")
        ]);

        Assert.Equal("*", result["~/a.js"]);
        Assert.Equal("B|C", result["~/b.js"]);
        Assert.Equal("DENY", result["~/c.js"]);
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void Attribute_Defaults()
    {
        Assert.Equal("?", new LayoutScriptFileAttribute("~/a.js").Permission);
        Assert.Equal("?", new LayoutScriptFileAttribute("~/a.js") { Permission = "" }.Permission);
        Assert.Equal("*", new LayoutScriptFileAttribute("~/a.js") { Permission = "*" }.Permission);
        Assert.Equal("*", new LayoutCssFileAttribute("~/a.css").Permission);
        Assert.Equal("A", new LayoutCssFileAttribute("~/a.css") { Permission = "A" }.Permission);
    }
}
