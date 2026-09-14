namespace Serenity.CodeGenerator;

public partial class TSConfigHelperTests
{
    [Fact]
    public void LocateTSConfigFile_ReturnsRootConfig_WhenModuleDefined()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/tsconfig.json", /*lang=json*/ """
            {
                "compilerOptions": { "module": "ESNext" }
            }
            """);

        Assert.Equal(fileSystem.Combine("/root", "tsconfig.json"),
            TSConfigHelper.LocateTSConfigFile(fileSystem, "/root"));
    }

    [Fact]
    public void LocateTSConfigFile_FallsBackToModulesFolder()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/Modules/tsconfig.json", /*lang=json*/ """
            {
                "compilerOptions": { "module": "CommonJS" }
            }
            """);

        Assert.Equal(fileSystem.Combine("/root", "Modules", "tsconfig.json"),
            TSConfigHelper.LocateTSConfigFile(fileSystem, "/root"));
    }

    [Fact]
    public void LocateTSConfigFile_SkipsConfig_WhenModuleIsNoneOrMissing()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/tsconfig.json", /*lang=json*/ """
            {
                "compilerOptions": { "module": "none" }
            }
            """);

        Assert.Null(TSConfigHelper.LocateTSConfigFile(fileSystem, "/root"));
    }

    [Fact]
    public void LocateTSConfigFile_SkipsInvalidConfig_AndUsesModulesFolder()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/tsconfig.json", "not json");
        AddFile(fileSystem, "/root/Modules/tsconfig.json", /*lang=json*/ """
            {
                "compilerOptions": { "module": "ESNext" }
            }
            """);

        Assert.Equal(fileSystem.Combine("/root", "Modules", "tsconfig.json"),
            TSConfigHelper.LocateTSConfigFile(fileSystem, "/root"));
    }

    [Fact]
    public void LocateTSConfigFile_ReturnsNull_WhenNoConfigFound()
    {
        var fileSystem = new MockFileSystem();

        Assert.Null(TSConfigHelper.LocateTSConfigFile(fileSystem, "/root"));
    }

    [Theory]
    [InlineData("/root/a.ts", true)]
    [InlineData("/root/a.tsx", true)]
    [InlineData("/root/a.mts", true)]
    [InlineData("/root/a.TS", true)]
    [InlineData("/root/a.js", false)]
    [InlineData("/root/a.json", false)]
    public void HasTSExtension_ChecksExtension(string path, bool expected)
    {
        Assert.Equal(expected, TSConfigHelper.HasTSExtension(path));
    }

    [Fact]
    public void TryParseJsonFile_Throws_ForNullArguments()
    {
        var fileSystem = new MockFileSystem();

        Assert.Throws<ArgumentNullException>(() => TSConfigHelper.TryParseJsonFile<TSConfig>(null!, "/root/a.json"));
        Assert.Throws<ArgumentNullException>(() => TSConfigHelper.TryParseJsonFile<TSConfig>(fileSystem, null!));
    }

    [Fact]
    public void TryParseJsonFile_ReturnsNull_WhenFileMissing()
    {
        var fileSystem = new MockFileSystem();

        Assert.Null(TSConfigHelper.TryParseJsonFile<TSConfig>(fileSystem, "/root/a.json"));
    }

    [Fact]
    public void TryParseJsonFile_ReturnsNull_WhenInvalid()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/a.json", "{ invalid");

        Assert.Null(TSConfigHelper.TryParseJsonFile<TSConfig>(fileSystem, "/root/a.json"));
    }

    [Fact]
    public void TryParseJsonFile_ParsesValidFile()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/a.json", /*lang=json*/ """
            { "compilerOptions": { "module": "ESNext" } }
            """);

        var config = TSConfigHelper.TryParseJsonFile<TSConfig>(fileSystem, "/root/a.json");

        Assert.NotNull(config);
        Assert.Equal("ESNext", config.CompilerOptions!.Module);
    }
}
