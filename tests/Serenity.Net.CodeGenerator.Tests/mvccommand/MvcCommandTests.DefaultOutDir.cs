namespace Serenity.CodeGenerator;

public partial class MvcCommandTests
{
    [Fact]
    public void Writes_To_DefaultOutDir()
    {
        var command = CreateCommand(new GeneratorConfig(),
            ["Modules/Admin/UserIndex.cshtml"], out var fileSystem);

        Assert.Equal(ExitCodes.Success, command.Run());

        Assert.True(fileSystem.FileExists(projectDir + "Imports/MVC/MVC.cs"));
        Assert.True(fileSystem.FileExists(projectDir + "Imports/MVC/ESM.cs"));
    }
}
