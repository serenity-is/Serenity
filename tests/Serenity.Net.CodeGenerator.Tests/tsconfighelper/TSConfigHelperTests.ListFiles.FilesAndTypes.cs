namespace Serenity.CodeGenerator;

public partial class TSConfigHelperTests
{
    [Fact]
    public void ListFiles_Uses_FilesArray_OnlyForExistingFiles()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/tsconfig.json", /*lang=json*/ """
            {
                "files": ["a.ts", "missing.ts"]
            }
            """);
        AddFile(fileSystem, "/root/a.ts", "");

        var files = TSConfigHelper.ListFiles(fileSystem, "/root/tsconfig.json", out var tsConfig,
            TestContext.Current.CancellationToken).ToArray();

        Assert.Equal(["a.ts", "missing.ts"], tsConfig.Files);
        Assert.Single(files);
        Assert.EndsWith("a.ts", files[0], StringComparison.Ordinal);
    }

    [Fact]
    public void ListFiles_UsesDefaultInclude_WhenConfigMissing()
    {
        var fileSystem = new MockFileSystem();

        var files = TSConfigHelper.ListFiles(fileSystem, "/root/missing.json", out var tsConfig,
            TestContext.Current.CancellationToken);

        Assert.Equal(["Modules"], tsConfig.Include);
        Assert.Empty(files);
    }

    [Fact]
    public void ListFiles_Handles_IncludeWithoutWildcardsForSingleFile()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/tsconfig.json", /*lang=json*/ """
            {
                "include": ["Modules/only.ts"]
            }
            """);
        AddFile(fileSystem, "/root/Modules/only.ts", "");
        AddFile(fileSystem, "/root/Modules/other.ts", "");

        var files = TSConfigHelper.ListFiles(fileSystem, "/root/tsconfig.json", out _,
            TestContext.Current.CancellationToken).ToArray();

        Assert.Single(files);
        Assert.EndsWith("only.ts", files[0], StringComparison.Ordinal);
    }

    [Fact]
    public void ListFiles_Includes_TypeRootsIndexDts_OutsideNodeModules()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/tsconfig.json", /*lang=json*/ """
            {
                "compilerOptions": {
                    "typeRoots": ["types"]
                },
                "include": ["Modules"]
            }
            """);
        AddFile(fileSystem, "/root/types/foo/index.d.ts", "");
        AddFile(fileSystem, "/root/types/bar/index.d.ts", "");
        AddFile(fileSystem, "/root/Modules/a.ts", "");

        var files = TSConfigHelper.ListFiles(fileSystem, "/root/tsconfig.json", out _,
            TestContext.Current.CancellationToken).ToArray();

        Assert.Contains(files, x => EndsWithPath(x, "foo/index.d.ts"));
        Assert.Contains(files, x => EndsWithPath(x, "bar/index.d.ts"));
        Assert.Contains(files, x => EndsWithPath(x, "a.ts"));
    }

    [Fact]
    public void ListFiles_Only_Includes_SerenityTypings_InsideNodeModules()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/tsconfig.json", /*lang=json*/ """
            {
                "compilerOptions": {
                    "typeRoots": ["./node_modules/@types"]
                },
                "include": ["Modules"]
            }
            """);
        AddFile(fileSystem, "/root/node_modules/@types/serenity-core/index.d.ts", "");
        AddFile(fileSystem, "/root/node_modules/@types/foo/index.d.ts", "");
        AddFile(fileSystem, "/root/Modules/a.ts", "");

        var files = TSConfigHelper.ListFiles(fileSystem, "/root/tsconfig.json", out _,
            TestContext.Current.CancellationToken).ToArray();

        Assert.Contains(files, x => EndsWithPath(x, "serenity-core/index.d.ts"));
        Assert.DoesNotContain(files, x => EndsWithPath(x, "foo/index.d.ts"));
    }

    [Fact]
    public void ListFiles_Applies_TypesFilter()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/tsconfig.json", /*lang=json*/ """
            {
                "compilerOptions": {
                    "typeRoots": ["types"],
                    "types": ["foo"]
                },
                "include": ["Modules"]
            }
            """);
        AddFile(fileSystem, "/root/types/foo/index.d.ts", "");
        AddFile(fileSystem, "/root/types/bar/index.d.ts", "");
        AddFile(fileSystem, "/root/Modules/a.ts", "");

        var files = TSConfigHelper.ListFiles(fileSystem, "/root/tsconfig.json", out _,
            TestContext.Current.CancellationToken).ToArray();

        Assert.Contains(files, x => EndsWithPath(x, "a.ts"));
    }

    [Fact]
    public void ListFiles_Scans_Recursively_WhenIncludeFolderHasWildcard()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/tsconfig.json", /*lang=json*/ """
            {
                "include": ["**/*.ts"]
            }
            """);
        AddFile(fileSystem, "/root/Modules/a.ts", "");
        AddFile(fileSystem, "/root/Controllers/b.ts", "");
        AddFile(fileSystem, "/root/Modules/c.tsx", "");

        var files = TSConfigHelper.ListFiles(fileSystem, "/root/tsconfig.json", out _,
            TestContext.Current.CancellationToken).ToArray();

        Assert.Contains(files, x => EndsWithPath(x, "Modules/a.ts"));
        Assert.Contains(files, x => EndsWithPath(x, "Controllers/b.ts"));
        Assert.DoesNotContain(files, x => EndsWithPath(x, "Modules/c.tsx"));
    }

    [Fact]
    public void ListFiles_Ignores_EmptyIncludePatterns()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/tsconfig.json", /*lang=json*/ """
            {
                "include": ["", "Modules"]
            }
            """);
        AddFile(fileSystem, "/root/Modules/a.ts", "");

        var files = TSConfigHelper.ListFiles(fileSystem, "/root/tsconfig.json", out _,
            TestContext.Current.CancellationToken).ToArray();

        Assert.Single(files);
        Assert.True(EndsWithPath(files[0], "Modules/a.ts"));
    }
}
