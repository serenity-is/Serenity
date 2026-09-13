namespace Serenity.CodeGenerator;

public partial class MvcCommandTests
{
    [Fact]
    public void Uses_FileScopedNamespaces()
    {
        var command = CreateCommand(new GeneratorConfig
        {
            FileScopedNamespaces = true
        }, ["Modules/Admin/UserIndex.cshtml"], out var fileSystem);

        Assert.Equal(ExitCodes.Success, command.Run());

        Assert.Contains("namespace MyTest;", ReadMvc(fileSystem));
    }
}
