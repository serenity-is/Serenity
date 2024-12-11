namespace Serenity.Web;

public class LocalTextPackagesTests
{
    [Fact]
    public void Constructor_AddsDefaultItemForSite()
    {
        var packages = new LocalTextPackages();
        Assert.True(packages.ContainsKey("Site"));
        Assert.Equal("", packages["Site"]);
    }

    [Fact]
    public void DefaultSitePackageIncludes_IncludesCommonPrefixes()
    {
        var regex = LocalTextPackages.DefaultSitePackageIncludes;
        Assert.Matches(regex.ToString(), "Columns.Test");
        Assert.Matches(regex.ToString(), "Db.Test");
        Assert.Matches(regex.ToString(), "Dialogs.Test");
        Assert.Matches(regex.ToString(), "Enums.Test");
        Assert.Matches(regex.ToString(), "Forms.Test");
        Assert.Matches(regex.ToString(), "Permission.Test");
        Assert.Matches(regex.ToString(), "Site.Test");
        Assert.Matches(regex.ToString(), "Validation.Test");
    }

    [Fact]
    public void DefaultSitePackageIncludes_DoesNotMatchContains()
    {
        var regex = LocalTextPackages.DefaultSitePackageIncludes;
        Assert.DoesNotMatch(regex.ToString(), "Some.Columns.Test");
        Assert.DoesNotMatch(regex.ToString(), "Some.Db.Test");
        Assert.DoesNotMatch(regex.ToString(), "Some.Dialogs.Test");
        Assert.DoesNotMatch(regex.ToString(), "Some.Enums.Test");
        Assert.DoesNotMatch(regex.ToString(), "Some.Forms.Test");
        Assert.DoesNotMatch(regex.ToString(), "Some.Permission.Test");
        Assert.DoesNotMatch(regex.ToString(), "Some.Site.Test");
        Assert.DoesNotMatch(regex.ToString(), "Some.Validation.Test");
    }

    [Fact]
    public void DefaultSitePackageIncludes_DoesNotMatchEndsWith()
    {
        var regex = LocalTextPackages.DefaultSitePackageIncludes;
        Assert.DoesNotMatch(regex.ToString(), "Some.Columns.");
        Assert.DoesNotMatch(regex.ToString(), "Some.Db.");
        Assert.DoesNotMatch(regex.ToString(), "Some.Dialogs.");
        Assert.DoesNotMatch(regex.ToString(), "Some.Enums.");
        Assert.DoesNotMatch(regex.ToString(), "Some.Forms.");
        Assert.DoesNotMatch(regex.ToString(), "Some.Permission.");
        Assert.DoesNotMatch(regex.ToString(), "Some.Site.");
        Assert.DoesNotMatch(regex.ToString(), "Some.Validation.");
    }
}

