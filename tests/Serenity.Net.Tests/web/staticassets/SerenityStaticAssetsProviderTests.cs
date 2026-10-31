using Microsoft.Extensions.FileProviders;
using System.IO;

namespace Serenity.Web;

public class SerenityStaticAssetsProviderTests : IDisposable
{
    private readonly string root;

    public SerenityStaticAssetsProviderTests()
    {
        root = Path.Combine(Path.GetTempPath(), "ssat-" + Guid.NewGuid().ToString("N"));
        foreach (var file in new[] { "index.js", "index.js.map", "file.tmp", "esm/page.js", "node_modules/pkg/x.js",
            ".well-known/security.txt", ".hidden/x.js" })
        {
            var path = Path.Combine(root, file.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, file);
        }
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

    // patterns relative to the folder, as in the manifest
    private SerenityStaticAssets.ManifestEntry Entry(string? includes = "**", string? excludes =
        "**/.*/**;**/node_modules/**;**/.*;**/*.tmp", bool wellKnown = true)
    {
        return new SerenityStaticAssets.ManifestEntry
        {
            BasePath = "My.Lib",
            Root = root,
            Includes = includes,
            Excludes = excludes,
            WellKnown = wellKnown,
            WellKnownExcludes = @"**/*.user"
        };
    }

    [Theory]
    [InlineData("/My.Lib/index.js", true)]
    [InlineData("/My.Lib/index.js.map", true)]
    [InlineData("/My.Lib/esm/page.js", true)]
    [InlineData("/My.Lib/.well-known/security.txt", true)]
    [InlineData("/My.Lib/file.tmp", false)]
    [InlineData("/My.Lib/node_modules/pkg/x.js", false)]
    [InlineData("/My.Lib/.hidden/x.js", false)]
    [InlineData("/Other.Lib/index.js", false)]
    [InlineData("/index.js", false)]
    public void Serves_Only_Files_Matching_The_Patterns_Under_The_Base_Path(string path, bool expected)
    {
        var provider = SerenityStaticAssets.CreateFileProvider(Entry());
        Assert.Equal(expected, provider.GetFileInfo(path).Exists);
    }

    [Fact]
    public void Serves_All_Files_When_Filter_Is_False()
    {
        // e.g. package folders, which only contain the files selected while packing
        var entry = Entry();
        entry.Filter = false;
        var provider = SerenityStaticAssets.CreateFileProvider(entry);
        Assert.True(provider.GetFileInfo("/My.Lib/file.tmp").Exists);
        Assert.True(provider.GetFileInfo("/My.Lib/node_modules/pkg/x.js").Exists);
        Assert.True(provider.GetFileInfo("/My.Lib/.hidden/x.js").Exists);
    }

    [Theory]
    [InlineData("/My.Lib/index.js")]
    [InlineData("/My.Lib/.well-known/security.txt")]
    [InlineData("/My.Lib/file.tmp")]
    [InlineData("/My.Lib/node_modules/pkg/x.js")]
    [InlineData("/My.Lib/.hidden/x.js")]
    public void Excludes_Nothing_When_There_Are_No_Patterns(string path)
    {
        // no implicit default patterns, the manifest always lists them
        var provider = SerenityStaticAssets.CreateFileProvider(new SerenityStaticAssets.ManifestEntry
        {
            BasePath = "My.Lib",
            Root = root
        });
        Assert.True(provider.GetFileInfo(path).Exists);
    }

    [Fact]
    public void Filters_Directory_Contents()
    {
        var provider = SerenityStaticAssets.CreateFileProvider(Entry());
        var names = provider.GetDirectoryContents("/My.Lib").Select(x => x.Name).OrderBy(x => x).ToArray();
        // directories are kept, their files are filtered when listed
        Assert.Contains("esm", names);
        Assert.Contains("index.js", names);
        Assert.DoesNotContain("file.tmp", names);
    }

    [Fact]
    public void FilteredFileProvider_Passes_Relative_Paths_To_The_Filter()
    {
        var seen = new List<string>();
        var provider = new FilteredFileProvider(new PhysicalFileProvider(root), path => { seen.Add(path); return true; });

        Assert.True(provider.GetFileInfo("/esm/page.js").Exists);
        Assert.True(provider.GetFileInfo(@"esm\page.js").Exists);
        Assert.Equal(["esm/page.js", "esm/page.js"], seen);
    }

    [Fact]
    public void FilteredFileProvider_Returns_Not_Found_For_Excluded_Or_Empty_Paths()
    {
        var provider = new FilteredFileProvider(new PhysicalFileProvider(root), _ => false);
        Assert.False(provider.GetFileInfo("/index.js").Exists);
        Assert.False(provider.GetFileInfo("").Exists);
    }

    [Fact]
    public void ReadManifest_Reads_Patterns()
    {
        var manifest = Path.Combine(root, "test.serenitystaticassets.json");
        File.WriteAllText(manifest, """
            {
              "PatternSets": [
                ["esm/**"],
                ["**/.*/**", "**/*.tmp"],
                ["**/*.user"]
              ],
              "Folders": [
                { "BasePath": "My.Lib", "Root": "C:/x/wwwroot/", "Includes": 0, "Excludes": 1, "WellKnown": false, "WellKnownExcludes": 2 },
                { "BasePath": "Pkg", "Root": "C:/pkg/staticwebassets/", "Filter": false },
                { "BasePath": "Defaults", "Root": "C:/x/wwwroot/", "Excludes": 99 }
              ]
            }
            """);

        var entries = SerenityStaticAssets.ReadManifest(manifest);
        Assert.Equal(3, entries.Length);
        Assert.True(entries[0].Filter);
        Assert.Equal("esm/**", entries[0].Includes);
        Assert.Equal("**/.*/**;**/*.tmp", entries[0].Excludes);
        Assert.False(entries[0].WellKnown);
        Assert.Equal("**/*.user", entries[0].WellKnownExcludes);

        Assert.False(entries[1].Filter);

        // omitted keys mean no patterns, an invalid index is ignored, flags default to true
        Assert.True(entries[2].Filter);
        Assert.Null(entries[2].Includes);
        Assert.Null(entries[2].Excludes);
        Assert.True(entries[2].WellKnown);
        Assert.Null(entries[2].WellKnownExcludes);
    }
}
