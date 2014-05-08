using jQueryApi;
using System.ComponentModel;

namespace Serenity
{
    using System.Text.RegularExpressions;

    [Editor, DisplayName("Kişi İsim")]
    [Element("<input type=\"text\"/>")]
    public class PersonNameEditor : Widget<object>, IStringValue
    {
        public PersonNameEditor(jQueryObject input)
            : base(input, new object())
        {
            this.AddValidationRule(this.uniqueName, delegate
            {
                if (!new Regex("^[ A-Za-zıİğĞöÖüÜşŞÇç]+$").Test(this.Value.TrimToEmpty()))
                    return "Lütfen sadece harflerden oluşan bir metin giriniz!";

                return null;
            });
        }

        public string Value
        {
            get { return this.element.GetValue(); }
            set { this.element.Value(value); }
        }
    }
}