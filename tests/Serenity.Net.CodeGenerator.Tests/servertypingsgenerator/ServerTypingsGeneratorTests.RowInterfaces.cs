using ServerTypingsTest.RowInterfaces.Entities;
using ServerTypingsTest.RowInterfaces.Forms;

namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGeneratorTests
    {
        [Fact]
        public void Row_Reads_Id_And_Name_Properties_From_The_Interface_Members_They_Implement()
        {
            var result = CreateGenerator(typeof(ContactRow)).Run();
            var code = Assert.Single(result, x => x.Filename == "RowInterfaces/ContactRow.ts").Text;

            Assert.Contains("idProperty = 'Code'", code);
            Assert.Contains("nameProperty = 'Title'", code);
        }

        [Fact]
        public void Row_Leaves_Out_A_Field_Whose_Interface_Member_Is_Ignored()
        {
            var result = CreateGenerator(typeof(ContactRow)).Run();
            var code = Assert.Single(result, x => x.Filename == "RowInterfaces/ContactRow.ts").Text;

            Assert.Contains("Email", code);
            Assert.DoesNotContain("Secret", code);
        }

        [Fact]
        public void Form_Uses_The_Editor_The_Row_Field_Has_From_The_Interface_Of_The_Row()
        {
            var result = CreateGenerator(typeof(ContactForm)).Run();
            var code = Assert.Single(result, x => x.Filename == "RowInterfaces/ContactForm.ts").Text;

            Assert.Contains("Email: EmailAddressEditor;", code);
        }
    }
}

namespace ServerTypingsTest.RowInterfaces.Entities
{
    public interface IContactInfo
    {
        [IdProperty]
        string Code { get; set; }

        [NameProperty]
        string Title { get; set; }

        [EmailAddressEditor]
        string Email { get; set; }

        [IgnoreUIField]
        string Secret { get; set; }
    }

    public class ContactRow : Row<ContactRow.RowFields>, IContactInfo
    {
        public string Code { get => fields.Code[this]; set => fields.Code[this] = value; }
        public string Title { get => fields.Title[this]; set => fields.Title[this] = value; }
        public string Email { get => fields.Email[this]; set => fields.Email[this] = value; }
        public string Secret { get => fields.Secret[this]; set => fields.Secret[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public StringField Code;
            public StringField Title;
            public StringField Email;
            public StringField Secret;
        }
    }
}

namespace ServerTypingsTest.RowInterfaces.Forms
{
    [FormScript("RowInterfaces.Contact"), BasedOnRow(typeof(ContactRow))]
    public class ContactForm
    {
        public string Title { get; set; }
        public string Email { get; set; }
    }
}
