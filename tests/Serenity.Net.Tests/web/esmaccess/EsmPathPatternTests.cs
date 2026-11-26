namespace Serenity.Web;

public class EsmPathPatternTests
{
    [Theory]
    [InlineData("Modules/Membership/**", "Modules/Membership/Account/LoginPage.js", true)]
    [InlineData("Modules/Membership/**", "Modules/Membership.js", false)]
    [InlineData("Modules/**/LoginPage.js", "Modules/LoginPage.js", true)]
    [InlineData("Modules/**/LoginPage.js", "Modules/A/B/LoginPage.js", true)]
    [InlineData("Modules/*/LoginPage.js", "Modules/A/B/LoginPage.js", false)]
    [InlineData("Modules/*/LoginPage.js", "Modules/A/LoginPage.js", true)]
    [InlineData("bundles/**", "bundles/script.bundle.js", true)]
    [InlineData("Modules/Common/ScriptInit.js", "modules/common/scriptinit.js", true)]
    [InlineData("Modules/?.js", "Modules/A.js", true)]
    [InlineData("Modules/?.js", "Modules/AB.js", false)]
    [InlineData("Modules/A.js", "Modules/A.js.map", false)]
    public void Matches_Relative_Path(string pattern, string path, bool expected)
    {
        Assert.Equal(expected, new EsmPathPattern(pattern).IsMatch(path, "/esm/" + path));
    }

    [Fact]
    public void Pattern_Starting_With_Slash_Matches_Full_Path()
    {
        var pattern = new EsmPathPattern("/Serenity.Extensions/esm/Modules/**");
        Assert.True(pattern.IsFullPath);
        Assert.True(pattern.IsMatch("Modules/A.js", "/Serenity.Extensions/esm/Modules/A.js"));
        Assert.False(pattern.IsMatch("Modules/A.js", "/esm/Modules/A.js"));
    }

    [Fact]
    public void ParseList_Splits_By_Semicolon_And_Skips_Empty()
    {
        var list = EsmPathPattern.ParseList(" a/** ; ;b.js;");
        Assert.Equal(["a/**", "b.js"], list.Select(x => x.Pattern));
        Assert.Empty(EsmPathPattern.ParseList(null));
        Assert.Empty(EsmPathPattern.ParseList(" "));
    }
}
