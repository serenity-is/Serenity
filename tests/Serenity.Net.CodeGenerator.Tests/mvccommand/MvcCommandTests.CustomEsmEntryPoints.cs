namespace Serenity.CodeGenerator;

public partial class MvcCommandTests
{
    [Fact]
    public void Uses_ConfiguredEsmEntryPoints()
    {
        var command = CreateCommand(new GeneratorConfig
        {
            TSBuild = new() { EntryPoints = ["Modules/**/*.ts"] }
        }, ["Modules/Admin/Helper.ts"], out var fileSystem);

        Assert.Equal(ExitCodes.Success, command.Run());

        Assert.Contains("Helper", ReadEsm(fileSystem));
    }
}
