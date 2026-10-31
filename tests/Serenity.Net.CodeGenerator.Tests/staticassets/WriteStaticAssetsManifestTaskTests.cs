using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Serenity.Web;
using System.IO;

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

    private static ITaskItem Root(string path, string basePath, params (string name, string value)[] metadata)
    {
        var item = new TaskItem(path);
        item.SetMetadata("BasePath", basePath);
        foreach (var (name, value) in metadata)
            item.SetMetadata(name, value);
        return item;
    }

    private void Write(params ITaskItem[] roots)
    {
        Assert.True(new WriteStaticAssetsManifestTask { Roots = roots, File = manifest }.Execute());
    }

    [Fact]
    public void Writes_A_Manifest_That_SerenityStaticAssets_Reads()
    {
        var project = Path.Combine(root, "My.Lib", "wwwroot") + Path.DirectorySeparatorChar;
        var package = Path.Combine(root, "pkg", "staticwebassets") + Path.DirectorySeparatorChar;

        Write(
            Root(project, "My.Lib", ("Includes", @"wwwroot\**"), ("Excludes", @"**/.*/**;wwwroot\**\*.tmp"),
                ("WellKnown", "true"), ("WellKnownExcludes", "**/*.user")),
            Root(package, "My.Package"));

        var entries = SerenityStaticAssets.ReadManifest(manifest);
        Assert.Equal(2, entries.Length);

        Assert.Equal("My.Lib", entries[0].BasePath);
        Assert.Equal(project, entries[0].Root);
        Assert.Equal(@"wwwroot\**", entries[0].Includes);
        Assert.Equal(@"**/.*/**;wwwroot\**\*.tmp", entries[0].Excludes);
        Assert.True(entries[0].WellKnown);
        Assert.Equal("**/*.user", entries[0].WellKnownExcludes);

        Assert.Equal("My.Package", entries[1].BasePath);
        Assert.Equal(package, entries[1].Root);
        Assert.Null(entries[1].Includes);
        Assert.Null(entries[1].Excludes);
        Assert.False(entries[1].WellKnown);
    }

    [Fact]
    public void Omits_Empty_Pattern_Fields_And_WellKnown_When_Not_True()
    {
        Write(Root(Path.Combine(root, "a") + Path.DirectorySeparatorChar, "A", ("Includes", ""), ("WellKnown", "false"),
            ("WellKnownExcludes", "**/*.user")));

        var json = File.ReadAllText(manifest);
        Assert.DoesNotContain("Includes", json);
        Assert.DoesNotContain("WellKnown", json);
    }

    [Fact]
    public void Escapes_Json_Special_Characters()
    {
        Write(Root(Path.Combine(root, "a") + Path.DirectorySeparatorChar, "A\"B", ("Excludes", "x\\y\t\"z\u0001")));

        var entry = Assert.Single(SerenityStaticAssets.ReadManifest(manifest));
        Assert.Equal("A\"B", entry.BasePath);
        Assert.Equal("x\\y\t\"z\u0001", entry.Excludes);
    }

    [Fact]
    public void Writes_Only_When_The_Contents_Change()
    {
        var a = Root(Path.Combine(root, "a") + Path.DirectorySeparatorChar, "A");
        Write(a);
        var oldTime = DateTime.UtcNow.AddMinutes(-10);
        File.SetLastWriteTimeUtc(manifest, oldTime);

        Write(a);
        Assert.Equal(oldTime, File.GetLastWriteTimeUtc(manifest));

        Write(a, Root(Path.Combine(root, "b") + Path.DirectorySeparatorChar, "B"));
        Assert.NotEqual(oldTime, File.GetLastWriteTimeUtc(manifest));
        Assert.Equal(2, SerenityStaticAssets.ReadManifest(manifest).Length);
    }

    [Fact]
    public void Writes_An_Empty_Array_For_No_Roots()
    {
        Write();

        Assert.Empty(SerenityStaticAssets.ReadManifest(manifest));
    }
}
