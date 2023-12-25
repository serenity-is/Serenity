using Serenity.CodeGenerator;

namespace Serenity.Tests.CodeGenerator;

public class RestoreCommandTests
{
    private readonly MockFileSystem fileSystem = new();
    private readonly MockGeneratorConsole console = new();
    private const string testProject = "Test.csproj";
    private ProjectFileInfo ProjectFactory(string s = testProject, Func<string, string> prop = null)
        => new(fileSystem, s, prop, console.Error);

    [Fact]
    public void Throws_ArgumentNull_When_FileSystem_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RestoreCommand(null, new MockGeneratorConsole())
            {
                BuildSystem = new MockBuildProjectSystem(fileSystem)
            });
    }

    [Fact]
    public void Throws_ArgumentNull_When_BuildSystem_IsNull()
    {
        var project = ProjectFactory();
        Assert.Throws<ArgumentNullException>(() =>
            new RestoreCommand(project, new MockGeneratorConsole())
            {
                BuildSystem = null
            }.Run());
    }

    [Fact]
    public void Returns_ProjectNotFound_ExitCode_When_CsProj_Not_Found()
    {
        var project = ProjectFactory("nonexisting.csproj");
        var command = new RestoreCommand(project, console)
        {
            BuildSystem = new MockBuildProjectSystem(project.FileSystem)
        };
        var exitCode = command.Run();
        Assert.Equal(ExitCodes.ProjectNotFound, exitCode);
    }

    [Fact]
    public void Returns_Cant_Determine_Packages_Dir_When_Nuget_Packages_Dir_Not_Found()
    {
        fileSystem.WriteAllText(testProject, "A");
        var project = ProjectFactory();
        var command = new RestoreCommand(project, console)
        {
            BuildSystem = new MockBuildProjectSystem(fileSystem)
        };
        var exitCode = command.Run();
        Assert.Equal(ExitCodes.CantDeterminePackagesDir, exitCode);
    }
}
