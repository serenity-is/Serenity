namespace Serenity.CodeGenerator;

public partial class MvcCommandTests
{
    [Fact]
    public void Skips_NonexistentSearchViewPaths()
    {
        var command = CreateCommand(new GeneratorConfig
        {
            MVC = new() { SearchViewPaths = ["Missing/", "Modules/"] }
        }, ["Modules/Admin/Index.cshtml"], out var fileSystem);

        Assert.Equal(ExitCodes.Success, command.Run());

        Assert.Contains("public static partial class Admin", ReadMvc(fileSystem));
    }
}
