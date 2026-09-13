namespace Serenity.CodeGenerator;

public partial class MvcCommandTests
{
    const string projectDir = @"/Repos/MyTest.Web/";
    const string projectFile = projectDir + "MyTest.Web.csproj";
    const string defaultCsproj = "<Project Sdk=\"Microsoft.NET.Sdk.Web\"></Project>";
    const string defaultOutDir = "Imports/MVC";

    MvcCommand CreateCommand(GeneratorConfig config, string[] files,
        out MockFileSystem fileSystem, string csproj = null)
    {
        fileSystem = new MockFileSystem();
        var project = new ProjectFileInfo(fileSystem, projectFile);
        var directory = fileSystem.GetDirectoryName(project.ProjectFile);
        fileSystem.AddFile(project.ProjectFile, csproj ?? defaultCsproj);
        fileSystem.AddFile(fileSystem.Combine(directory, "sergen.json"),
            (config ?? new GeneratorConfig()).SaveToJson());
        var command = new MvcCommand(project, new MockGeneratorConsole());
        foreach (var file in files)
            fileSystem.AddFile(fileSystem.Combine(directory, PathHelper.ToPath(file)), "");
        return command;
    }

    static string ReadMvc(MockFileSystem fileSystem, string outDir = defaultOutDir)
    {
        return fileSystem.ReadAllText(projectDir + outDir + "/MVC.cs");
    }

    static string ReadEsm(MockFileSystem fileSystem, string outDir = defaultOutDir)
    {
        return fileSystem.ReadAllText(projectDir + outDir + "/ESM.cs");
    }
}
