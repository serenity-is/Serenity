using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Serenity.Web;
using System.IO;
using System.Text.Json;

namespace Serenity.CodeGenerator;

public class WriteStaticAssetsManifestTaskTests : IDisposable
{
    private readonly string root;
    private readonly string manifest;

    public WriteStaticAssetsManifestTaskTests()
    {
        root = Path.Combine(Path.GetTempPath(), "wsamt-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        manifest = Path.Combine(root, "sub", "App.serenitystaticassets.json");
    }

    public void Dispose()
    {
        try
        {
            Directory.Delete(root, true);
        }
        catch
        {
        }
    }

    // like the default exclude properties evaluate to, with some patterns that can't match under wwwroot
    private const string DefaultExcludes = @"bin/**;obj/**;P:\x\artifacts/**;node_modules\**;**/*.user;**/*.*proj;" +
        @"**\node_modules\**;**/.*/**;wwwroot\**\.*;wwwroot\**\*.tmp";

    private const string DefaultWellKnownExcludes = @"bin/**;obj/**;**/*.user;**/*.*proj;**\node_modules\**";

    private string Folder(string name) => Path.Combine(root, name, "wwwroot") + Path.DirectorySeparatorChar;

    private static ITaskItem Root(string path, string basePath, params (string name, string value)[] metadata)
    {
        var item = new TaskItem(path);
        item.SetMetadata("BasePath", basePath);
        foreach (var (name, value) in metadata)
            item.SetMetadata(name, value);
        return item;
    }

    private ITaskItem ProjectRoot(string name, string excludes = DefaultExcludes, string includes = @"wwwroot\**",
        string wellKnown = "true", string wellKnownExcludes = DefaultWellKnownExcludes)
    {
        return Root(Folder(name), name, ("Includes", includes), ("Excludes", excludes),
            ("WellKnown", wellKnown), ("WellKnownExcludes", wellKnownExcludes));
    }

    private void Write(params ITaskItem[] roots)
    {
        Assert.True(new WriteStaticAssetsManifestTask { Roots = roots, File = manifest }.Execute());
    }

    private JsonElement Json() => JsonDocument.Parse(File.ReadAllText(manifest)).RootElement;

    [Fact]
    public void Writes_A_Manifest_That_SerenityStaticAssets_Reads()
    {
        var package = Path.Combine(root, "pkg", "staticwebassets") + Path.DirectorySeparatorChar;

        Write(ProjectRoot("My.Lib"), Root(package, "My.Package"));

        var entries = SerenityStaticAssets.ReadManifest(manifest);
        Assert.Equal(2, entries.Length);

        Assert.Equal("My.Lib", entries[0].BasePath);
        Assert.Equal(Folder("My.Lib").Replace('\\', '/'), entries[0].Root);
        Assert.True(entries[0].Filter);
        // wwwroot\** is just all files
        Assert.Null(entries[0].Includes);
        Assert.Equal("**/*.user;**/*.*proj;**/node_modules/**;**/.*/**;**/.*;**/*.tmp", entries[0].Excludes);
        Assert.True(entries[0].WellKnown);
        Assert.Equal("**/*.user;**/*.*proj;**/node_modules/**", entries[0].WellKnownExcludes);

        // package folders only contain the selected files, so they are not filtered
        Assert.Equal("My.Package", entries[1].BasePath);
        Assert.Equal(package.Replace('\\', '/'), entries[1].Root);
        Assert.False(entries[1].Filter);
        Assert.Null(entries[1].Includes);
        Assert.Null(entries[1].Excludes);
    }

    [Fact]
    public void Writes_Each_Pattern_Set_Once_And_References_It_By_Index()
    {
        Write(ProjectRoot("A"), ProjectRoot("B"), ProjectRoot("C", excludes: DefaultExcludes + @";wwwroot\**\*.map"));

        var json = Json();
        var sets = json.GetProperty("PatternSets");
        // excludes, well known excludes, and the different excludes of C
        Assert.Equal(3, sets.GetArrayLength());

        var folders = json.GetProperty("Folders").EnumerateArray().ToArray();
        Assert.Equal(0, folders[0].GetProperty("Excludes").GetInt32());
        Assert.Equal(1, folders[0].GetProperty("WellKnownExcludes").GetInt32());
        Assert.Equal(0, folders[1].GetProperty("Excludes").GetInt32());
        Assert.Equal(1, folders[1].GetProperty("WellKnownExcludes").GetInt32());
        Assert.Equal(2, folders[2].GetProperty("Excludes").GetInt32());
        Assert.Equal(1, folders[2].GetProperty("WellKnownExcludes").GetInt32());
        Assert.Contains("**/*.map", sets[2].EnumerateArray().Select(x => x.GetString()));
    }

    [Fact]
    public void Reuses_A_Pattern_Set_With_The_Same_Patterns_In_A_Different_Order()
    {
        Write(ProjectRoot("A", excludes: @"**/*.user;**\node_modules\**;wwwroot\**\*.tmp"),
            ProjectRoot("B", excludes: @"wwwroot\**\*.tmp;**/*.user;**\node_modules\**"));

        var folders = Json().GetProperty("Folders").EnumerateArray().ToArray();
        Assert.Equal(folders[0].GetProperty("Excludes").GetInt32(), folders[1].GetProperty("Excludes").GetInt32());
    }

    [Fact]
    public void Leaves_Out_Patterns_That_Cant_Match_Under_The_Folder_And_Duplicates()
    {
        Write(ProjectRoot("A", excludes: @"node_modules\**;coverage\**;out\**;;P:\startsharp\artifacts/**;bin/**;obj/**;" +
            @"P:\startsharp\artifacts\bin\A\/**;**/*.user;;;**\node_modules\**;wwwroot\**\*.tmp;WWWROOT\**\*.TMP;**/*.user;"));

        var entry = Assert.Single(SerenityStaticAssets.ReadManifest(manifest));
        Assert.Equal("**/*.user;**/node_modules/**;**/*.tmp", entry.Excludes);
    }

    [Fact]
    public void Keeps_Whitelist_Includes_Relative_To_The_Asset_Folder()
    {
        Write(ProjectRoot("A", includes: @"wwwroot\esm\**;wwwroot\*.css;other\**", wellKnown: ""));

        var entry = Assert.Single(SerenityStaticAssets.ReadManifest(manifest));
        Assert.Equal("esm/**;*.css", entry.Includes);
        Assert.False(entry.WellKnown);
        Assert.Null(entry.WellKnownExcludes);
    }

    [Fact]
    public void Omits_Empty_Excludes()
    {
        Write(ProjectRoot("A", excludes: @"bin/**", wellKnownExcludes: ""));

        Assert.False(Json().GetProperty("Folders")[0].TryGetProperty("Excludes", out _));
        var entry = Assert.Single(SerenityStaticAssets.ReadManifest(manifest));
        Assert.True(entry.Filter);
        Assert.Null(entry.Excludes);
        Assert.Null(entry.WellKnownExcludes);
    }

    [Fact]
    public void Uses_The_Pattern_Folder()
    {
        Assert.True(new WriteStaticAssetsManifestTask
        {
            Roots = [Root(Folder("A"), "A", ("Excludes", @"content\**\*.tmp;wwwroot\**\*.map"))],
            File = manifest,
            PatternFolder = "content"
        }.Execute());

        var entry = Assert.Single(SerenityStaticAssets.ReadManifest(manifest));
        Assert.Equal("**/*.tmp", entry.Excludes);
    }

    [Fact]
    public void Escapes_Json_Special_Characters()
    {
        Write(Root(Folder("A"), "A\"B\\C", ("Excludes", "**/x\t\"z\u0001")));

        var entry = Assert.Single(SerenityStaticAssets.ReadManifest(manifest));
        Assert.Equal("A\"B\\C", entry.BasePath);
        Assert.Equal("**/x\t\"z\u0001", entry.Excludes);
    }

    [Fact]
    public void Writes_Only_When_The_Contents_Change()
    {
        var a = ProjectRoot("A");
        Write(a);
        var oldTime = DateTime.UtcNow.AddMinutes(-10);
        File.SetLastWriteTimeUtc(manifest, oldTime);

        Write(a);
        Assert.Equal(oldTime, File.GetLastWriteTimeUtc(manifest));

        Write(a, ProjectRoot("B"));
        Assert.NotEqual(oldTime, File.GetLastWriteTimeUtc(manifest));
        Assert.Equal(2, SerenityStaticAssets.ReadManifest(manifest).Length);
    }

    [Fact]
    public void Writes_Empty_Arrays_For_No_Roots()
    {
        Write();

        var json = Json();
        Assert.Equal(0, json.GetProperty("PatternSets").GetArrayLength());
        Assert.Equal(0, json.GetProperty("Folders").GetArrayLength());
        Assert.Empty(SerenityStaticAssets.ReadManifest(manifest));
    }
}
