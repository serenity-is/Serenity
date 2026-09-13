namespace Serenity.CodeGenerator;

public partial class MvcCommandTests
{
    [Fact]
    public void Uses_ConfiguredOutDir()
    {
        var command = CreateCommand(new GeneratorConfig
        {
            MVC = new() { OutDir = "Generated/Mvc" }
        }, ["Modules/Admin/UserIndex.cshtml"], out var fileSystem);

        Assert.Equal(ExitCodes.Success, command.Run());

        Assert.True(fileSystem.FileExists(projectDir + "Generated/Mvc/MVC.cs"));
        Assert.True(fileSystem.FileExists(projectDir + "Generated/Mvc/ESM.cs"));
    }
}
