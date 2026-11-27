using Microsoft.Extensions.Configuration;

namespace Serenity.Extensions;

public class PermissionGrantOptionsTests
{
    [Theory]
    [InlineData("*", "Northwind:General", true)]
    [InlineData("Northwind:*", "Northwind:Customer:Delete", true)]
    [InlineData("Northwind:*", "Meeting:General", false)]
    [InlineData("*:Delete", "Northwind:Customer:Delete", true)]
    [InlineData("*:Delete", "Northwind:Customer:Modify", false)]
    [InlineData("administration:security", "Administration:Security", true)]
    [InlineData("Administration:Security", "Administration:SecurityX", false)]
    [InlineData("N*d:*l", "Northwind:General", true)]
    public void PermissionPattern_Matches_Stars_Ignoring_Case(string pattern, string key, bool expected)
    {
        Assert.Equal(expected, PermissionPattern.IsMatch(pattern, key));
    }

    [Fact]
    public void Without_Settings_Everything_Is_Grantable()
    {
        var options = new PermissionGrantOptions();
        Assert.Equal(PermissionGrantTier.Grantable, options.GetTier("Administration:Security"));
    }

    [Fact]
    public void The_Most_Restrictive_Matching_Tier_Wins_And_A_Key_Matching_Nothing_Is_HolderOnly()
    {
        var options = new PermissionGrantOptions
        {
            Grantable = "Northwind:*",
            HolderOnly = "*:Delete",
            SuperAdminOnly = "Administration:Security"
        };

        Assert.Equal(PermissionGrantTier.Grantable, options.GetTier("Northwind:General"));
        Assert.Equal(PermissionGrantTier.HolderOnly, options.GetTier("Northwind:Customer:Delete"));
        Assert.Equal(PermissionGrantTier.SuperAdminOnly, options.GetTier("Administration:Security"));
        Assert.Equal(PermissionGrantTier.HolderOnly, options.GetTier("Meeting:General"));
    }

    [Fact]
    public void An_Empty_Grantable_List_Means_None()
    {
        var options = new PermissionGrantOptions { Grantable = "" };
        Assert.Equal(PermissionGrantTier.HolderOnly, options.GetTier("Northwind:General"));
    }

    [Fact]
    public void Lists_Are_Separated_By_Commas_Semicolons_Or_White_Space()
    {
        var options = new PermissionGrantOptions { SuperAdminOnly = "Administration:Security,Administration:Credentials;\nAudit:* Logs:*" };

        Assert.Equal(PermissionGrantTier.SuperAdminOnly, options.GetTier("Administration:Credentials"));
        Assert.Equal(PermissionGrantTier.SuperAdminOnly, options.GetTier("Audit:View"));
        Assert.Equal(PermissionGrantTier.SuperAdminOnly, options.GetTier("Logs:Read"));
        Assert.Equal(PermissionGrantTier.Grantable, options.GetTier("Administration:Translation"));
    }

    [Fact]
    public void A_Later_Settings_File_Or_Environment_Variable_Replaces_A_List_As_A_Whole()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonStream(Stream("""{ "PermissionGrants": { "HolderOnly": "Administration:*", "SuperAdminOnly": "Administration:Security, Administration:Credentials" } }"""))
            .AddInMemoryCollection(new Dictionary<string, string?> { ["PermissionGrants:SuperAdminOnly"] = "Administration:Security" })
            .Build();

        var options = configuration.GetSection(PermissionGrantOptions.SectionKey).Get<PermissionGrantOptions>()!;

        Assert.Equal(PermissionGrantTier.SuperAdminOnly, options.GetTier("Administration:Security"));
        Assert.Equal(PermissionGrantTier.HolderOnly, options.GetTier("Administration:Credentials"));
        Assert.Equal(PermissionGrantTier.Grantable, options.GetTier("Northwind:General"));
    }

    private static System.IO.MemoryStream Stream(string json) => new(System.Text.Encoding.UTF8.GetBytes(json));
}
