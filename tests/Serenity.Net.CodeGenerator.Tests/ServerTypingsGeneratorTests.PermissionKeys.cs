using ServerTypingsTest.PermissionKeys;

namespace Serenity.Tests.CodeGenerator
{
    public partial class ServerTypingsGeneratorTests
    {
        private static string NormalizeCSharp(string input)
        {
            var result = string.Join("", CSharpMinifier.Minifier.Minify(
                input.Replace("\r", "").Replace("\n", ""),
                CSharpMinifier.MinificationOptions.Default)).Replace("\r", "").Replace("\n", "");
            return result;
        }

        private static string NormalizeTS(string input)
        {
            return NormalizeCSharp(input);
        }

        [Theory]
        [InlineData(
            typeof(PermissionKeysSample1Depth1),
@"namespace ServerTypingsTest.PermissionKeys {
    export namespace PermissionKeysSample1Depth1 {
        export const Security = ""Administration:Security"";
    }
}")]
        [InlineData(
            typeof(PermissionKeysSample2Depth1),
@"namespace ServerTypingsTest.PermissionKeys {
    export namespace PermissionKeysSample2Depth1 {
        export const Security = ""Administration:Security"";
    
        export namespace PermissionKeysSample2Depth2 {
            export const Security= ""Administration:Security"";
        }
    }
}")]
        [InlineData(
            typeof(PermissionKeysSample3Depth1),
@"namespace ServerTypingsTest.PermissionKeys {
    export namespace PermissionKeysSample3Depth1 {
        export const Security = ""Administration:Security"";
        
        export namespace PermissionKeysSample3Depth2 {
            export const Security =""Administration:Security"";
            
            export namespace PermissionKeysSample3Depth3 {
                export const Security = ""Administration:Security"";
            }
        }
    }
}")]

        public void PermissionKeys_Generated_Properly(Type classType, string expected)
        {
            var generator = CreateGenerator(classType);
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == $"PermissionKeys.{classType.Name}.ts").Text;

            code = NormalizeTS(code);
            expected = NormalizeTS(expected);

            Assert.Equal(expected, code);

        }
    }
}

namespace ServerTypingsTest.PermissionKeys
{
    [NestedPermissionKeys]
    public class PermissionKeysSample1Depth1
    {
        public const string Security = "Administration:Security";
    }

    [NestedPermissionKeys]
    public class PermissionKeysSample2Depth1
    {
        public const string Security = "Administration:Security";

        public class PermissionKeysSample2Depth2
        {
            public const string Security = "Administration:Security";
        }
    }

    [NestedPermissionKeys]
    public class PermissionKeysSample3Depth1
    {
        public const string Security = "Administration:Security";

        public class PermissionKeysSample3Depth2
        {
            public const string Security = "Administration:Security";

            public class PermissionKeysSample3Depth3
            {
                public const string Security = "Administration:Security";
            }
        }
    }
}


