namespace Serenity.CodeGenerator;

public partial class GenerateCommandTests
{
    [Fact]
    public void Run_ThrowsArgumentNull_WhenArgumentsIsNull()
    {
        var fileSystem = new MockFileSystem();
        AddConfig(fileSystem, new GeneratorConfig());
        var command = new GenerateCommand(new MockProjectFileInfo(fileSystem),
            new MockGeneratorConsole());

        Assert.Throws<ArgumentNullException>(() => command.Run());
    }

    [Fact]
    public void Run_ReturnsNoConnectionString_WhenNoConnectionsDefined()
    {
        var fileSystem = new MockFileSystem();
        AddConfig(fileSystem, new GeneratorConfig());
        var console = new MockGeneratorConsole();
        var command = CreateCommand(fileSystem, console, []);

        var result = command.Run();

        Assert.Equal(ExitCodes.NoConnectionString, result);
        Assert.Contains(console.WriteCalls, x =>
            x.type == MockGeneratorConsole.CallType.Error &&
            x.message == "No connections in appsettings files or sergen.json!");
    }

    [Fact]
    public void Run_ReturnsInvalidConnectionKey_WhenKeyNotFound()
    {
        var fileSystem = new MockFileSystem();
        var config = new GeneratorConfig();
        config.Connections!.Add(new GeneratorConfig.Connection
        {
            Key = DefaultConnectionAttribute.Key,
            ConnectionString = "Data Source=test.db",
            ProviderName = "Microsoft.Data.Sqlite"
        });
        AddConfig(fileSystem, config);
        var console = new MockGeneratorConsole();
        var command = CreateCommand(fileSystem, console, ["--cnk", "Unknown"]);

        var result = command.Run();

        Assert.Equal(ExitCodes.InvalidConnectionKey, result);
        Assert.Contains(console.WriteCalls, x =>
            x.type == MockGeneratorConsole.CallType.Error &&
            x.message == "Can't find connection with key: Unknown!");
    }

    [Fact]
    public void Run_ReturnsNoConnectionString_WhenConnectionPromptReturnsNull()
    {
        var fileSystem = new MockFileSystem();
        var config = new GeneratorConfig();
        config.Connections!.Add(new GeneratorConfig.Connection
        {
            Key = DefaultConnectionAttribute.Key,
            ConnectionString = "Data Source=test.db",
            ProviderName = "Microsoft.Data.Sqlite"
        });
        AddConfig(fileSystem, config);
        var console = new MockGeneratorConsole();
        var command = CreateCommand(fileSystem, console, []);

        var result = command.Run();

        Assert.Equal(ExitCodes.NoConnectionString, result);
        Assert.Contains(console.WriteCalls, x => x.type == MockGeneratorConsole.CallType.Prompt);
        Assert.Contains(console.WriteCalls, x =>
            x.type == MockGeneratorConsole.CallType.Error &&
            x.message == "No connection selected!");
    }

    [Fact]
    public void Run_ParsesAppSettingsConnections_AndFixesRelativePaths()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, projectFile, csproj);
        fileSystem.Directory.SetCurrentDirectory(projectDir);
        AddFile(fileSystem, projectDir + "/sergen.json", new GeneratorConfig().SaveToJson());
        AddFile(fileSystem, projectDir + "/appsettings.json", /*lang=json*/ """
            {
                "Data": {
                    "Forward": {
                        "ConnectionString": "Data Source=../../../forward.db",
                        "ProviderName": "Microsoft.Data.Sqlite"
                    },
                    "Backward": {
                        "ConnectionString": "Data Source=..\\..\\..\\backward.db",
                        "ProviderName": "Microsoft.Data.Sqlite"
                    }
                }
            }
            """);
        var console = new MockGeneratorConsole();
        var command = CreateCommand(fileSystem, console, ["--cnk", "Unknown"]);

        var result = command.Run();

        Assert.Equal(ExitCodes.InvalidConnectionKey, result);
    }
}
