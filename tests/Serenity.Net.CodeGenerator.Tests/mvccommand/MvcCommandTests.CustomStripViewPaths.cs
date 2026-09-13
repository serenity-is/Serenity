namespace Serenity.CodeGenerator;

public partial class MvcCommandTests
{
    [Fact]
    public void Uses_ConfiguredStripViewPaths()
    {
        var command = CreateCommand(new GeneratorConfig
        {
            MVC = new() { StripViewPaths = ["Modules/Home/"] }
        }, ["Modules/Home/Index.cshtml"], out var fileSystem);

        Assert.Equal(ExitCodes.Success, command.Run());

        var mvc = ReadMvc(fileSystem);
        Assert.Contains("public const string Index", mvc);
        Assert.DoesNotContain("public static partial class Home", mvc);
    }
}
