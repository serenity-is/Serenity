using Serenity.CodeGeneration;

namespace Serenity.CodeGenerator;

public partial class ClientTypesCommandTests
{
    const string projectDir = "/Repos/MyTest.Web/";
    const string projectFile = projectDir + "MyTest.Web.csproj";
    const string defaultCsproj = "<Project Sdk=\"Microsoft.NET.Sdk.Web\"></Project>";

    private static (ClientTypesCommand Command, MockFileSystem FileSystem, MockGeneratorConsole Console) Create(
        GeneratorConfig config, Func<string, string> getProperty = null, params ExternalType[] tsTypes)
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(projectDir);
        fileSystem.AddFile(projectFile, defaultCsproj);
        fileSystem.AddFile(fileSystem.Combine(projectDir, "sergen.json"),
            (config ?? new GeneratorConfig() { RootNamespace = "MyProject" }).SaveToJson());
        var project = new ProjectFileInfo(fileSystem, projectFile,
            getProperty is null ? null : n => getProperty(n), null);
        var console = new MockGeneratorConsole();
        var command = new ClientTypesCommand(project, console)
        {
            TsTypes = [.. tsTypes]
        };
        return (command, fileSystem, console);
    }

    [Theory]
    [InlineData("enable", "#nullable enable")]
    [InlineData("annotations", "#nullable enable annotations")]
    public void Run_Generates_Client_Types_In_Default_OutDir(string nullableProp, string expectedDirective)
    {
        var config = new GeneratorConfig()
        {
            RootNamespace = "MyProject",
            EndOfLine = "lf"
        };

        var (command, fileSystem, console) = Create(config,
            getProperty: name => name switch
            {
                "GlobalUsings" => "My.A=My.Alias;My.B",
                "Nullable" => nullableProp,
                _ => null
            },
            tsTypes: new ExternalType
            {
                Name = "TestOptions",
                Namespace = "MyProject",
                IsInterface = true,
                Interfaces = ["TransformInclude"],
                Fields = [new() { Name = "funcProp", Type = "string" }]
            });

        var result = command.Run();

        Assert.Equal(ExitCodes.Success, result);
        var generatedFile = Assert.Single(fileSystem.GetFiles(projectDir, "*.cs", recursive: true));
        Assert.Contains("TestOptions.generated.cs", generatedFile);
        Assert.Contains("Imports/ClientTypes", generatedFile.Replace('\\', '/'));
        var text = fileSystem.ReadAllText(generatedFile);

        Assert.Contains(expectedDirective, text);
        Assert.Contains("public partial class TestOptions", text);
        Assert.Contains("public string? funcProp", text);
    }

    [Fact]
    public void Run_With_ParseGlobalUsings_False_Generates_File_Scoped_Namespace()
    {
        var config = new GeneratorConfig()
        {
            RootNamespace = "MyProject",
            EndOfLine = "lf",
            FileScopedNamespaces = true,
            ClientTypes = new() { OutDir = "Imports/OtherTypes" }
        };

        var (command, fileSystem, console) = Create(config,
            getProperty: name => name switch
            {
                "GlobalUsings" => "My.A=My.Alias",
                _ => null
            },
            tsTypes: new ExternalType
            {
                Name = "TestOptions",
                Namespace = "MyProject",
                IsInterface = true,
                Interfaces = ["TransformInclude"]
            });

        var result = command.Run();

        Assert.Equal(ExitCodes.Success, result);
        var generatedFile = Assert.Single(fileSystem.GetFiles(projectDir, "*.cs", recursive: true));
        Assert.Contains("Imports/OtherTypes", generatedFile.Replace('\\', '/'));
        var text = fileSystem.ReadAllText(generatedFile);
        Assert.Contains("namespace MyProject;", text);
        Assert.Contains(console.WriteCalls, c =>
            c.type == MockGeneratorConsole.CallType.WriteLine &&
            c.message.Contains("Transforming Client Types for MyTest.Web"));
    }
}
