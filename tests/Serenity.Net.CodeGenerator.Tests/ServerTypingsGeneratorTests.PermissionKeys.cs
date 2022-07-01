using Serenity.CodeGeneration;
using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Web;
using Xunit;

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
        [InlineData(typeof(PermissionKeysSample1Depth1), new string[]
        {
            nameof(PermissionKeysSample1Depth1),
            nameof(PermissionKeysSample1Depth1.Security),
            PermissionKeysSample1Depth1.Security,
        })]
        [InlineData(typeof(PermissionKeysSample2Depth1),new string[]
        {
            nameof(PermissionKeysSample2Depth1),
            nameof(PermissionKeysSample2Depth1.Security),
            PermissionKeysSample2Depth1.Security,
            nameof(PermissionKeysSample2Depth1.PermissionKeysSample2Depth2),
            nameof(PermissionKeysSample2Depth1.PermissionKeysSample2Depth2.Security),
            PermissionKeysSample2Depth1.PermissionKeysSample2Depth2.Security,
        })]
        [InlineData(typeof(PermissionKeysSample3Depth1), new string[]
        {
            nameof(PermissionKeysSample3Depth1),
            nameof(PermissionKeysSample3Depth1.Security),
            PermissionKeysSample3Depth1.Security,
            nameof(PermissionKeysSample3Depth1.PermissionKeysSample3Depth2),
            nameof(PermissionKeysSample3Depth1.PermissionKeysSample3Depth2.Security),
            PermissionKeysSample3Depth1.PermissionKeysSample3Depth2.Security,
            nameof(PermissionKeysSample3Depth1.PermissionKeysSample3Depth2.PermissionKeysSample3Depth3),
            nameof(PermissionKeysSample3Depth1.PermissionKeysSample3Depth2.PermissionKeysSample3Depth3.Security),
            PermissionKeysSample3Depth1.PermissionKeysSample3Depth2.PermissionKeysSample3Depth3.Security,
        })]

        public void TestPermissionKeys(Type classType, string[] stringsToSearch)
        {
            var generator = CreateGenerator();
            var result = generator.Run();

            Assert.Contains($"Tests.CodeGenerator.{classType.Name}.ts", result.Keys);

            var code = result[$"Tests.CodeGenerator.{classType.Name}.ts"];

            code = NormalizeTS(code);

            foreach (var item in stringsToSearch)
            {
                Assert.Contains(item, code);
                code = code.Substring(code.IndexOf(item) + item.Length);
            }
        }
    }
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


