namespace Serenity.CodeGeneration;

public partial class ServerTypingsGeneratorTests
{
    [Fact]
    public void Form_WithUnknownEditorType_Generates_WidgetAny()
    {
        var generator = CreateGenerator(typeof(FormWithUnknownEditor));
        generator.AddTSType(new()
        {
            Name = "Widget",
            Module = "@serenity-is/corelib",
            GenericParameters =
            [
                new()
                {
                    Name = "TOptions",
                }
            ]
        });
        var result = generator.Run();
        var code = Assert.Single(ExceptGenericFiles(result)).Text;
        Assert.Equal(NormalizeTS(@"import { Widget, PrefixedContext, initFormType } from ""@serenity-is/corelib"";

export interface FormWithUnknownEditor {
    Test: Widget;
}

export class FormWithUnknownEditor extends PrefixedContext {
    static readonly formKey = 'FormWithUnknownEditor';
    private static init: boolean;

    constructor(prefix: string) {
        super(prefix);

        if (!FormWithUnknownEditor.init) {
            FormWithUnknownEditor.init = true;

            var w0 = Widget;

            initFormType(FormWithUnknownEditor, [
                'Test', w0
            ]);
        }
    }
}"), NormalizeTS(code));
    }
}

[FormScript("FormWithUnknownEditor")]
public class FormWithUnknownEditor
{
    [EditorType("UnknownEditorKey")]
    public string Test { get; set; }
}