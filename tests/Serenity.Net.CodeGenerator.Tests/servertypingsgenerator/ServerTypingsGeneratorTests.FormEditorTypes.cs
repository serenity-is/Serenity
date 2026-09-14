using ServerTypingsTest.Forms;

namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGeneratorTests
    {
        [Fact]
        public void Form_Auto_Determines_Editor_Types()
        {
            var generator = CreateGenerator(typeof(FormWithAutoEditors));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "FormWithAutoEditors.ts").Text;

            Assert.Contains("export interface FormWithAutoEditors", code);
            Assert.Contains("StringProp: ", code);
            Assert.Contains("IntProp: ", code);
            Assert.Contains("BoolProp: ", code);
            Assert.Contains("DateProp: ", code);
            Assert.Contains("DecimalProp: ", code);
            Assert.DoesNotContain("IgnoredProp", code);
            Assert.Contains("export class FormWithAutoEditors extends PrefixedContext", code);
            Assert.Contains("initFormType(FormWithAutoEditors, [", code);
        }

        [Fact]
        public void Form_Determines_Editor_Types_From_Attributes()
        {
            var generator = CreateGenerator(typeof(FormWithEditorTypes));
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "FormWithEditorTypes.ts").Text;

            Assert.Contains("ExplicitProp: ", code);
            Assert.Contains("KeyConstProp: ", code);
            Assert.Contains("LiteralProp: ", code);
            Assert.Contains("FallbackProp: ", code);
            Assert.Contains("export class FormWithEditorTypes extends PrefixedContext", code);
        }
    }
}

namespace ServerTypingsTest.Forms
{
    [FormScript("Forms.Auto")]
    public class FormWithAutoEditors
    {
        public string StringProp { get; set; }
        public int IntProp { get; set; }
        public bool BoolProp { get; set; }
        public DateTime DateProp { get; set; }
        public decimal DecimalProp { get; set; }

        [IgnoreUIField]
        public string IgnoredProp { get; set; }
    }

    [FormScript("Forms.Editors")]
    public class FormWithEditorTypes
    {
        [EditorType("Forms.ExplicitEditor")]
        public string ExplicitProp { get; set; }

        [MyEditor]
        public string KeyConstProp { get; set; }

        [MyLiteralEditor]
        public string LiteralProp { get; set; }

        [MyFallbackEditor]
        public string FallbackProp { get; set; }
    }

    public class MyEditorAttribute : CustomEditorAttribute
    {
        public const string Key = "Forms.MyEditor";

        public MyEditorAttribute()
            : base(Key)
        {
        }
    }

    public class MyLiteralEditorAttribute : CustomEditorAttribute
    {
        public MyLiteralEditorAttribute()
            : base("Forms.LiteralEditor")
        {
        }
    }

    public class MyFallbackEditorAttribute : EditorTypeAttribute
    {
        static readonly string fallback = "Forms.FallbackEditor";

        public MyFallbackEditorAttribute()
            : base(fallback)
        {
        }
    }
}
