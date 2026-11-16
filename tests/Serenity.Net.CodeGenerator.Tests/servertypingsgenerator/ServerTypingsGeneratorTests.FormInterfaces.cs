using ServerTypingsTest.FormInterfaces;

namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGeneratorTests
    {
        private static string GenerateInterfaceForm()
        {
            var result = CreateGenerator(typeof(FormWithInterface)).Run();
            return Assert.Single(result, x => x.Filename == "FormInterfaces/FormWithInterface.ts").Text;
        }

        [Fact]
        public void Form_Uses_The_Editor_Of_The_Interface_Member_A_Property_Implements()
        {
            var code = GenerateInterfaceForm();

            Assert.Contains("Email: EmailAddressEditor;", code);
            Assert.Contains("'Email', EmailAddressEditor", code);
        }

        [Fact]
        public void Form_Uses_The_Own_Editor_Of_A_Property_Over_The_One_Of_The_Interface()
        {
            var code = GenerateInterfaceForm();

            Assert.Contains("Notes: TextAreaEditor;", code);
        }

        [Fact]
        public void Form_Uses_The_Editor_Of_A_Combination_Attribute_Of_The_Interface_Member()
        {
            var code = GenerateInterfaceForm();

            Assert.Contains("Total: DecimalEditor;", code);
        }

        [Fact]
        public void Form_Keeps_The_Order_The_Form_Declares_The_Properties_In()
        {
            var code = GenerateInterfaceForm();

            Assert.True(code.IndexOf("Notes:", StringComparison.Ordinal) < code.IndexOf("Email:", StringComparison.Ordinal));
            Assert.True(code.IndexOf("Email:", StringComparison.Ordinal) < code.IndexOf("Total:", StringComparison.Ordinal));
        }

        [Fact]
        public void Form_Leaves_Out_A_Property_Whose_Interface_Member_Is_Ignored()
        {
            var code = GenerateInterfaceForm();

            Assert.DoesNotContain("Secret", code);
        }
    }
}

namespace ServerTypingsTest.FormInterfaces
{
    public class DecimalTotalAttribute : System.Attribute, Serenity.Reflection.IIntrinsicPropertyAttributeProvider
    {
        [DecimalEditor]
        public object PropertyAttributes { get; } = null!;
    }

    public interface IContactInfo
    {
        [EmailAddressEditor]
        string Email { get; set; }

        [EmailAddressEditor]
        string Notes { get; set; }

        [DecimalTotal]
        decimal? Total { get; set; }

        [IgnoreUIField]
        string Secret { get; set; }
    }

    [FormScript("FormInterfaces.WithInterface")]
    public class FormWithInterface : IContactInfo
    {
        [TextAreaEditor]
        public string Notes { get; set; }

        public string Email { get; set; }

        public decimal? Total { get; set; }

        public string Secret { get; set; }
    }
}
