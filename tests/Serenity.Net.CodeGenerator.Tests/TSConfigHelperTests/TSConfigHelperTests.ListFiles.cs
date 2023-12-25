using Serenity.CodeGenerator;

namespace Serenity.Tests.CodeGenerator;

public partial class TSConfigHelperTests
{
    [InlineData("Modules")]
    [InlineData("Modules/")]
    [InlineData("Modules/**/*")]
    [InlineData("./Modules")]
    [InlineData("./Modules/")]
    [InlineData("./Modules/**/*")]
    [Theory]
    public void ListFiles_IncludesAllFilesFor_MainFolderPattern(string pattern)
    {
        var fileSystem = new MockFileSystem();
        var currentDirectory = fileSystem.Directory.GetCurrentDirectory();
        currentDirectory = fileSystem.Combine(currentDirectory, "Test");
        fileSystem.Directory.SetCurrentDirectory(currentDirectory);

        var tsconfig = fileSystem.Combine(currentDirectory, "tsconfig.json");

        fileSystem.AddFile(tsconfig, new($$"""
            {
                "compilerOptions": {
                    "module": "ESNext"
                },
                "include": [
                    "{{pattern}}"
                ]
            }
            """));

        string[] testFiles = [
            "/Modules/outside.ts",
            "node_modules/Modules/dont.ts",
            "node_modules/Modules/test/dont.ts",
            "Modules/test.ts",
            "Modules/test2.tsx",
            "Modules/Sub/test3.ts",
            "Modules/Sub/Another/test4.tsx",
            "Modules/Sub/Another/test5.ts"
        ];

        foreach (var testFile in testFiles)
            fileSystem.AddEmptyFile(fileSystem.Combine(currentDirectory, PathHelper.ToPath(testFile)));

        var expectedFiles = testFiles.Where(x => x.StartsWith("Modules/", StringComparison.Ordinal))
            .Select(x => fileSystem.Combine(currentDirectory, PathHelper.ToPath(x)))
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToArray();

        var files = TSConfigHelper.ListFiles(fileSystem, tsconfig, out _)
            .OrderBy(x => x, StringComparer.Ordinal);

        Assert.Equal(expectedFiles, files);
    }

    [InlineData("**/*.tsx")]
    [InlineData("./**/*.tsx")]
    [InlineData("./Modules/*.tsx", "*/*/*/*.tsx")]
    [Theory]
    public void ListFiles_Handles_ExcludePatterns(params string[] pattern)
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile("tsconfig.json", new($$"""
            {
                "compilerOptions": {
                    "module": "ESNext"
                },
                "exclude": {{JSON.Stringify(pattern)}},
                "include": [
                    "Modules"
                ]
            }
            """));

        string[] testFiles = [
            "node_modules/Modules/dont.ts",
            "node_modules/Modules/test/dont.ts",
            "Modules/Sub/test1.ts",
            "Modules/Sub/Another/test2.tsx",
            "Modules/Sub/Another/test3.ts",
            "Modules/test4.ts",
            "Modules/test5.tsx",
        ];

        foreach (var testFile in testFiles)
            fileSystem.AddEmptyFile(testFile);

        var currentDirectory = fileSystem.Directory.GetCurrentDirectory();
        var expectedFiles = testFiles.Where(x => 
                x.StartsWith("Modules/", StringComparison.Ordinal) &&
                !x.EndsWith(".tsx", StringComparison.Ordinal))
            .Select(x => fileSystem.Combine(currentDirectory, PathHelper.ToPath(x)))
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToArray();

        var files = TSConfigHelper.ListFiles(fileSystem, fileSystem.Combine(currentDirectory, "tsconfig.json"), out _)
            .OrderBy(x => x, StringComparer.Ordinal);

        Assert.Equal(expectedFiles, files);
    }
}
