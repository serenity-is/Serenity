namespace Serenity.CodeGenerator;

public partial class MvcCommandTests
{
    [Fact]
    public void Uses_Crlf_EndOfLine()
    {
        var command = CreateCommand(new GeneratorConfig
        {
            EndOfLine = "crlf"
        }, ["Modules/Admin/UserIndex.cshtml"], out var fileSystem);

        Assert.Equal(ExitCodes.Success, command.Run());

        Assert.Contains("\r\n", ReadMvc(fileSystem));
    }
}
