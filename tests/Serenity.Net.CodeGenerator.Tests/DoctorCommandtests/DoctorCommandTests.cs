namespace Serenity.CodeGenerator;

public partial class DoctorCommandTests
{
    [Fact]
    public void Prints_Project_File()
    {
        var (command, env) = CreateCommand(projectFile: @"/MyProjectName.csproj");
        command.Run();
        Assert.Contains(env.Console.WriteCalls, x =>
            x.data is ConsoleColor.Cyan &&
            x.message.Contains("Project File:"));
        Assert.Contains(env.Console.WriteCalls, x =>
            x.data is null &&
            x.message.Contains("MyProjectName.csproj"));
    }

    [Fact]
    public void Checks_If_Project_FileName_IsPascalCase()
    {
        var (command, env) = CreateCommand(projectFile: @"/MyTest.Web/myTest.Web.csproj");
        command.Run();
        Assert.Contains(env.Console.WriteCalls, x => 
            x.data is ConsoleColor.Red &&
            x.message.Contains("Project filename should start with a capital letter") );
    }

    [Fact]
    public void DoesNotError_If_Project_FileName_IsPascalCase()
    {
        var (command, env) = CreateCommand(projectFile: @"/MyTest.Web/MyTest.Web.csproj");
        command.Run();
        Assert.DoesNotContain(env.Console.WriteCalls, x =>
            x.message.Contains("Project filename should start with a capital letter"));
    }

    private class MockEnv
    {
        public string ProjectFile { get; set; }
        public string ProjectDirectory => FileSystem.GetDirectoryName(ProjectFile); 
        public ProjectFileInfo Project { get; set; }
        public MockFileSystem FileSystem { get; set; } = new();
        public MockGeneratorConsole Console { get; set; } = new();
        public MockProcessExecutor Executor = new();
    }


    private (DoctorCommand, MockEnv) CreateCommand(
        string projectFile = @"/Repos/MyTest.Web/MyTest.Web.csproj")
    {
        var environment = new MockEnv
        {
            FileSystem = new MockFileSystem(),
            ProjectFile = projectFile
        };

        environment.Project = new ProjectFileInfo(environment.FileSystem, environment.ProjectFile);
        environment.FileSystem.AddFile(environment.ProjectFile, "<Project Sdk=\"Microsoft.NET.Sdk.Web\"></Project>");
        return (new DoctorCommand(environment.Project, environment.Console, environment.Executor), environment);
    }

}
