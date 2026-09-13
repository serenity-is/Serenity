namespace Serenity.CodeGenerator;

public partial class MvcCommandTests
{
    [Fact]
    public void Uses_ConfiguredSearchViewPaths()
    {
        var command = CreateCommand(new GeneratorConfig
        {
            MVC = new() { SearchViewPaths = ["AppModules/"] }
        }, [
            "AppModules/Admin/Index.cshtml",
            "Modules/Other/Index.cshtml"
        ], out var fileSystem);

        Assert.Equal(ExitCodes.Success, command.Run());

        var mvc = ReadMvc(fileSystem);
        Assert.Contains("public static partial class AppModules", mvc);
        Assert.DoesNotContain("Other", mvc);
    }
}
