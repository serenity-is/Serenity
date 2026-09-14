using System.Diagnostics;

namespace Serenity.CodeGenerator;

public partial class ProjectFileInfoTests
{
    const string projectDir = "/app";
    const string projectFile = projectDir + "/My.Web.csproj";
    const string defaultCsproj = "<Project Sdk=\"Microsoft.NET.Sdk.Web\"></Project>";

    private static ProjectFileInfo Create(out MockFileSystem fileSystem, string csproj = null,
        Func<string, string> getProperty = null, Action<string> onError = null,
        Func<ProcessStartInfo, string> executeMSBuild = null, bool createProjectFile = true)
    {
        fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(projectDir);
        if (createProjectFile)
            fileSystem.WriteAllText(projectFile, csproj ?? defaultCsproj);

        var info = new ProjectFileInfo(fileSystem, projectFile,
            getProperty is null ? null : name => getProperty(name),
            onError);
        info.ExecuteMSBuild = executeMSBuild;
        return info;
    }

    private static string AddFile(MockFileSystem fileSystem, string path, string content = "")
    {
        var dir = fileSystem.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir) && !fileSystem.DirectoryExists(dir))
            fileSystem.CreateDirectory(dir);
        fileSystem.WriteAllText(path, content);
        return path;
    }
}
