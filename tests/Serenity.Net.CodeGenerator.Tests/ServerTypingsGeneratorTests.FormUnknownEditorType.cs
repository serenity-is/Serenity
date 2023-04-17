using ServerTypingsTest.TypeWithGenericParameters;

namespace Serenity.Tests.CodeGenerator
{
    public partial class ServerTypingsGeneratorTests
    {
        [Fact]
        public void Form_WithUnknownEditorType_Generates_WidgetAny()
        {
            var generator = CreateGenerator(typeof(FormWithUnknownEditor));
            generator.AddTSType(new()
            {
                Name = "Widget",
                Namespace = "Serenity",
                GenericParameters = new()
                {
                    new()
                    {
                        Name = "TOptions",
                    }
                }
            });
            var result = generator.Run();
            var code = Assert.Single(result).Text;
            Assert.Equal(NormalizeTS(@"
namespace Serenity.Tests.CodeGenerator {export interface FormWithUnknownEditor {
        Test: Serenity.Widget<any>;
    }

    export class FormWithUnknownEditor extends Serenity.PrefixedContext {
        static formKey = 'FormWithUnknownEditor';
        private static init: boolean;

        constructor(prefix: string) {
            super(prefix);

            if (!FormWithUnknownEditor.init) {
                FormWithUnknownEditor.init = true;

                var s = Serenity;
                var w0 = s.Widget;

                Q.initFormType(FormWithUnknownEditor, [
                    'Test', w0
                ]);
            }
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
}