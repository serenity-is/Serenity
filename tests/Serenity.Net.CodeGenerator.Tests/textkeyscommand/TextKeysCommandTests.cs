using Serenity.CodeGeneration;
using Serenity.Testing;

namespace Serenity.CodeGenerator;

public class TextKeysCommandTests
{
    private const string ProjectDir = "/app/My.Web/";
    private const string ProjectFile = ProjectDir + "My.Web.csproj";

    private const string Template = """
        {
          "Site.Layout.Logout": "Logout",
          "Site.Layout.Login": "Login"
        }
        """;

    private sealed class MockProjectFileInfo(MockFileSystem fileSystem, bool usesSourceGenerators)
        : IProjectFileInfo
    {
        public string ProjectFile => TextKeysCommandTests.ProjectFile;
        public IFileSystem FileSystem => fileSystem;

        public string[]? GetAssemblyList(string[]? configured) => throw new NotSupportedException();
        public string? GetAssemblyName() => throw new NotSupportedException();
        public string? GetDotNetSergen() => throw new NotSupportedException();
        public string? GetEsmAssetBasePath() => throw new NotSupportedException();
        public IDictionary<string, string?> GetGlobalUsings() => new Dictionary<string, string?>();
        public string? GetNullable() => throw new NotSupportedException();
        public string? GetOutDir() => throw new NotSupportedException();
        public string? GetRootNamespace() => "My";
        public Version? GetSerenityWebVersion() => throw new NotSupportedException();
        public string? GetTargetFramework() => throw new NotSupportedException();
        public bool UsesSourceGenerators() => usesSourceGenerators;
    }

    private static (MockFileSystem FileSystem, MockGeneratorConsole Console, string OutFile) Create(
        bool usesSourceGenerators)
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(ProjectDir);
        var template = fileSystem.Combine(ProjectDir, "texts", "resources", "site.texts.__.json");
        fileSystem.CreateDirectory(fileSystem.GetDirectoryName(template));
        fileSystem.WriteAllText(template, Template);
        fileSystem.WriteAllText(fileSystem.Combine(ProjectDir, "sergen.json"),
            new GeneratorConfig { MVC = new GeneratorConfig.MVCConfig { AsNamespace = true } }.SaveToJson());
        return (fileSystem, new MockGeneratorConsole(),
            fileSystem.Combine(ProjectDir, "Imports", "MVC", TextKeysGenerator.FileName));
    }

    [Fact]
    public void Writes_The_File_Of_A_Project_That_Does_Not_Use_Source_Generators()
    {
        var (fileSystem, console, outFile) = Create(usesSourceGenerators: false);

        new TextKeysCommand(new MockProjectFileInfo(fileSystem, false), console).Run();

        Assert.True(fileSystem.FileExists(outFile));
    }

    [Fact]
    public void Does_Not_Write_The_File_That_The_Source_Generators_Of_The_Project_Write()
    {
        var (fileSystem, console, outFile) = Create(usesSourceGenerators: true);

        new TextKeysCommand(new MockProjectFileInfo(fileSystem, true), console).Run();

        // the file would be a duplicate of the one that the generators add to the compilation, and the two would drift
        Assert.False(fileSystem.FileExists(outFile));
        Assert.Contains(console.WriteCalls, x => x.message == Texts.TextKeysGeneratedOnBuild);
    }

    [Fact]
    public void Writes_The_File_Of_A_Project_That_Uses_Source_Generators_When_Forced()
    {
        var (fileSystem, console, outFile) = Create(usesSourceGenerators: true);

        // the mvc and transform commands of a project that uses the generators run only when they are forced, and the
        // message that tells the user to force them would be the one that they already did otherwise
        new TextKeysCommand(new MockProjectFileInfo(fileSystem, true), console) { Force = true }.Run();

        Assert.True(fileSystem.FileExists(outFile));
        Assert.DoesNotContain(console.WriteCalls, x => x.message == Texts.TextKeysGeneratedOnBuild);
    }
}
