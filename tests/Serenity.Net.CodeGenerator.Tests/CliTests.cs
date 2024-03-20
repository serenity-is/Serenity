using Serenity.CodeGenerator;
using Xunit.Sdk;
using CallType = Serenity.Tests.MockGeneratorConsole.CallType;

namespace Serenity.Tests.CodeGenerator;

public partial class CliTests
{
    const string TestProject = "Test.csproj";

    private readonly MockFileSystem fileSystem = new();
    private readonly MockGeneratorConsole console = new();
    private readonly List<BaseGeneratorCommand> runCalls = [];

    private class ForAllCommandsAttribute : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            return [
                [Cli.CommandKeys.Generate],
                [Cli.CommandKeys.Mvc],
                [Cli.CommandKeys.ServerTypings],
                [Cli.CommandKeys.ClientTypes],
                [Cli.CommandKeys.MvcAndClientTypes],
                [Cli.CommandKeys.Restore],
                [Cli.CommandKeys.Transform]
            ];
        }
    }

    public CliTests()
    {
        fileSystem.AddFile(TestProject, new("<Project></Project>"));
    }

    private Cli NewCli(Func<BaseGeneratorCommand, ExitCodes> runCommand = null)
    {
        var cli = new Cli(fileSystem, console)
        {
            RunCommandCallback = cmd =>
            {
                runCalls.Add(cmd);
                return runCommand?.Invoke(cmd) ?? ExitCodes.Success;
            }
        };
        return cli;
    }

    private ExitCodes RunCli(string[] arguments,
        Func<BaseGeneratorCommand, ExitCodes> runCommand = null)
    {
        return NewCli(runCommand).Run(new ArgumentReader(arguments));
    }

    [Fact]
    public void ThrowsArgumentNull_ForAnyNullArgument()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new Cli(null, console));

        Assert.Throws<ArgumentNullException>(() =>
            new Cli(fileSystem, null));
    }

    [InlineData("/?")]
    [InlineData("/help")]
    [InlineData("test", "--help")]
    [InlineData("mvc", "--help")]
    [Theory]
    public void WritesHelpAndExits_IfHelpSwitchIsProvided(params string[] arguments)
    {
        var exitCode = RunCli(arguments);
        Assert.Equal(ExitCodes.Help, exitCode);
        Assert.Empty(runCalls);
        Assert.Equal(CallType.Help, Assert.Single(console.WriteCalls).type);
    }

    [InlineData()]
    [InlineData("--projectrefs", "Test")]
    [InlineData("/prop", "A=B")]
    [InlineData("-p", "SomeProject.csproj")]
    [Theory]
    public void WritesHelpAndExits_IfNoCommand(params string[] arguments)
    {
        var exitCode = RunCli(arguments);
        Assert.Equal(ExitCodes.NoCommand, exitCode);
        Assert.Empty(runCalls);
        Assert.Equal(CallType.Help, Assert.Single(console.WriteCalls).type);
    }

    [ForAllCommands]
    [Theory]
    public void ExitsWith_NoProjectFiles_IfZeroProjectFiles_InCurrentDirectory(string command)
    {
        fileSystem.DeleteFile(TestProject);
        var exitCode = RunCli([command]);
        Assert.Equal(ExitCodes.NoProjectFiles, exitCode);
        Assert.Empty(runCalls);
        var (type, message, _) = Assert.Single(console.WriteCalls);
        Assert.Equal(CallType.Error, type);
        Assert.Equal(Texts.NoProjectFiles, message);
    }

    [ForAllCommands]
    [Theory]
    public void ExitsWith_MultipleProjectFiles_IfMoreThanOne_ProjectFiles_InCurrentDirectory(string command)
    {
        fileSystem.AddFile("TestProject2.csproj", new("<Project></Project>"));
        var exitCode = RunCli([command]);
        Assert.Equal(ExitCodes.MultipleProjectFiles, exitCode);
        Assert.Empty(runCalls);
        var (type, message, _) = Assert.Single(console.WriteCalls);
        Assert.Equal(CallType.Error, type);
        Assert.Equal(Texts.MultipleProjectFiles, message);
    }
}
