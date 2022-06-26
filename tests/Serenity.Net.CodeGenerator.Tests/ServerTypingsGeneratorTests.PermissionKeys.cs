using Serenity.CodeGeneration;
using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Web;
using Xunit;

namespace Serenity.Tests.CodeGenerator
{
    public partial class ServerTypingsGeneratorTests
    {
        public string GetInsideOfTheNextScope(string code, int index = 0)
        {
            code = code.Substring(index);

            if(code.Contains("{") == false) return "";

            var startIndex = code.IndexOf("{");
            var endIndex = code.Length;

            var stack = new Stack<int>();
            stack.Push(startIndex);
            var searchIndex = startIndex + 1;

            while(stack.Count > 0 && searchIndex < code.Length)
            {
                var current = code[searchIndex];
                if(current == '{')
                {
                    stack.Push(searchIndex);
                }
                else if (current == '}')
                {
                    stack.Pop();
                    if(stack.Count == 0)
                    {
                        endIndex = searchIndex + 1 ; 
                    }
                }
                searchIndex++;
            }

            return code.Substring(startIndex, endIndex - startIndex);
        }

        [Fact]
        public void PermissionKeys_WithDepth1()
        {
            var generator = CreateGenerator();
            var result = generator.Run();

            var classType = typeof(PermissionKeysDepth1);
            var classProperties = classType.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(x => x.DeclaringType == classType) // dont get inherited properties
                .Where(x => x.IsLiteral); // only get const properties

            Assert.Contains($"Tests.CodeGenerator.{classType.Name}.ts", result.Keys);

            var code = result[$"Tests.CodeGenerator.{classType.Name}.ts"];

            Assert.Contains($"namespace {classType.Name}", code);
            
            var index = code.IndexOf($"namespace {classType.Name}");

            foreach (var property in classProperties)
            {
                Assert.Contains($"const {property.Name} = \"{property.GetRawConstantValue()}\";",code);
            }
        }

        [Fact]
        public void PermissionKeys_WithDepth2()
        {
            var generator = CreateGenerator();
            var result = generator.Run();

            var classType = typeof(PermissionKeysDepth2);
            var classProperties = classType.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(x => x.DeclaringType == classType) // dont get inherited properties
                .Where(x => x.IsLiteral); // only get const properties

            Assert.Contains($"Tests.CodeGenerator.{classType.Name}.ts", result.Keys);

            var code = result[$"Tests.CodeGenerator.{classType.Name}.ts"];

            Assert.Contains($"namespace {classType.Name}", code);

            var index = code.IndexOf($"namespace {classType.Name}");

            foreach (var property in classProperties)
            {
                Assert.Contains($"const {property.Name} = \"{property.GetRawConstantValue()}\";", code);
            }

            var insideOfDepth2 = GetInsideOfTheNextScope(code, index);

            var childClassType = typeof(PermissionKeysDepth2.PermissionKeysChild1);
            var childClassProperties = childClassType.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(x => x.DeclaringType == classType) // dont get inherited properties
                .Where(x => x.IsLiteral); // only get const properties

            Assert.Contains($"namespace {childClassType.Name}", insideOfDepth2);

            var insideOfChild = GetInsideOfTheNextScope(insideOfDepth2, insideOfDepth2.IndexOf($"namespace {childClassType.Name}"));

            foreach (var property in childClassProperties)
            {
                Assert.Contains($"const {property.Name} = \"{property.GetRawConstantValue()}\";", insideOfChild);
            }
        }


    }

    [NestedPermissionKeys]
    [DisplayName("Administration")]
    public class PermissionKeysDepth1
    {
        [Description("User, Role Management and Permissions")]
        public const string Security = "Administration:Security";

        [Description("Languages and Translations")]
        public const string Translation = "Administration:Translation";
    }

    [NestedPermissionKeys]
    [DisplayName("Administration")]
    public class PermissionKeysDepth2
    {
        [Description("User, Role Management and Permissions")]
        public const string Security = "Administration:Security";

        [Description("Languages and Translations")]
        public const string Translation = "Administration:Translation";

        public class PermissionKeysChild1
        {
            [Description("User, Role Management and Permissions")]
            public const string Security = "Administration:Security:2";

            [Description("Languages and Translations")]
            public const string Translation = "Administration:Translation:2";
        }
    }
}


