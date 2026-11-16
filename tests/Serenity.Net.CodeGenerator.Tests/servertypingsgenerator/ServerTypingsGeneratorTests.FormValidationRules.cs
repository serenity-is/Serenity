using ServerTypingsTest.FormRules;

namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGeneratorTests
    {
        private static ExternalType RuleClass(string name, string ns)
        {
            return new ExternalType
            {
                Name = name,
                Module = "/Modules/Rules/" + name,
                Fields =
                [
                    new ExternalMember
                    {
                        Name = "[Symbol.typeInfo]",
                        IsStatic = true,
                        Type = "RuleTypeInfo",
                        Value = ns
                    }
                ]
            };
        }

        private static string GenerateRulesForm()
        {
            var generator = CreateGenerator(typeof(FormWithRules));
            generator.AddTSType(RuleClass("EvenValidationRule", "ServerTypingsTest.FormRules."));
            generator.AddTSType(RuleClass("SpecialCodeValidationRule", "MyApp.Rules."));
            generator.AddTSType(RuleClass("UnusedValidationRule", "ServerTypingsTest.FormRules."));
            var result = generator.Run();
            return Assert.Single(result, x => x.Filename == "FormRules/FormWithRules.ts").Text;
        }

        [Fact]
        public void Form_Imports_The_Rule_Class_Of_A_Rule_By_The_Key_Constant_Of_Its_Attribute()
        {
            var code = GenerateRulesForm();
            Assert.Contains("import { SpecialCodeValidationRule } from \"@/Rules/SpecialCodeValidationRule\";", code);
            Assert.Contains("SpecialCodeValidationRule", code.Split("// referenced types")[0].Split('\n')[^1]);
        }

        [Fact]
        public void Form_Imports_The_Rule_Class_Of_A_Rule_By_The_Convention_Without_A_Key()
        {
            var code = GenerateRulesForm();
            Assert.Contains("import { EvenValidationRule } from \"@/Rules/EvenValidationRule\";", code);
            Assert.Contains("EvenValidationRule", code.Split("// referenced types")[0].Split('\n')[^1]);
        }

        [Fact]
        public void Form_Doesnt_Import_The_Built_In_Rules_Or_Ones_It_Doesnt_Have()
        {
            var code = GenerateRulesForm();
            Assert.DoesNotContain("UnusedValidationRule", code);
            Assert.DoesNotContain("RangeValidationRule", code);
        }
    }
}

namespace ServerTypingsTest.FormRules
{
    public class ValidateEvenAttribute : Serenity.ComponentModel.ValidationRuleAttribute
    {
        protected override bool IsValidValue(object value, System.ComponentModel.DataAnnotations.ValidationContext context) => true;
    }

    public class ValidateSpecialCodeAttribute() : Serenity.ComponentModel.ValidationRuleAttribute(Key)
    {
        public const string Key = "MyApp.Rules.SpecialCodeValidationRule";

        protected override bool IsValidValue(object value, System.ComponentModel.DataAnnotations.ValidationContext context) => true;
    }

    [FormScript("FormRules.WithRules")]
    public class FormWithRules
    {
        [ValidateEven]
        public int? Quantity { get; set; }

        [ValidateSpecialCode]
        public string Code { get; set; }

        [ValidateRange(1, 10)]
        public int? Plain { get; set; }
    }
}
