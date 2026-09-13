namespace Serenity.CodeGenerator;

public partial class MvcCommandTests
{
    [Fact]
    public void Appends_To_DefaultEsmEntryPoints_When_FirstIsPlus()
    {
        var command = CreateCommand(new GeneratorConfig
        {
            TSBuild = new() { EntryPoints = ["+", "Views/**/*.ts"] }
        }, [
            "Modules/Admin/TestPage.ts",
            "Views/Home/HomePage.ts"
        ], out var fileSystem);

        Assert.Equal(ExitCodes.Success, command.Run());

        var esm = ReadEsm(fileSystem);
        Assert.Contains("TestPage", esm);
        Assert.Contains("HomePage", esm);
    }
}
