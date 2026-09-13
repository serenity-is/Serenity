namespace Serenity.CodeGenerator;

public partial class MvcCommandTests
{
    [Fact]
    public void Uses_EsmAssetBasePath_FromProjectFile()
    {
        var csproj = """
            <Project Sdk="Microsoft.NET.Sdk.Web">
                <PropertyGroup>
                    <ESMAssetBasePath>/assets/esm</ESMAssetBasePath>
                </PropertyGroup>
            </Project>
            """;

        var command = CreateCommand(new GeneratorConfig(),
            ["Modules/Admin/TestPage.ts"], out var fileSystem, csproj);

        Assert.Equal(ExitCodes.Success, command.Run());

        Assert.Contains("public const string TestPage = \"~/assets/esm/Modules/Admin/TestPage.js\";",
            ReadEsm(fileSystem));
    }
}
