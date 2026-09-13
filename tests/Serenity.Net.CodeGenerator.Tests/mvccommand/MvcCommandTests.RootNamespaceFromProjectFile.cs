namespace Serenity.CodeGenerator;

public partial class MvcCommandTests
{
    [Fact]
    public void Uses_RootNamespaceFromProjectFile_AndStripsWebSuffix()
    {
        var csproj = """
            <Project Sdk="Microsoft.NET.Sdk.Web">
                <PropertyGroup>
                    <RootNamespace>Acme.Web</RootNamespace>
                </PropertyGroup>
            </Project>
            """;

        var command = CreateCommand(new GeneratorConfig(),
            ["Modules/Admin/UserIndex.cshtml"], out var fileSystem, csproj);

        Assert.Equal(ExitCodes.Success, command.Run());

        Assert.Contains("namespace Acme", ReadMvc(fileSystem));
        Assert.DoesNotContain("namespace Acme.Web", ReadMvc(fileSystem));
    }
}
