namespace Serenity.CodeGenerator;

public partial class MvcCommandTests
{
    [Fact]
    public void Uses_RootNamespaceFromConfig()
    {
        var command = CreateCommand(new GeneratorConfig
        {
            RootNamespace = "MyCompany.MyProduct"
        }, ["Modules/Admin/UserIndex.cshtml"], out var fileSystem);

        Assert.Equal(ExitCodes.Success, command.Run());

        Assert.Contains("namespace MyCompany.MyProduct", ReadMvc(fileSystem));
    }
}
