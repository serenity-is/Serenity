namespace Serenity.CodeGenerator;

public partial class MvcCommandTests
{
    [Fact]
    public void Uses_AsNamespace()
    {
        var command = CreateCommand(new GeneratorConfig
        {
            MVC = new() { AsNamespace = true }
        }, ["Modules/Admin/UserIndex.cshtml"], out var fileSystem);

        Assert.Equal(ExitCodes.Success, command.Run());

        var mvc = ReadMvc(fileSystem);
        Assert.Contains("namespace MyTest.MVC", mvc);
        Assert.Contains("public static partial class Views", mvc);
        Assert.DoesNotContain("public static partial class MVC", mvc);
    }
}
