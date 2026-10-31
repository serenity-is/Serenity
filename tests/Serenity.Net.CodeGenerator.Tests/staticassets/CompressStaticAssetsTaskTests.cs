using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.IO;
using System.IO.Compression;

namespace Serenity.CodeGenerator;

public class CompressStaticAssetsTaskTests : IDisposable
{
    private readonly string root;
    private readonly string source;
    private readonly string cache;

    public CompressStaticAssetsTaskTests()
    {
        root = Path.Combine(Path.GetTempPath(), "csat-" + Guid.NewGuid().ToString("N"));
        source = Path.Combine(root, "src");
        cache = Path.Combine(root, "cache");
        Directory.CreateDirectory(source);
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

    private ITaskItem File(string relativePath, string content = null, string assetPath = null, string compressionExcludePatterns = null)
    {
        var path = Path.Combine(source, relativePath.Replace('\\', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        System.IO.File.WriteAllText(path, content ?? string.Concat(Enumerable.Repeat("body { color: red; } ", 50)));

        var item = new TaskItem(path);
        item.SetMetadata("RelativePath", relativePath);
        if (assetPath != null)
            item.SetMetadata("AssetPath", assetPath);
        if (compressionExcludePatterns != null)
            item.SetMetadata("CompressionExcludePatterns", compressionExcludePatterns);
        return item;
    }

    private CompressStaticAssetsTask Run(ITaskItem[] files, string includes = "**/*.js;**/*.css;**/*.map",
        string excludes = null, string formats = "gzip;brotli")
    {
        var task = new CompressStaticAssetsTask
        {
            Files = files,
            IntermediateDirectory = cache,
            PatternRoot = "wwwroot",
            IncludePatterns = includes,
            ExcludePatterns = excludes,
            Formats = formats
        };

        Assert.True(task.Execute());
        return task;
    }

    private static string Decompress(string path)
    {
        using var input = System.IO.File.OpenRead(path);
        using Stream decompressor = path.EndsWith(".br") ?
            new BrotliStream(input, CompressionMode.Decompress) :
            new GZipStream(input, CompressionMode.Decompress);
        using var reader = new StreamReader(decompressor);
        return reader.ReadToEnd();
    }

    private static string[] RelativePaths(CompressStaticAssetsTask task) =>
        task.CompressedFiles.Select(x => x.GetMetadata("RelativePath")).OrderBy(x => x, StringComparer.Ordinal).ToArray();

    [Fact]
    public void Creates_Brotli_And_Gzip_Files_That_Decompress_To_The_Source()
    {
        var file = File(@"wwwroot\css\site.css");

        var task = Run([file]);

        Assert.Equal([@"wwwroot\css\site.css.br", @"wwwroot\css\site.css.gz"], RelativePaths(task));
        var original = System.IO.File.ReadAllText(file.ItemSpec);
        foreach (var compressed in task.CompressedFiles)
        {
            Assert.StartsWith(cache, compressed.ItemSpec);
            Assert.Equal("PreserveNewest", compressed.GetMetadata("CopyToPublishDirectory"));
            Assert.Equal(original, Decompress(compressed.ItemSpec));
        }
    }

    [Fact]
    public void Uses_Include_And_Exclude_Patterns_Relative_To_The_Pattern_Root()
    {
        var task = Run([File(@"wwwroot\app.js"), File(@"wwwroot\app.js.map"), File(@"wwwroot\image.png"),
            File(@"wwwroot\esm\page.js")], excludes: "**/*.map;esm/**");

        Assert.Equal([@"wwwroot\app.js.br", @"wwwroot\app.js.gz"], RelativePaths(task));
    }

    [Fact]
    public void Applies_Library_Exclude_Patterns_To_The_Asset_Path()
    {
        var task = Run([
            File(@"wwwroot\My.Lib\index.js", assetPath: "index.js", compressionExcludePatterns: "**/*.txt"),
            File(@"wwwroot\My.Lib\notes.txt.js", assetPath: "notes.txt.js", compressionExcludePatterns: "**/notes*"),
            File(@"wwwroot\Other\notes.txt.js")
        ]);

        Assert.Equal([@"wwwroot\My.Lib\index.js.br", @"wwwroot\My.Lib\index.js.gz",
            @"wwwroot\Other\notes.txt.js.br", @"wwwroot\Other\notes.txt.js.gz"], RelativePaths(task));
    }

    [Fact]
    public void Keeps_Existing_Compressed_Files_To_Publish()
    {
        // e.g. created by the static web assets SDK
        var task = Run([File(@"wwwroot\app.js"), File(@"wwwroot\app.js.br", "already")], includes: "**/*.js");

        Assert.Equal([@"wwwroot\app.js.gz"], RelativePaths(task));
    }

    [Fact]
    public void Skips_Empty_Files()
    {
        var task = Run([File(@"wwwroot\empty.js", "")]);

        Assert.Empty(task.CompressedFiles);
    }

    [Fact]
    public void Creates_Variants_Even_When_They_Are_Not_Smaller()
    {
        var task = Run([File(@"wwwroot\tiny.js", "x")]);

        Assert.Equal([@"wwwroot\tiny.js.br", @"wwwroot\tiny.js.gz"], RelativePaths(task));
    }

    [Theory]
    [InlineData("gzip", new[] { @"wwwroot\app.js.gz" })]
    [InlineData("brotli", new[] { @"wwwroot\app.js.br" })]
    [InlineData("GZip;Brotli;gzip", new[] { @"wwwroot\app.js.br", @"wwwroot\app.js.gz" })]
    [InlineData("", new string[0])]
    [InlineData(" ; ", new string[0])]
    public void Uses_Formats(string formats, string[] expected)
    {
        var task = Run([File(@"wwwroot\app.js")], formats: formats);

        Assert.Equal(expected, RelativePaths(task));
    }

    [Fact]
    public void Processes_Files_Listed_More_Than_Once_Only_Once()
    {
        var file = File(@"wwwroot\app.js");

        var task = Run([file, file]);

        Assert.Equal([@"wwwroot\app.js.br", @"wwwroot\app.js.gz"], RelativePaths(task));
    }

    [Fact]
    public void Reuses_Cached_Files_Until_The_Source_Changes()
    {
        var file = File(@"wwwroot\app.js");
        var first = Run([file]).CompressedFiles.Single(x => x.ItemSpec.EndsWith(".gz")).ItemSpec;
        var cachedTime = DateTime.UtcNow.AddMinutes(-10);
        System.IO.File.SetLastWriteTimeUtc(file.ItemSpec, cachedTime.AddMinutes(-1));
        System.IO.File.SetLastWriteTimeUtc(first, cachedTime);

        Run([file]);
        Assert.Equal(cachedTime, System.IO.File.GetLastWriteTimeUtc(first));

        System.IO.File.WriteAllText(file.ItemSpec, "changed changed changed");
        Run([file]);
        Assert.NotEqual(cachedTime, System.IO.File.GetLastWriteTimeUtc(first));
        Assert.Equal("changed changed changed", Decompress(first));
    }

    [Fact]
    public void Ignores_Missing_Source_Files()
    {
        var item = new TaskItem(Path.Combine(source, "missing.js"));
        item.SetMetadata("RelativePath", @"wwwroot\missing.js");

        var task = Run([item]);

        Assert.Empty(task.CompressedFiles);
    }
}
