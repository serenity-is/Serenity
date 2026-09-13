namespace Serenity.CodeGenerator;

public partial class MvcCommandTests
{
    [Fact]
    public void Only_Includes_CshtmlFiles_Recursively()
    {
        var command = CreateCommand(new GeneratorConfig(), [
            "Modules/Admin/Index.cshtml",
            "Modules/Admin/Notes.txt",
            "Modules/Admin/Index.cshtml.bak"
        ], out var fileSystem);

        Assert.Equal(ExitCodes.Success, command.Run());

        var mvc = ReadMvc(fileSystem);
        Assert.Contains("public const string Index", mvc);
        Assert.DoesNotContain("Notes", mvc);
    }
}
