using CallType = Serenity.Testing.MockGeneratorConsole.CallType;

namespace Serenity.CodeGenerator;

public partial class CliTests
{
    private static string MSBuildJson(string sourceGeneratorTransform = null, bool visibleProperty = true, string dotNetSergen = null)
    {
        var properties = new List<string>();
        if (sourceGeneratorTransform != null)
            properties.Add($"\"SourceGeneratorTransform\": \"{sourceGeneratorTransform}\"");
        if (dotNetSergen != null)
            properties.Add($"\"DotNetSergen\": {JSON.Stringify(dotNetSergen)}");

        // only the targets of Pro.Coder add this one, so a project without them does not have it
        var items = visibleProperty ? "{ \"Identity\": \"SourceGeneratorTransform\" }" : "";
        return "{ \"Properties\": { " + string.Join(", ", properties) + " }, " +
            "\"Items\": { \"CompilerVisibleProperty\": [ " + items + " ] } }";
    }

    private ExitCodes RunWithMSBuild(string msbuildJson, params string[] arguments)
    {
        var cli = NewCli();
        cli.SergenLocation = "/tools/sergen/sergen.dll";
        cli.ProjectFactory = (project, getProperty) => new ProjectFileInfo(fileSystem, project, getProperty)
        {
            ExecuteMSBuild = _ => msbuildJson
        };

        return cli.Run(new ArgumentReader(arguments));
    }

    [InlineData(Cli.CommandKeys.Mvc)]
    [InlineData(Cli.CommandKeys.ClientTypes)]
    [InlineData(Cli.CommandKeys.MvcAndClientTypes)]
    [InlineData(Cli.CommandKeys.Transform)]
    [InlineData(Cli.CommandAliases.Transform)]
    [Theory]
    public void RefusesToRunCommands_ThatDuplicateWhatTheSourceGeneratorsGenerate(string command)
    {
        var exitCode = RunWithMSBuild(MSBuildJson(), command);

        Assert.Equal(ExitCodes.GeneratedOnBuild, exitCode);
        Assert.Empty(runCalls);
        var (type, message, _) = Assert.Single(console.WriteCalls);
        Assert.Equal(CallType.Error, type);
        Assert.Equal(Texts.GeneratedOnBuild, message);
        Assert.Contains("dotnet build", message);
        Assert.Contains("--force", message);
    }

    [InlineData(Cli.CommandKeys.Mvc)]
    [InlineData(Cli.CommandKeys.ClientTypes)]
    [InlineData(Cli.CommandKeys.MvcAndClientTypes)]
    [Theory]
    public void RunsThoseCommands_WithTheForceSwitch(string command)
    {
        var exitCode = RunWithMSBuild(MSBuildJson(), command, "--force");

        Assert.Equal(ExitCodes.Success, exitCode);
        Assert.NotEmpty(runCalls);
    }

    [Fact]
    public void RunsThem_WhenTheSourceGeneratorsAreTurnedOff()
    {
        Assert.Equal(ExitCodes.Success, RunWithMSBuild(MSBuildJson(sourceGeneratorTransform: "false"), Cli.CommandKeys.Mvc));
        Assert.Single(runCalls);
    }

    [Fact]
    public void RunsThem_WhenTheProjectDoesNotUseProCoder()
    {
        Assert.Equal(ExitCodes.Success, RunWithMSBuild(MSBuildJson(visibleProperty: false), Cli.CommandKeys.Mvc));
        Assert.Single(runCalls);
    }

    [Fact]
    public void TakesThePropertyFromTheArguments_BeforeTheProject()
    {
        var exitCode = RunWithMSBuild(MSBuildJson(), Cli.CommandKeys.Mvc, "-prop:SourceGeneratorTransform=false");

        Assert.Equal(ExitCodes.Success, exitCode);
        Assert.Single(runCalls);
    }

    [Fact]
    public void DoesNotAskMSBuildForTheGenerateCommand()
    {
        var asked = false;
        var cli = NewCli();
        cli.ProjectFactory = (project, getProperty) => new ProjectFileInfo(fileSystem, project, getProperty)
        {
            ExecuteMSBuild = _ => { asked = true; return MSBuildJson(); }
        };

        Assert.Equal(ExitCodes.Success, cli.Run(new ArgumentReader([Cli.CommandKeys.Generate])));
        Assert.False(asked);
    }

    [Fact]
    public void WarnsWhenTheProjectRunsAnotherSergen()
    {
        var exitCode = RunWithMSBuild(MSBuildJson(sourceGeneratorTransform: "false", dotNetSergen: "dotnet /repo/artifacts/sergen.dll"),
            Cli.CommandKeys.Mvc);

        Assert.Equal(ExitCodes.Success, exitCode);
        var (type, message, _) = Assert.Single(console.WriteCalls);
        Assert.Equal(CallType.WriteLine, type);
        Assert.Contains("/repo/artifacts/sergen.dll", message);
        Assert.Contains("/tools/sergen/sergen.dll", message);
    }

    [InlineData("dotnet /tools/sergen/sergen.dll")]
    [InlineData("dotnet \"/tools/sergen/other/../sergen.dll\"")]
    [InlineData("dotnet /TOOLS/SERGEN/sergen.dll")]
    [InlineData("dotnet sergen")]
    [InlineData("sergen")]
    [InlineData("")]
    [Theory]
    public void DoesNotWarn_WhenItIsTheSergenOfTheProject_OrTheTool(string dotNetSergen)
    {
        RunWithMSBuild(MSBuildJson(sourceGeneratorTransform: "false", dotNetSergen: dotNetSergen), Cli.CommandKeys.Mvc);

        Assert.Empty(console.WriteCalls);
    }
}
