namespace Serenity.CodeGeneration;

public partial class ClientTypesGeneratorTests
{
    [Fact]
    public void Editor_Options_From_Node_And_Options_Arguments()
    {
        var files = Generate(null, true, ("Modules/TestEditor.ts", /*lang=typescript*/ """
            import { Decorators, Widget } from "@serenity-is/corelib"

            export interface TestEditorOptions {
                cols?: number;
            }

            @Decorators.registerEditor('MyProject.MyTest.TestEditor')
            export class TestEditor extends Widget {
                constructor(node: HTMLElement, options?: TestEditorOptions) {
                }
            }
            """));

        var text = Read(files, "MyTest.TestEditorAttribute.cs");
        Assert.Contains("public double Cols", text);
    }

    [Fact]
    public void Editor_Options_From_WidgetProps_Generic()
    {
        var files = Generate(null, true, ("Modules/TestEditor.ts", /*lang=typescript*/ """
            import { Decorators, Widget, WidgetProps } from "@serenity-is/corelib"

            export interface TestEditorOptions {
                cols?: number;
            }

            @Decorators.registerEditor('MyProject.MyTest.TestEditor')
            export class TestEditor extends Widget {
                constructor(props?: WidgetProps<TestEditorOptions>) {
                }
            }
            """));

        var text = Read(files, "MyTest.TestEditorAttribute.cs");
        Assert.Contains("public double Cols", text);
    }

    [Fact]
    public void Editor_Options_Inherited_From_Base_Class()
    {
        var files = Generate(null, true, ("Modules/TestEditor.ts", /*lang=typescript*/ """
            import { Decorators, TextAreaEditor } from "@serenity-is/corelib"

            @Decorators.registerEditor('MyProject.MyTest.TestEditor')
            export class TestEditor extends TextAreaEditor {
            }
            """));

        var text = Read(files, "MyTest.TestEditorAttribute.cs");
        Assert.Contains("public double Rows", text);
        Assert.Contains("public double Cols", text);
    }

    [Fact]
    public void Editor_Options_From_Intersection_Object_Literal()
    {
        var files = Generate(null, true, ("Modules/TestEditor.ts", /*lang=typescript*/ """
            import { Decorators, Widget } from "@serenity-is/corelib"

            @Decorators.registerEditor('MyProject.MyTest.TestEditor')
            export class TestEditor extends Widget {
                constructor(props: { rows: number } & { cols: number }) {
                }
            }
            """));

        var text = Read(files, "MyTest.TestEditorAttribute.cs");
        Assert.Contains("public double Rows", text);
        Assert.Contains("public double Cols", text);
    }

    [Fact]
    public void Editor_Options_From_Inherited_Interface()
    {
        var files = Generate(null, true, ("Modules/TestEditor.ts", /*lang=typescript*/ """
            import { Decorators, Widget } from "@serenity-is/corelib"

            export interface BaseEditorOptions {
                cols?: number;
            }

            export interface TestEditorOptions extends BaseEditorOptions {
                rows?: number;
            }

            @Decorators.registerEditor('MyProject.MyTest.TestEditor')
            export class TestEditor extends Widget {
                constructor(props: TestEditorOptions) {
                }
            }
            """));

        var text = Read(files, "MyTest.TestEditorAttribute.cs");
        Assert.Contains("public double Rows", text);
        Assert.Contains("public double Cols", text);
    }

    [Fact]
    public void Editor_Stops_At_LookupEditorOptions_Base()
    {
        var files = Generate(null, true, ("Modules/TestLookupEditor.ts", /*lang=typescript*/ """
            import { Decorators, LookupEditorBase } from "@serenity-is/corelib"

            export interface MyLookupOptions extends LookupEditorOptions {
                customThing?: string;
            }

            @Decorators.registerEditor('MyProject.MyTest.TestLookupEditor')
            export class TestLookupEditor extends LookupEditorBase<MyLookupOptions> {
                constructor(props: MyLookupOptions) {
                }
            }
            """));

        var text = Read(files, "MyTest.TestLookupEditorAttribute.cs");
        Assert.Contains("public string CustomThing", text);
        Assert.DoesNotContain("AllowClear", text);
        Assert.DoesNotContain("LookupKey", text);
        Assert.DoesNotContain("Async", text);
    }
}
