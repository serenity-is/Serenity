using jQueryApi;
using System;
using System.ComponentModel;
using System.Html;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Serenity
{
    [Editor, DisplayName("Telefon"), OptionsType(typeof(PhoneEditorOptions))]
    [Element("<input type=\"text\"/>")]
    public class PhoneEditor : Widget<PhoneEditorOptions>, IStringValue
    {
        static PhoneEditor()
        {
            Q.Prop(typeof(PhoneEditor), "value");
        }

        public PhoneEditor(jQueryObject input, PhoneEditorOptions opt)
            : base(input, opt)
        {
            var self = this;

            this.AddValidationRule(this.uniqueName, (e) =>
            {
                string value = this.Value.TrimToNull();
                if (value == null)
                    return null;

                return Validate(value);
            });

            string hint = options.Internal ? 
                "Dahili telefon numarası '456, 8930, 12345' formatlarında" :
                (options.Mobile ? "Cep telefonu numarası '(533) 342 01 89' formatında" :
                    "Telefon numarası '(216) 432 10 98' formatında");

            if (options.Multiple)
                hint = hint.Replace("numarası", "numaraları") + " ve birden fazlaysa virgülle ayrılarak ";
            
            hint += " girilmelidir.";

            input.Attribute("title", hint);

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

            input.Bind("keyup", delegate(jQueryEvent e)
            {
                if (options.Internal)
                    return;

                var val = (input.GetValue() ?? "");
                if (val.Length > 0 && ((dynamic)input[0]).selectionEnd == val.Length &&
                    ((e.Which >= 48 && e.Which <= 57) || (e.Which >= 96 && e.Which <= 105)) &&
                    val[val.Length - 1] >= '0' && val[val.Length - 1] <= '9' && !val.StartsWith("+") && val.IndexOf('/') < 0)
                {
                    if (Validate(val) == null)
                        FormatValue();
                    else
                    {
                        for (var i = 1; i <= 7; i++)
                        {
                            val += "9";
                            if (Validate(val) == null)
                            {
                                this.Value = val;
                                FormatValue();
                                val = this.Value;
                                for (var j = 1; j <= i; j++)
                                {
                                    val = val.Trim();
                                    val = val.Substr(0, val.Length - 1);
                                }
                                this.Value = val;
                                break;
                            }
                        }
                    }
                }
            });
        }

        public string Validate(string value)
        {
            return Validate(value, options.Multiple, options.Internal, options.Mobile, options.AllowInternational, options.AllowExtension);
        }

        public void FormatValue()
        {
            this.element.Value(GetFormattedValue());
        }

        public string GetFormattedValue()
        {
            var value = this.element.GetValue();

            Func<string, string> formatter = null;

            Func<string, string> myFormatter = s =>
            {
                if (string.IsNullOrEmpty(s) || options.Internal)
                    return formatter(s);

                s = (s ?? "").Trim();

                if (s.StartsWith("+"))
                    return s;

                if (s.IndexOf('/') > 0)
                {
                    var p = s.Split(new char[] { '/' });
                    if (p.Length != 2)
                        return s;

                    if (p[0].Length < 5)
                        return s;

                    int x;
                    if (!int.TryParse(p[1], out x))
                        return s;

                    return p[0] + " / " + x.ToString();
                }

                return formatter(s);
            };

            if (!options.Internal &&
                !options.Mobile)
            {
                formatter = FormatPhoneTurkey;
            }
            else if (options.Mobile)
            {
                formatter = FormatMobileTurkey;
            }
            else if (options.Internal)
            {
                formatter = FormatPhoneInternal;
            }

            if (formatter != null)
            {
                if (options.Multiple)
                    return FormatMulti(value, myFormatter);
                else
                    return myFormatter(value);
            }

            return value;
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

        private static string FormatPhoneTurkey(string phone)
        {
            if (!IsValidPhoneTurkey(phone))
                return phone;

            phone = phone.Replace(" ", "").Replace("(", "").Replace(")", "");
            if (phone.StartsWith("0"))
                phone = phone.Substring(1);

            phone = "(" + phone.Substring(0, 3) + ") " + phone.Substring(3, 3) + " " + phone.Substring(6, 2) + " " + phone.Substring(8, 2);
            return phone;
        }

        private static string FormatPhoneTurkeyMulti(string phone)
        {
            if (!IsValidPhoneTurkeyMulti(phone))
                return phone;

            return FormatMulti(phone, FormatPhoneTurkey);
        }

        private static string FormatMobileTurkey(string phone)
        {
            if (!IsValidMobileTurkey(phone))
                return phone;

            return FormatPhoneTurkey(phone);
        }

        private static string FormatMobileTurkeyMulti(string phone)
        {
            if (!IsValidMobileTurkeyMulti(phone))
                return phone;

            return FormatPhoneTurkeyMulti(phone);
        }

        private static string FormatPhoneInternal(string phone)
        {
            if (!IsValidPhoneInternal(phone))
                return phone;

            return phone.Trim();
        }

        private static string FormatPhoneInternalMulti(string phone)
        {
            if (!IsValidPhoneInternalMulti(phone))
                return phone;

            return FormatMulti(phone, FormatPhoneInternal);
        }

        private static bool IsValidPhoneTurkeyMulti(string phone)
        {
            return IsValidMulti(phone, IsValidPhoneTurkey);
        }

        private static bool IsValidMobileTurkeyMulti(string phone)
        {
            return IsValidMulti(phone, IsValidMobileTurkey);
        }

        private static bool IsValidPhoneInternalMulti(string phone)
        {
            return IsValidMulti(phone, IsValidPhoneInternal);
        }

        public string Value
        {
            get
            {
                return GetFormattedValue();
            }
            set
            {
                this.element.Value(value);
            }
        }

        #region @@@SharedValidationCodeBlock@@@

        private static string Validate(string phone, bool isMultiple, bool isInternal, bool isMobile, bool allowInternational, bool allowExtension)
        {
            Func<string, bool> validateFunc;

            if (isInternal)
                validateFunc = IsValidPhoneInternal;
            else if (isMobile)
                validateFunc = IsValidMobileTurkey;
            else
                validateFunc = IsValidPhoneTurkey;

            Func<string, bool> myValidateFunc = s =>
            {
                if (!validateFunc(s))
                {
                    if (isInternal)
                        return false;

                    s = (s ?? "").Trim();

                    if (s.StartsWith("+"))
                    {
                        if (allowInternational &&
                            s.Length > 7)
                        {
                            return true;
                        }

                        return false;
                    }

                    if (allowExtension &&
                        s.IndexOf('/') > 0)
                    {
                        var p = s.Split(new char[] { '/' });
                        if (p.Length != 2)
                            return false;

                        if (p[0].Length < 5 || !validateFunc(p[0]))
                            return false;

                        int x;
                        if (!int.TryParse(p[1].Trim(), out x))
                            return false;

                        return true;
                    }

                    return false;
                }

                return true;
            };

            bool valid = isMultiple ? IsValidMulti(phone, myValidateFunc) : myValidateFunc(phone);

            if (valid)
                return null;

            if (isMultiple)
            {
                if (isInternal)
                    return "Dahili telefon numaraları '4567' formatında ve birden fazlaysa virgülle ayrılarak girilmelidir!";

                if (isMobile)
                    return "Telefon numaraları '(533) 342 01 89' formatında ve birden fazlaysa virgülle ayrılarak girilmelidir!";

                return "Telefon numaları '(216) 432 10 98' formatında ve birden fazlaysa virgülle ayrılarak girilmelidir!";
            }
            else
            {
                if (isInternal)
                    return "Dahili telefon numarası '4567' formatında girilmelidir!";

                if (isMobile)
                    return "Telefon numarası '(533) 342 01 89' formatında girilmelidir!";

                return "Telefon numarası '(216) 432 10 98' formatında girilmelidir!";
            }
        }

        private static bool IsValidPhoneTurkey(string phone)
        {
            if (phone.IsEmptyOrNull())
                return false;

            phone = phone.Replace(" ", "");

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

        private static bool IsValidMobileTurkey(string phone)
        {
            if (!IsValidPhoneTurkey(phone))
                return false;

            phone = phone.TrimStart();
            phone = phone.Replace(" ", "");

            int lookIndex = 0;
            if (phone.StartsWith("0"))
                lookIndex++;

            if (phone[lookIndex] == '5' ||
                phone[lookIndex] == '(' && phone[lookIndex + 1] == '5')
                return true;

            return false;
        }

        private static bool IsValidPhoneInternal(string phone)
        {
            if (phone.IsEmptyOrNull())
                return false;

            phone = phone.Trim();

            if (phone.Length < 2 || phone.Length > 5)
                return false;

            for (var i = 0; i < phone.Length; i++)
            {
                var c = (int)phone[i];
                if (c < (int)'0' || c > (int)'9')
                    return false;
            }

            return true;
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

        #endregion @@@SharedValidationCodeBlock@@@
    }

    [Serializable, Reflectable]
    public class PhoneEditorOptions
    {
        [DisplayName("Birden Çok Girişe İzin Ver")]
        public bool Multiple { get; set; }
        [DisplayName("Dahili Telefon")]
        public bool Internal { get; set; }
        [DisplayName("Cep Telefonu")]
        public bool Mobile { get; set; }
        [DisplayName("Dahili Girişine İzin Ver")]
        public bool AllowExtension { get; set; }
        [DisplayName("Uluslararası Telefon Girişine İzin Ver")]
        public bool AllowInternational { get; set; }
    }
}