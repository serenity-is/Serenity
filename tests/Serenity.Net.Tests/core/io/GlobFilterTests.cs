namespace Serenity.IO;

public class GlobFilterTests
{
    static readonly string[] filters =
    [
        ".git/",
        "/web.config",
        "/Web.Debug.config",
        "/wkhtmltopdf-license.txt",
        "/Welcome.htm",
        "/Readme.CodeGenerator.txt",
        "/package.json",
        "/*.conf",
        "/bin/*.xml",
        "/Imports/",
        "TestResults/",
        "*.cs",
        "App_Data/**/*.log",
        "/App_Code/**/*.xyz",
        "node_modules/",
        ".csslintrc",
        "_references.js",
        "*.csproj.user",
        "*.dsk",
        "*.suo",
        "*-vsdoc.js",
        "/*-dat.js",
        "Modules/**/*",
    ];

    [Theory]
    [InlineData(@".git/", true)]
    [InlineData(@"a.git/", false)]
    [InlineData(@"d/e/.git/", true)]
    [InlineData(@"d/e/.git/obj/abc", true)]
    [InlineData(@"d/e/a.git/", false)]
    [InlineData(@"Web.config", true)]
    [InlineData(@"WEB.CONFIG", true)]
    [InlineData(@"views/WEB.CONFIG", false)]
    [InlineData(@"abc/def/WEB.CONFIG", false)]
    [InlineData(@"a/b/c/WEB.CONFIG", false)]
    [InlineData(@"Web.debug.config", true)]
    [InlineData(@"WEB.debug.CONFIG", true)]
    [InlineData(@"views/WEB.debug.CONFIG", false)]
    [InlineData(@"abc/def/WEB.debug.CONFIG", false)]
    [InlineData(@"a/b/c/WEB.debug.CONFIG", false)]
    [InlineData(@"wkhtmltopdf-license.txt", true)]
    [InlineData(@"somewkhtmltopdf-license.txt", false)]
    [InlineData(@"sub/wkhtmltopdf-license.txt", false)]
    [InlineData(@"Readme.CodeGenerator.txt", true)]
    [InlineData(@"someReadme.CodeGenerator.txt", false)]
    [InlineData(@"sub/Readme.CodeGenerator.txt", false)]
    [InlineData(@"test.suo", true)]
    [InlineData(@"abcd/test.suo", true)]
    [InlineData(@"abc/def/test.suo", true)]
    [InlineData(@".dsk", true)]
    [InlineData(@"test.dsk", true)]
    [InlineData(@"a/test.dsk", true)]
    [InlineData(@"a/.dsk", true)]
    [InlineData(@"test.dsk/x.t", false)]
    [InlineData(@"some.dsk.txt", false)]
    [InlineData(@"dummy.csproj.user", true)]
    [InlineData(@"abc/d.csproj.user", true)]
    [InlineData(@"abc/def/d.csproj.user", true)]
    [InlineData(@"abc/d.csproj.user/e.txt", false)]
    [InlineData(@"package.json", true)]
    [InlineData(@"test//package.json", false)]
    [InlineData(@"abc.conf", true)]
    [InlineData(@".conf", true)]
    [InlineData(@"test/abc.conf", false)]
    [InlineData(@"test/.conf", false)]
    [InlineData(@"bin/test.xml", true)]
    [InlineData(@"bin/.xml", true)]
    [InlineData(@"test.xml", false)]
    [InlineData(@"bin/subfolder/.xml", false)]
    [InlineData(@"bin/subfolder/test.xml", false)]
    [InlineData(@"some/folder/test.xml", false)]
    [InlineData(@"Imports/test.txt", true)]
    [InlineData(@"Imports/SomeSub/test.txt", true)]
    [InlineData(@"SomeSub/Imports/test.txt", false)]
    [InlineData(@"SomeSub/Imports/abc/test.txt", false)]
    [InlineData(@"Imports", false)]
    [InlineData(@"TestResults/test.txt", true)]
    [InlineData(@"TestResults/SomeSub/test.txt", true)]
    [InlineData(@"SomeSub/TestResults/test.txt", true)]
    [InlineData(@"SomeSub/TestResults/abc/test.txt", true)]
    [InlineData(@"TestResults", false)]
    [InlineData(@"test.cs", true)]
    [InlineData(@"abc/def/test.cs", true)]
    [InlineData(@"test.cs/dummy.txt", false)]
    [InlineData(@"abc/test.csx", false)]
    [InlineData(@"abc/def/test.csx", false)]
    [InlineData(@"App_Code/test.xyz", true)]
    [InlineData(@"App_Code/x/test.xyz", true)]
    [InlineData(@"abc/App_Code/x/test.xyz", false)]
    [InlineData(@"App_Data/test.log", true)]
    [InlineData(@"App_Data/abc/def.log", true)]
    [InlineData(@"App_Data/abc/def/xyz.log", true)]
    [InlineData(@"App_Dat/xyz.log", false)]
    [InlineData(@"App_Data.log", false)]
    [InlineData(@"xyz/App_Data/abc/def.log", true)]
    [InlineData(@"xyz/App_Data.log", false)]
    [InlineData(@"xyz/test.log", false)]
    [InlineData(@"test.xyz", false)]
    [InlineData(@"node_modules/", true)]
    [InlineData(@"a.node_modules/", false)]
    [InlineData(@"d/e/node_modules/", true)]
    [InlineData(@"d/e/node_modules/obj/abc", true)]
    [InlineData(@"d/e/a.node_modules/", false)]
    [InlineData(@".csslintrc", true)]
    [InlineData(@"x/.csslintrc", true)]
    [InlineData(@".csslintrc/x.txt", false)]
    [InlineData(@"_references.js", true)]
    [InlineData(@"x/_references.js", true)]
    [InlineData(@"_references.js/x.txt", false)]
    [InlineData(@"test-vsdoc.js", true)]
    [InlineData(@"abc/xyz-vsdoc.js", true)]
    [InlineData(@"-vsdoc.js", true)]
    [InlineData(@"-vsdoc.test.js", false)]
    [InlineData(@"-vsdoc.xyz", false)]
    [InlineData(@"test-dat.js", true)]
    [InlineData(@"abc/xyz-dat.js", false)]
    [InlineData(@"-dat.js", true)]
    [InlineData(@"-dat.test.js", false)]
    [InlineData(@"-dat.xyz", false)]
    [InlineData(@"Modules/test.ts", true)]
    [InlineData(@"Modules/x/test.tsx", true)]
    [InlineData(@"sub_modules/Modules/x/test.tsx", true)]
    [InlineData(@"sub_modules/Modules/test.tsx", true)]
    [InlineData(@"sub_modules/Module/x/test.tsx", false)]
    public void Glob_Filter_Works_Properly(string path, bool result)
    {
        var filter = new GlobFilter(filters);
        Assert.Equal(result, filter.IsMatch(path));
    }

