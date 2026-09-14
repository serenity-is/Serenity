namespace Serenity.CodeGeneration;

public partial class ClientTypesGeneratorTests
{
    private static List<GeneratedSource> RunTypes(params ExternalType[] types)
    {
        var generator = new ClientTypesGenerator
        {
            OmitComments = true
        };
        generator.RootNamespaces.Add("MyProject");

        foreach (var type in types)
            generator.AddTSType(type);

        return generator.Run();
    }

    private static ExternalType Editor(string name, params ExternalMember[] members)
    {
        return new ExternalType
        {
            Name = name,
            Namespace = "MyProject",
            SourceFile = "/root/Modules/" + name + ".ts",
            Module = "/Modules/" + name,
            BaseType = "@serenity-is/corelib:Widget",
            Attributes =
            [
                new ExternalAttribute
                {
                    Type = "Serenity.Decorators.registerEditor",
                    Arguments = [new ExternalArgument { Value = "MyProject." + name }]
                }
            ],
            Methods = [.. members.OfType<ExternalMethod>()],
            Fields = [.. members.OfType<ExternalMember>().Where(x => x is not ExternalMethod)]
        };
    }

    [Fact]
    public void Editor_Option_From_Set_Method_Strips_Prefix()
    {
        var options = new ExternalType
        {
            Name = "TestEditorOptions",
            Namespace = "MyProject",
            IsInterface = true,
            Methods =
            [
                new ExternalMethod
                {
                    Name = "set_multiple",
                    Arguments = [new ExternalArgument { Type = "boolean" }]
                }
            ]
        };

        var editor = Editor("TestEditor",
            new ExternalMethod
            {
                Name = "$ctor",
                IsConstructor = true,
                Arguments = [new ExternalArgument { Type = "MyProject.TestEditorOptions" }]
            });

        var files = RunTypes(options, editor);
        var text = Read(files, "TestEditorAttribute.cs");
        Assert.Contains("public bool Multiple", text);
        Assert.Contains("GetOption<bool>(\"multiple\")", text);
    }

    [Fact]
    public void Formatter_From_FormatterTypeInfo_Field()
    {
        var formatter = new ExternalType
        {
            Name = "TestFormatter",
            SourceFile = "/root/Modules/TestFormatter.ts",
            Module = "/Modules/TestFormatter",
            Fields =
            [
                new ExternalMember
                {
                    Name = "[Symbol.typeInfo]",
                    IsStatic = true,
                    Type = "FormatterTypeInfo",
                    Value = "MyProject."
                }
            ]
        };

        var files = RunTypes(formatter);
        var text = Read(files, "TestFormatterAttribute.cs");
        Assert.Contains("public partial class TestFormatterAttribute : CustomFormatterAttribute", text);
        Assert.Contains("public const string Key = \"MyProject.TestFormatter\";", text);
    }

    [Fact]
    public void Editor_Options_From_Generic_Parameter_Extends()
    {
        var options = new ExternalType
        {
            Name = "TestEditorOptions",
            Namespace = "MyProject",
            IsInterface = true,
            Fields = [new ExternalMember { Name = "cols", Type = "number" }]
        };

        var editor = new ExternalType
        {
            Name = "TestEditor",
            Namespace = "MyProject",
            SourceFile = "/root/Modules/TestEditor.ts",
            Module = "/Modules/TestEditor",
            BaseType = "@serenity-is/corelib:Widget",
            Attributes =
            [
                new ExternalAttribute
                {
                    Type = "Serenity.Decorators.registerEditor",
                    Arguments = [new ExternalArgument { Value = "MyProject.TestEditor" }]
                }
            ],
            GenericParameters =
            [
                new ExternalGenericParameter { Name = "P", Default = "MyProject.TestEditorOptions" }
            ]
        };

        var files = RunTypes(options, editor);
        var text = Read(files, "TestEditorAttribute.cs");
        Assert.Contains("public double Cols", text);
    }

    [Fact]
    public void BasicType_Skips_All_Complex_Member_Types()
    {
        var basicType = new ExternalType
        {
            Name = "TestOptions",
            Namespace = "MyProject",
            SourceFile = "/root/Modules/TestOptions.ts",
            Module = "/Modules/TestOptions",
            IsInterface = true,
            Interfaces = ["TransformInclude"],
            Fields =
            [
                new ExternalMember { Name = "funcProp", Type = "System.Func`1" },
                new ExternalMember { Name = "delegateProp", Type = "System.Delegate" },
                new ExternalMember { Name = "functionProp", Type = "Function" }
            ]
        };

        var files = RunTypes(basicType);
        var text = Read(files, "TestOptions.generated.cs");
        Assert.Equal("""
            namespace MyProject
            {
                public partial class TestOptions
                {
                }
            }
            """.ReplaceLineEndings(), text);
    }
}
