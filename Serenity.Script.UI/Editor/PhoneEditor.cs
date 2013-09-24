using jQueryApi;
using Serenity.ComponentModel;
using System;
using System.Html;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Serenity
{
    [Editor("Metin")]
    [Element("<input type=\"text\"/>")]
    public class PhoneEditor : Widget<PhoneEditorOptions>, IStringValue
    {
        public PhoneEditor(jQueryObject input, PhoneEditorOptions opt)
            : base(input, opt)
        {
            RegisterValidationMethods();

            input.AddClass(options.Multiple ? 
                (options.Internal ? "phoneInternalMulti" : "phoneTurkeyMulti") :
                (options.Internal ? "phoneInternal" : "phoneTurkey"));

            input.Bind("change", delegate(jQueryEvent e)
            {
                if (!e.HasOriginalEvent())
                    return;

                FormatValue();
            });
        }

        [InlineCode("this.optional({element})")]
        private static bool ValidatorIsOptional(Element element)
        {
            return false;
        }

        public void FormatValue()
        {
            var value = this.element.GetValue();

            if (!options.Multiple &&
                !options.Internal)
            {
                this.element.Value(FormatPhoneTurkey(value));
            }
            else if (options.Multiple &&
                !options.Internal)
            {
                this.element.Value(FormatPhoneTurkeyMulti(value));
            }
            else if (options.Internal &&
                !options.Multiple)
            {
                this.element.Value(FormatPhoneInternal(value));
            }
            else if (options.Internal &&
                options.Multiple)
            {
                this.element.Value(FormatPhoneInternalMulti(value));
            }
        }

        private static string FormatPhoneTurkey(string phone)
        {
            if (!IsValidPhoneTurkey(phone))
                return phone;

            phone = phone.Replace(" ", "").Replace("(", "").Replace(")", "");
            if (phone.StartsWith("0"))
                phone = phone.Substr(1);

            phone = "(" + phone.Substr(0, 3) + ") " + phone.Substr(3, 3) + " " + phone.Substr(6, 2) + " " + phone.Substr(8, 2);
            return phone;
        }

        private static bool IsValidPhoneTurkey(string phone)
        {
            if (phone.IsEmptyOrNull())
                return false;

            phone = phone.Replace(" ", "");

            if (phone.Length < 10)
                return false;

            if (phone.StartsWith("0"))
                phone = phone.Substr(1);

            if (phone.StartsWith("(") &&
                phone.CharAt(4) == ")")
            {
                phone = phone.Substr(1, 3) + phone.Substr(5);
            }

            if (phone.Length != 10)
                return false;

            if (phone.StartsWith("0"))
                return false;

            if (phone.CharAt(3) == "0")
                return false;

            for (var i = 0; i < phone.Length; i++)
            {
                var c = phone.CharCodeAt(i);
                if (c < (int)'0' || c > (int)'9')
                    return false;
            }

            return true;
        }

        private static string FormatPhoneTurkeyMulti(string phone)
        {
            if (!IsValidPhoneTurkeyMulti(phone))
                return phone;

            var phones = phone.Replace(';', ',').Split(',');
            string result = "";
            foreach (var x in phones)
            {
                string s = x.TrimToNull();
                if (s == null)
                    continue;

                if (result.Length > 0)
                    result += ", ";

                result += FormatPhoneTurkey(s);
            }
            return result;
        }

        private static bool IsValidPhoneTurkeyMulti(string phone)
        {
            if (phone.IsEmptyOrNull())
                return false;

            var phones = phone.Replace(';', ',').Split(',');
            bool anyValid = false;
            foreach (var x in phones)
            {
                string s = x.TrimToNull();
                if (s == null)
                    continue;

                if (!IsValidPhoneTurkey(s))
                    return false;

                anyValid = true;
            }

            if (!anyValid)
                return false;

            return true;
        }

        private static string FormatPhoneInternal(string phone)
        {
            if (!IsValidPhoneTurkey(phone))
                return phone;

            return phone.Trim();
        }

        private static bool IsValidPhoneInternal(string phone)
        {
            if (phone.IsEmptyOrNull())
                return false;

            phone = phone.Trim();

            if (phone.Length < 3 || phone.Length > 5)
                return false;

            for (var i = 0; i < phone.Length; i++)
            {
                var c = phone.CharCodeAt(i);
                if (c < (int)'0' || c > (int)'9')
                    return false;
            }

            return true;
        }

        private static string FormatPhoneInternalMulti(string phone)
        {
            if (!IsValidPhoneInternalMulti(phone))
                return phone;

            var phones = phone.Replace(';', ',').Split(',');
            string result = "";
            foreach (var x in phones)
            {
                string s = x.TrimToNull();
                if (s == null)
                    continue;

                if (result.Length > 0)
                    result += ", ";

                result += FormatPhoneInternal(s);
            }
            return result;
        }

        private static bool IsValidPhoneInternalMulti(string phone)
        {
            if (phone.IsEmptyOrNull())
                return false;

            var phones = phone.Replace(';', ',').Split(',');
            bool anyValid = false;
            foreach (var x in phones)
            {
                string s = x.TrimToNull();
                if (s == null)
                    continue;

                if (!IsValidPhoneInternal(s))
                    return false;

                anyValid = true;
            }

            if (!anyValid)
                return false;

            return true;
        }


        private static void RegisterValidationMethods()
        {
            if (jQueryValidator.Methods["phoneTurkey"] == null)
                jQueryValidator.AddMethod("phoneTurkey", (value, element) =>
                {
                    return ValidatorIsOptional(element) || IsValidPhoneTurkey(value);
                }, "Telefon numarası '(533) 432 10 98' formatında girilmelidir!");

            if (jQueryValidator.Methods["phoneTurkeyMulti"] == null)
                jQueryValidator.AddMethod("phoneTurkeyMulti", (value, element) =>
                {
                    return ValidatorIsOptional(element) || IsValidPhoneTurkeyMulti(value);
                }, "Telefon numaraları '(533) 432 10 98' formatında ve birden fazlaysa virgülle ayrılarak girilmelidir!");

            if (jQueryValidator.Methods["phoneInternal"] == null)
                jQueryValidator.AddMethod("phoneInternal", (value, element) =>
                {
                    return ValidatorIsOptional(element) || IsValidPhoneInternal(value);
                }, "Dahili telefon numarası '4567' formatında girilmelidir!");

            if (jQueryValidator.Methods["phoneInternalMulti"] == null)
                jQueryValidator.AddMethod("phoneInternalMulti", (value, element) =>
                {
                    return ValidatorIsOptional(element) || IsValidPhoneInternalMulti(value);
                }, "Dahili telefon numaraları '4567, 8930' formatında ve birden fazlaysa virgülle ayrılarak girilmelidir!");
        }

        public string Value
        {
            get 
            { 
                FormatValue();  
                return this.element.GetValue(); 
            }
            set 
            { 
                this.element.Value(value);
            }
        }
    }

    [Serializable, Reflectable]
    public class PhoneEditorOptions
    {
        [DisplayName("Birden Çok Girişe İzin Ver")]
        public bool Multiple { get; set; }
        [DisplayName("Dahili Telefon")]
        public bool Internal { get; set; }
    }
}