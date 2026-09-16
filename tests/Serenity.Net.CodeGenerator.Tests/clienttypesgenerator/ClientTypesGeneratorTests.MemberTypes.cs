namespace Serenity.CodeGeneration;

public partial class ClientTypesGeneratorTests
{
    [Fact]
    public void Option_Member_Type_Names_From_System_Types()
    {
        var editor = Editor("TestEditor",
            new ExternalMethod
            {
                Name = "$ctor",
                IsConstructor = true,
                Arguments = [new ExternalArgument { Type = "MyProject.TestEditorOptions" }]
            });

        var options = new ExternalType
        {
            Name = "TestEditorOptions",
            Namespace = "MyProject",
            IsInterface = true,
            Fields =
            [
                new ExternalMember { Name = "intProp", Type = "System.Nullable`1[System.Int32]" },
                new ExternalMember { Name = "dateProp", Type = "System.DateTime" },
                new ExternalMember { Name = "guidProp", Type = "System.Guid" },
                new ExternalMember { Name = "unknownProp", Type = "Some.UnknownType" }
            ]
        };

        var files = RunTypes(options, editor);
        var text = Read(files, "TestEditorAttribute.cs");
        Assert.Contains("public int IntProp", text);
        Assert.Contains("public DateTime DateProp", text);
        Assert.Contains("public Guid GuidProp", text);
        Assert.Contains("public object UnknownProp", text);
    }

    [Theory]
    [InlineData("enable")]
    [InlineData("annotations")]
    public void Editor_Options_Use_Nullable_Ref_Types_When_Enabled(string nullableProp)
    {
        var files = Generate(nullableProp, true, ("Modules/TestEditor.ts", /*lang=typescript*/ """
            import { Decorators, Widget } from "@serenity-is/corelib"

            @Decorators.registerEditor('MyProject.MyTest.TestEditor')
            export class TestEditor extends Widget {
                constructor(props: { name?: string }) {
                }
            }
            """));

        var text = Read(files, "MyTest.TestEditorAttribute.cs");
        Assert.Contains("public string? Name", text);
        Assert.Contains("GetOption<string?>(\"name\")", text);
    }

    [Fact]
    public void Type_In_Serenity_Namespace_Maps_To_ComponentModel()
    {
        var files = Generate(null, true, ("Modules/TestOptions.ts", /*lang=typescript*/ """
            import { TransformInclude } from "@serenity-is/corelib"

            namespace Serenity {
                export interface TestOptions extends TransformInclude {
                    token?: string;
                }
            }
            """));

        var text = Read(files, "ComponentModel.TestOptions.generated.cs");
        Assert.Equal("""
            namespace Serenity.ComponentModel
            {
                public partial class TestOptions
                {
                    public string token { get; set; }
                }
            }
            """.ReplaceLineEndings(), text);
    }
}
