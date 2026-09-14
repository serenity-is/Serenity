namespace Serenity.CodeGeneration;

public partial class ClientTypesGeneratorTests
{
    [Fact]
    public void Editor_Options_From_Separate_Interface()
    {
        var files = Generate(null, true, ("Modules/TestEditor.ts", /*lang=typescript*/ """
            import { Decorators, Widget } from "@serenity-is/corelib"

            export interface TestEditorOptions {
                cols?: number;
                rows?: number;
            }

            @Decorators.registerEditor('MyProject.MyTest.TestEditor')
            export class TestEditor extends Widget {
                constructor(props: TestEditorOptions) {
                }
            }
            """));

        var text = Read(files, "MyTest.TestEditorAttribute.cs");
        Assert.Contains("public double Cols", text);
        Assert.Contains("public double Rows", text);
    }

    [Fact]
    public void Editor_Options_From_Type_Alias_Declaration()
    {
        var files = Generate(null, true, ("Modules/TestEditor.ts", /*lang=typescript*/ """
            import { Decorators, Widget } from "@serenity-is/corelib"

            export type TestEditorOptions = { cols: number, rows: number };

            @Decorators.registerEditor('MyProject.MyTest.TestEditor')
            export class TestEditor extends Widget {
                constructor(props: TestEditorOptions) {
                }
            }
            """));

        var text = Read(files, "MyTest.TestEditorAttribute.cs");
        Assert.Contains("public double Cols", text);
        Assert.Contains("public double Rows", text);
    }

    [Fact]
    public void Editor_Options_From_Generic_Parameter_Default()
    {
        var files = Generate(null, true, ("Modules/TestEditor.ts", /*lang=typescript*/ """
            import { Decorators, EditorProps, Widget } from "@serenity-is/corelib"

            export interface TestEditorOptions {
                cols?: number;
                rows?: number;
            }

            @Decorators.registerEditor('MyProject.MyTest.TestEditor')
            export class TestEditor<P extends TestEditorOptions = TestEditorOptions> extends Widget {
                constructor(props: EditorProps<P>) {
                }
            }
            """));

        var text = Read(files, "MyTest.TestEditorAttribute.cs");
        Assert.Contains("public double Cols", text);
        Assert.Contains("public double Rows", text);
    }

    [Fact]
    public void Editor_With_LookupEditorBase_Skips_Base_Options()
    {
        var files = Generate(null, true, ("Modules/TestLookupEditor.ts", /*lang=typescript*/ """
            import { Decorators, LookupEditorBase } from "@serenity-is/corelib"

            export interface TestLookupEditorOptions {
                lookupKey?: string;
                async?: boolean;
                customThing?: string;
            }

            @Decorators.registerEditor('MyProject.MyTest.TestLookupEditor')
            export class TestLookupEditor extends LookupEditorBase<TestLookupEditorOptions> {
                constructor(props: TestLookupEditorOptions) {
                }
            }
            """));

        var text = Read(files, "MyTest.TestLookupEditorAttribute.cs");
        Assert.StartsWith("using Serenity;", text);
        Assert.Contains(": LookupEditorBaseAttribute", text);
        Assert.Contains("public string CustomThing", text);
        Assert.DoesNotContain("LookupKey", text);
        Assert.DoesNotContain("Async", text);
    }

    [Fact]
    public void Editor_With_ServiceLookupEditorBase_Skips_Base_Options()
    {
        var files = Generate(null, true, ("Modules/TestServiceLookupEditor.ts", /*lang=typescript*/ """
            import { Decorators, ServiceLookupEditorBase } from "@serenity-is/corelib"

            export interface TestServiceLookupEditorOptions {
                service?: string;
                idField?: string;
                customThing?: string;
            }

            @Decorators.registerEditor('MyProject.MyTest.TestServiceLookupEditor')
            export class TestServiceLookupEditor extends ServiceLookupEditorBase<TestServiceLookupEditorOptions> {
                constructor(props: TestServiceLookupEditorOptions) {
                }
            }
            """));

        var text = Read(files, "MyTest.TestServiceLookupEditorAttribute.cs");
        Assert.Contains(": ServiceLookupEditorBaseAttribute", text);
        Assert.Contains("public string CustomThing", text);
        Assert.DoesNotContain("Service ", text);
        Assert.DoesNotContain("IdField", text);
    }
}
