using Serenity.CodeGeneration;

namespace Serenity.CodeGenerator;

public partial class ServerTypingsCommandTests
{
    const string projectDir = "/Repos/MyTest.Web/";
    const string projectFile = projectDir + "MyTest.Web.csproj";
    const string defaultCsproj = "<Project Sdk=\"Microsoft.NET.Sdk.Web\"></Project>";

    private static (ServerTypingsCommand Command, MockFileSystem FileSystem, MockGeneratorConsole Console) Create(
        GeneratorConfig config, Func<string, string> getProperty = null, ExternalType[] tsTypes = null)
    {
        var errors = new List<string>();
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(projectDir);
        fileSystem.AddFile(projectFile, defaultCsproj);
        fileSystem.AddFile(fileSystem.Combine(projectDir, "sergen.json"),
            (config ?? new GeneratorConfig() { RootNamespace = "MyProject" }).SaveToJson());
        var project = new ProjectFileInfo(fileSystem, projectFile,
            getProperty is null ? null : n => getProperty(n), error => errors.Add(error));
        var console = new MockGeneratorConsole();
        var command = new ServerTypingsCommand(project, console);
        if (tsTypes != null)
            command.TsTypes = [.. tsTypes];
        return (command, fileSystem, console);
    }

    [Fact]
    public void Run_Without_Assemblies_Reports_Error_And_Exits()
    {
        var (command, fileSystem, console) = Create(new GeneratorConfig()
        {
            ClientTypes = new() { OutDir = null },
            RootNamespace = "MyProject"
        }, getProperty: name => name switch
        {
            _ => null
        });

        var result = command.Run();

        Assert.NotEqual(ExitCodes.Success, result);
        Assert.Equal(ExitCodes.CantDetermineOutputAssemblies, result);
        Assert.Contains(console.WriteCalls, c =>
            c.type == MockGeneratorConsole.CallType.Error &&
            c.message.Contains("Can't determine the output assemblies"));
    }

    [Fact]
    public void Run_With_Configured_Assemblies_And_TsTypes_Generates_Server_Types()
    {
        var assemblyLocation = typeof(ServerTypingsCommandTests).Assembly.Location;
        var config = new GeneratorConfig()
        {
            RootNamespace = "MyProjectTests",
            EndOfLine = "lf",
            ServerTypings = new()
            {
                Assemblies = [assemblyLocation],
                LocalTexts = false,
                ModuleReExports = false,
                PreferRelativePaths = false
            }
        };

        var (command, fileSystem, console) = Create(config,
            tsTypes: [new() { Name = "Widget", Module = "@serenity-is/corelib" }]);
        fileSystem.CreateDirectory(fileSystem.GetDirectoryName(assemblyLocation));
        fileSystem.WriteAllText(assemblyLocation, "");

        var result = command.Run();

        Assert.Equal(ExitCodes.Success, result);
        Assert.True(fileSystem.DirectoryExists(projectDir + "Modules/ServerTypes"));
        Assert.NotEmpty(fileSystem.GetFiles(projectDir + "Modules/ServerTypes", "*.ts", recursive: true));
        Assert.Contains(console.WriteCalls, c =>
            c.type == MockGeneratorConsole.CallType.WriteLine &&
            c.message.Contains("Transforming Modular Server Types for MyTest.Web"));
    }
}