    [Theory]
    [InlineData("x.css", true)]
    [InlineData("a/x.css", true)]
    [InlineData("a/b/c/x.css", true)]
    [InlineData("/a/x.css", true)]
    [InlineData("a//x.css", true)]
    [InlineData("ax.css", false)]
    [InlineData("a/ax.css", false)]
    [InlineData("a/x.css/b", false)]
    [InlineData("/x.css", false)]
    public void Recursive_Wildcard_Matches_Any_Number_Of_Folders(string path, bool result)
    {
        var filter = new GlobFilter(["**/x.css"]);
        Assert.Equal(result, filter.IsMatch(path));
    }

    [Theory]
    [InlineData("a/z/x.css", true)]
    [InlineData("a/b/z/c/d/x.css", true)]
    [InlineData("a/z/x.cs", false)]
    [InlineData("a/b/c/x.css", false)]
    public void Multiple_Recursive_Wildcards_Match_Any_Number_Of_Folders(string path, bool result)
    {
        var filter = new GlobFilter(["a/**/z/**/x.css"]);
        Assert.Equal(result, filter.IsMatch(path));
    }

    /// <summary>
    /// The pattern "**/" was converted to "(.+/)*", which has exponentially many
    /// ways to split the folders of a path, so a path that does not match took
    /// a second for 26 folders and 16 seconds for 30 of them.
    /// </summary>
    [Fact]
    public void Recursive_Wildcard_Is_Matched_In_Polynomial_Time_For_Deep_Paths()
    {
        var filter = new GlobFilter(["**/x.css", "a/**/a/**/x.css"]);
        var path = string.Concat(Enumerable.Repeat("a/", 60)) + "y.css";

        var sw = System.Diagnostics.Stopwatch.StartNew();
        var actual = filter.IsMatch(path);
        sw.Stop();

        Assert.False(actual);
        Assert.True(sw.Elapsed < TimeSpan.FromSeconds(3),
            $"Matching a path with 60 folders took {sw.Elapsed}.");
    }
}