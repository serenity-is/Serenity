using ServerTypingsTest.PermissionKeys;

namespace Serenity.Tests.CodeGenerator;

public partial class ServerTypingsGeneratorTests
{
    [Theory]
    [InlineData(
        typeof(PermissionKeysSample1Depth1),
@"export namespace PermissionKeysSample1Depth1 {
    export const Security = ""Administration:Security"";
}")]
    [InlineData(
        typeof(PermissionKeysSample2Depth1),
@"export namespace PermissionKeysSample2Depth1 {
    export const Security = ""Administration:Security"";
    
    export namespace PermissionKeysSample2Depth2 {
        export const Security= ""Administration:Security"";
    }
}")]
    [InlineData(
        typeof(PermissionKeysSample3Depth1),
@"export namespace PermissionKeysSample3Depth1 {
    export const Security = ""Administration:Security"";
        
    export namespace PermissionKeysSample3Depth2 {
        export const Security =""Administration:Security"";
            
        export namespace PermissionKeysSample3Depth3 {
            export const Security = ""Administration:Security"";
        }
    }
}")]

    public void PermissionKeys_Modules_Generated_Properly(Type classType, string expected)
    {
        var generator = CreateGeneratorModules(classType);
        var result = generator.Run();
        var code = Assert.Single(result, x => x.Filename == $"PermissionKeys/{classType.Name}.ts").Text;

        code = NormalizeTS(code);
        expected = NormalizeTS(expected);

        Assert.Equal(expected, code);
        
    }
}


