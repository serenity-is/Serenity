namespace Serenity.CodeGeneration;

public partial class ClientTypesGeneratorTests
{
    [Fact]
    public void Editor_Only_Includes_Fields_Marked_With_Option()
    {
        var files = Generate(null, true, ("Modules/TestEditor.ts", /*lang=typescript*/ """
            import { Decorators, Widget } from "@serenity-is/corelib"

            @Decorators.registerEditor('MyProject.MyTest.TestEditor')
            export class TestEditor extends Widget {
                @Decorators.option()
                customOption?: string;

                plainField?: string;
            }
            """));

        var text = Read(files, "MyTest.TestEditorAttribute.cs");
        Assert.Contains("public string CustomOption", text);
        Assert.DoesNotContain("PlainField", text);
    }

    [Fact]
    public void Editor_Translates_JS_Property_Names()
    {
        var files = Generate(null, true, ("Modules/TestEditor.ts", /*lang=typescript*/ """
            import { Decorators, Widget } from "@serenity-is/corelib"

            @Decorators.registerEditor('MyProject.MyTest.TestEditor')
            export class TestEditor extends Widget {
                constructor(props: { id?: number, customProp?: string, AlreadyPascal?: boolean }) {
                }
            }
            """));

        var text = Read(files, "MyTest.TestEditorAttribute.cs");
        Assert.Contains("public double ID", text);
        Assert.Contains("public string CustomProp", text);
        Assert.Contains("public bool AlreadyPascal", text);
    }
}
