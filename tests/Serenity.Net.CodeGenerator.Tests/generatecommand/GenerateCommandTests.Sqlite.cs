namespace Serenity.CodeGenerator;

public partial class GenerateCommandTests
{
    private static GeneratorConfig SqliteConfig(string connectionString, bool saveGeneratedTables = true)
    {
        var config = new GeneratorConfig
        {
            RootNamespace = "My",
            EndOfLine = "lf",
            SaveGeneratedTables = saveGeneratedTables
        };
        config.Connections!.Add(new GeneratorConfig.Connection
        {
            Key = "Default",
            ConnectionString = connectionString,
            ProviderName = "Microsoft.Data.Sqlite",
            Dialect = "Sqlite"
        });
        return config;
    }

    [Fact]
    public void Generate_WithAllArguments_WritesEntityFiles()
    {
        using var db = new MemorySqliteDb("MyTable");

        var fileSystem = new MockFileSystem();
        var config = SqliteConfig(db.ConnectionString);
        config.CustomTemplates = "Templates";
        AddConfig(fileSystem, config);
        var console = new MockGeneratorConsole();
        var command = CreateCommand(fileSystem, console,
            ["--cnk", "Default", "--tbl", "MyTable", "--mod", "Test",
             "--cls", "MyTable", "--pms", "Test:Permission", "--wtg", "*"]);

        var result = command.Run();

        Assert.Equal(ExitCodes.Success, result);
        Assert.Contains(console.WriteCalls, x =>
            x.type == MockGeneratorConsole.CallType.WriteLine &&
            x.message == "Table Code Generation");

        var generated = fileSystem.GetFiles(projectDir, "*.cs", recursive: true);
        Assert.Contains(generated, x => x.EndsWith("MyTableRow.cs", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(generated, x => x.EndsWith("MyTableEndpoint.cs", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(generated, x => x.EndsWith("MyTableColumns.cs", StringComparison.OrdinalIgnoreCase));

        var sergen = fileSystem.ReadAllText(projectDir + "/sergen.json");
        Assert.Contains("MyTable", sergen, StringComparison.Ordinal);
    }

    [Fact]
    public void Generate_Interactive_SelectsTablesAndWhatToGenerate()
    {
        using var db = new MemorySqliteDb("MyTable");

        var fileSystem = new MockFileSystem();
        var config = SqliteConfig(db.ConnectionString, saveGeneratedTables: false);
        AddConfig(fileSystem, config);
        var console = new MockGeneratorConsole();
        console.PromptResults.Enqueue(new List<string> { "MyTable" });
        console.PromptResults.Enqueue("TestModule");
        console.PromptResults.Enqueue("MyTable");
        console.PromptResults.Enqueue("Test:Permission");
        console.PromptResults.Enqueue(new List<string> { "Row" });

        var command = CreateCommand(fileSystem, console, ["--cnk", "Default"]);

        var result = command.Run();

        Assert.Equal(ExitCodes.Success, result);

        var generated = fileSystem.GetFiles(projectDir, "*.cs", recursive: true);
        Assert.Contains(generated, x => x.EndsWith("MyTableRow.cs", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(generated, x => x.EndsWith("MyTableColumns.cs", StringComparison.OrdinalIgnoreCase));

        Assert.False(fileSystem.FileExists(projectDir + "/sergen.json") &&
            fileSystem.ReadAllText(projectDir + "/sergen.json").Contains("MyTable",
                StringComparison.Ordinal));
    }

    [Fact]
    public void Generate_ReturnsNoTablesSelected_WhenPromptReturnsEmpty()
    {
        using var db = new MemorySqliteDb("MyTable");

        var fileSystem = new MockFileSystem();
        AddConfig(fileSystem, SqliteConfig(db.ConnectionString));
        var console = new MockGeneratorConsole();
        console.PromptResults.Enqueue(new List<string>());

        var command = CreateCommand(fileSystem, console, ["--cnk", "Default"]);

        var result = command.Run();

        Assert.Equal(ExitCodes.NoTablesSelected, result);
        Assert.Contains(console.WriteCalls, x =>
            x.type == MockGeneratorConsole.CallType.Error &&
            x.message == "No tables selected!");
    }

    [Fact]
    public void Generate_ReturnsInvalidTable_WhenTableNotFound()
    {
        using var db = new MemorySqliteDb("MyTable");

        var fileSystem = new MockFileSystem();
        AddConfig(fileSystem, SqliteConfig(db.ConnectionString));
        var console = new MockGeneratorConsole();

        var command = CreateCommand(fileSystem, console,
            ["--cnk", "Default", "--tbl", "Missing"]);

        var result = command.Run();

        Assert.Equal(ExitCodes.InvalidTable, result);
        Assert.Contains(console.WriteCalls, x =>
            x.type == MockGeneratorConsole.CallType.Error &&
            x.message == "Can't find table with name: Missing!");
    }

    [Fact]
    public void Generate_UsesAppSettingsConnection_AndGlobalUsings()
    {
        using var db = new MemorySqliteDb("MyTable");

        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, projectFile, csproj);
        fileSystem.Directory.SetCurrentDirectory(projectDir);
        var config = new GeneratorConfig
        {
            RootNamespace = "My",
            EndOfLine = "lf",
            IncludeGlobalUsings = ["Extra.Using"]
        };
        AddFile(fileSystem, projectDir + "/sergen.json", config.SaveToJson());
        AddFile(fileSystem, projectDir + "/appsettings.json", $$"""
            {
                "Data": {
                    "Default": {
                        "ConnectionString": "{{db.ConnectionString}}",
                        "ProviderName": "Microsoft.Data.Sqlite"
                    }
                }
            }
            """);
        var console = new MockGeneratorConsole();
        var globalUsings = new Dictionary<string, string?>
        {
            ["Serenity"] = null,
            ["Alias"] = "Target"
        };

        var command = CreateCommand(fileSystem, console,
            ["--cnk", "Default", "--tbl", "MyTable", "--mod", "Test",
             "--cls", "MyTable", "--pms", "Test:Permission", "--wtg", "R"],
            globalUsings, [typeof(GenerateCommand).Assembly.Location]);

        var result = command.Run();

        Assert.Equal(ExitCodes.Success, result);
        var generated = fileSystem.GetFiles(projectDir, "*.cs", recursive: true);
        Assert.Contains(generated, x => x.EndsWith("MyTableRow.cs", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Generate_UpdatesExistingConfigTable()
    {
        using var db = new MemorySqliteDb("MyTable");

        var fileSystem = new MockFileSystem();
        var config = SqliteConfig(db.ConnectionString);
        config.Connections![0].Tables!.Add(new GeneratorConfig.Table
        {
            Tablename = "MyTable",
            Identifier = "Old",
            Module = "OldModule"
        });
        AddConfig(fileSystem, config);
        var console = new MockGeneratorConsole();
        var command = CreateCommand(fileSystem, console,
            ["--cnk", "Default", "--tbl", "MyTable", "--mod", "Test",
             "--cls", "MyTable", "--pms", "Test:Permission", "--wtg", "R"]);

        var result = command.Run();

        Assert.Equal(ExitCodes.Success, result);
        Assert.Contains("Test", fileSystem.ReadAllText(projectDir + "/sergen.json"),
            StringComparison.Ordinal);
    }

    [Fact]
    public void Generate_Ignores_ApplicationMetadataErrors_WhenAssemblyInvalid()
    {
        using var db = new MemorySqliteDb("MyTable");

        var fileSystem = new MockFileSystem();
        AddConfig(fileSystem, SqliteConfig(db.ConnectionString));
        var console = new MockGeneratorConsole();
        var command = CreateCommand(fileSystem, console,
            ["--cnk", "Default", "--tbl", "MyTable", "--mod", "Test",
             "--cls", "MyTable", "--pms", "Test:Permission", "--wtg", "R"],
            assemblyList: ["/no/such/assembly.dll"]);

        var result = command.Run();

        Assert.Equal(ExitCodes.Success, result);
    }
}
