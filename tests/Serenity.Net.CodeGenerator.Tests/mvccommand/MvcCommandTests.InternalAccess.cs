namespace Serenity.CodeGenerator;

public partial class MvcCommandTests
{
    [Fact]
    public void Uses_InternalAccess()
    {
        var command = CreateCommand(new GeneratorConfig
        {
            MVC = new() { InternalAccess = true }
        }, ["Modules/Admin/UserIndex.cshtml"], out var fileSystem);

        Assert.Equal(ExitCodes.Success, command.Run());

        Assert.Contains("internal static partial class MVC", ReadMvc(fileSystem));
    }
}
