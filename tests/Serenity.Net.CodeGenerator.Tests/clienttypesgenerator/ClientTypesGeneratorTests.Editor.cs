namespace Serenity.CodeGeneration;

public partial class ClientTypesGeneratorTests
{
    [Fact]
    public void Editor_Generates_Attribute_With_Constructor_Options()
    {
        var files = Generate(null, true, ("Modules/TestEditor.ts", /*lang=typescript*/ """
            import { Decorators, Widget } from "@serenity-is/corelib"

            @Decorators.registerEditor('MyProject.MyTest.TestEditor')
            export class TestEditor extends Widget {
                constructor(props: { cols?: number, rows?: number }) {
                }
            }
            """));

        var text = Read(files, "MyTest.TestEditorAttribute.cs");
        Assert.Equal("""
            using Serenity;
            using Serenity.ComponentModel;
            using System;
            using System.Collections.Generic;
            using System.ComponentModel;

            namespace MyProject.MyTest
            {
                public partial class TestEditorAttribute : CustomEditorAttribute
                {
                    public const string Key = "MyProject.MyTest.TestEditor";

                    public TestEditorAttribute()
                        : base(Key)
                    {
                    }

                    public double Cols
                    {
                        get { return GetOption<double>("cols"); }
                        set { SetOption("cols", value); }
                    }

                    public double Rows
                    {
                        get { return GetOption<double>("rows"); }
                        set { SetOption("rows", value); }
                    }
                }
            }
            """.ReplaceLineEndings(), text);
    }

    [Fact]
    public void Editor_Uses_TypeInfo_From_RegisterEditor()
    {
        var files = Generate(null, true,
            ("Modules/Ns.ts", /*lang=typescript*/ """
                export const nsTest: "MyProject.MyTest." = "MyProject.MyTest.";
                """),
            ("Modules/TestEditor.ts", /*lang=typescript*/ """
                import { StringEditor, WidgetProps } from "@serenity-is/corelib";
                import { nsTest } from "./Ns";

                export interface TestEditorOptions {
                    multiple?: boolean;
                }

                export class TestEditor<P extends TestEditorOptions = TestEditorOptions> extends StringEditor<P> {
                    static override [Symbol.typeInfo] = this.registerEditor(nsTest);

                    constructor(props: WidgetProps<P>) {
                        super(props);
                    }
                }
                """));

        var text = Read(files, "MyTest.TestEditorAttribute.cs");
        Assert.Equal("""
            using Serenity;
            using Serenity.ComponentModel;
            using System;
            using System.Collections.Generic;
            using System.ComponentModel;

            namespace MyProject.MyTest
            {
                public partial class TestEditorAttribute : CustomEditorAttribute
                {
                    public const string Key = "MyProject.MyTest.TestEditor";

                    public TestEditorAttribute()
                        : base(Key)
                    {
                    }

                    public bool Multiple
                    {
                        get { return GetOption<bool>("multiple"); }
                        set { SetOption("multiple", value); }
                    }
                }
            }
            """.ReplaceLineEndings(), text);
    }

    [Fact]
    public void Editor_Appends_Type_Name_When_Key_Ends_With_Dot()
    {
        var files = Generate(null, true, ("Modules/TestEditor.ts", /*lang=typescript*/ """
            import { Decorators, Widget } from "@serenity-is/corelib"

            @Decorators.registerEditor('MyProject.MyTest.')
            export class TestEditor extends Widget {
            }
            """));

        var text = Read(files, "MyTest.TestEditorAttribute.cs");
        Assert.Contains("public const string Key = \"MyProject.MyTest.TestEditor\";", text);
    }

    [Fact]
    public void Editor_Without_Key_Uses_FullName_And_Default_Namespace()
    {
        var files = Generate(null, true, ("Modules/TestEditor.ts", /*lang=typescript*/ """
            import { Decorators, Widget } from "@serenity-is/corelib"

            @Decorators.registerEditor()
            export class TestEditor extends Widget {
            }
            """));

        var text = Read(files, "TestEditorAttribute.cs");
        Assert.Contains("public const string Key = \"/Modules/TestEditor:TestEditor\";", text);
    }
}
