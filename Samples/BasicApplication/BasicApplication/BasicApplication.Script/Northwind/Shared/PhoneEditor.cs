namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using Serenity.ComponentModel;
    using System;

    [Editor]
    public class PhoneEditor : StringEditor, IStringValue
    {
        public PhoneEditor(jQueryObject input)
            : base(input)
        {
            this.AddValidationRule(this.uniqueName, (e) =>
            {
                string value = this.Value.TrimToNull();
                if (value == null)
                    return null;

                return Validate(value, this.Multiple);
            });

            input.Bind("change", delegate(jQueryEvent e)
            {
                if (!e.HasOriginalEvent())
                    return;

                FormatValue();
            });

            input.Bind("blur", delegate(jQueryEvent e)
            {
                if (this.element.HasClass("valid"))
                {
                    FormatValue();
                }
            });
        }

        public void FormatValue()
        {
            this.element.Value(GetFormattedValue());
        }

        public string GetFormattedValue()
        {
            var value = this.element.GetValue();

            if (Multiple)
                return FormatMulti(value, FormatPhone);

            return FormatPhone(value);
        }

        [Option]
        public bool Multiple { get; set; }

        private static string Validate(string phone, bool isMultiple)
        {
            bool valid = isMultiple ? IsValidMulti(phone, IsValidPhone) : IsValidPhone(phone);

            if (valid)
                return null;

            return Q.Text(isMultiple ? "Validation.NorthwindPhoneMultiple" : "Validation.NorthwindPhone");
        }

        private static bool IsValidPhone(string phone)
        {
            if (phone.IsEmptyOrNull())
                return false;

            phone = phone.Replace(" ", "").Replace("-", "");

            if (phone.Length < 10)
                return false;

            if (phone.StartsWith("0"))
                phone = phone.Substring(1);

            if (phone.StartsWith("(") &&
                phone[4] == ')')
            {
                phone = phone.Substring(1, 3) + phone.Substring(5);
            }

            if (phone.Length != 10)
                return false;

            if (phone.StartsWith("0"))
                return false;

            for (var i = 0; i < phone.Length; i++)
            {
                var c = (int)phone[i];
                if (c < (int)'0' || c > (int)'9')
                    return false;
            }

            return true;
        }

        private static string FormatPhone(string phone)
        {
            if (!IsValidPhone(phone))
                return phone;

            phone = phone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
            if (phone.StartsWith("0"))
                phone = phone.Substring(1);

            phone = "(" + phone.Substring(0, 3) + ") " + phone.Substring(3, 3) + "-" + phone.Substring(6, 2) + phone.Substring(8, 2);
            return phone;
        }

        private static string FormatMulti(string phone, Func<string, string> format)
        {
            var phones = phone.Replace(';', ',').Split(',');
            string result = "";
            foreach (var x in phones)
            {
                string s = x.TrimToNull();
                if (s == null)
                    continue;

                if (result.Length > 0)
                    result += ", ";

                result += format(s);
            }
            return result;
        }

        private static bool IsValidMulti(string phone, Func<string, bool> check)
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

                if (!check(s))
                    return false;

                anyValid = true;
            }

            if (!anyValid)
                return false;

            return true;
        }

        public new string Value
        {
            get
            {
                return GetFormattedValue();
            }
            set
            {
                element.Value(value);
            }
        }
    }
}