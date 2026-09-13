namespace Serenity.CodeGenerator;

public partial class MvcCommandTests
{
    [Fact]
    public void Uses_Lf_EndOfLine()
    {
        var command = CreateCommand(new GeneratorConfig
        {
            EndOfLine = "lf"
        }, ["Modules/Admin/UserIndex.cshtml"], out var fileSystem);

        Assert.Equal(ExitCodes.Success, command.Run());

        Assert.DoesNotContain("\r", ReadMvc(fileSystem));
    }
}
