using Serenity.CodeGenerator;

namespace Serenity.Tests.CodeGenerator;

public class RestoreCommandTests
{
    [Fact]
    public void Throws_ArgumentNull_When_FileSystem_IsNull()
    {
        var fileSystem = new MockFileSystem();
        var projectSystem = new MockBuildProjectSystem(fileSystem);
        Assert.Throws<ArgumentNullException>(() => new RestoreCommand(null, projectSystem));
    }

    [Fact]
    public void Throws_ArgumentNull_When_ProjectSystem_IsNull()
    {
        var project = new ProjectFileInfo(new MockFileSystem(), "test.csproj", []);
        var projectSystem = new MockBuildProjectSystem(project.FileSystem);
        Assert.Throws<ArgumentNullException>(() => new RestoreCommand(project, null));
    }

    [Fact]
    public void Returns_ProjectNotFound_ExitCode_When_CsProj_Not_Found()
    {
        var project = new ProjectFileInfo(new MockFileSystem(), "nonexisting.csproj", []);
        var projectSystem = new MockBuildProjectSystem(project.FileSystem);
        var command = new RestoreCommand(project, projectSystem);
        var exitCode = command.Run();
        Assert.Equal(ExitCodes.ProjectNotFound, exitCode);
    }

    [Fact]
    public void Returns_Cant_Determine_Packages_Dir_When_Nuget_Packages_Dir_Not_Found()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.WriteAllText("Test.csproj", "A");
        var project = new ProjectFileInfo(fileSystem, "Test.csproj", []);
        var projectSystem = new MockBuildProjectSystem(fileSystem);
        var command = new RestoreCommand(project, projectSystem);
        var exitCode = command.Run();
        Assert.Equal(ExitCodes.CantDeterminePackagesDir, exitCode);
    }
}
