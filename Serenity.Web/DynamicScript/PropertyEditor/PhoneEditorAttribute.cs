using System;

namespace Serenity.ComponentModel
{
    public partial class PhoneEditorAttribute : CustomEditorAttribute, ICustomValidator
    {
        public PhoneEditorAttribute()
            : base("Phone")
        {
        }

        public Boolean Internal
        {
            get { return GetOption<Boolean>("internal"); }
            set { SetOption("internal", value); }
        }

        public Boolean Mobile
        {
            get { return GetOption<Boolean>("mobile"); }
            set { SetOption("mobile", value); }
        }

        public Boolean Multiple
        {
            get { return GetOption<Boolean>("multiple"); }
            set { SetOption("multiple", value); }
        }

        public Boolean AllowInternational
        {
            get { return GetOption<Boolean>("allowInternational"); }
            set { SetOption("allowInternational", value); }
        }

        public Boolean AllowExtension
        {
            get { return GetOption<Boolean>("allowExtension"); }
            set { SetOption("allowExtension", value); }
        }

        public string Validate(IValidationContext context)
        {
            if (context.Value == null)
                return null;

            var value = context.Value.ToString();
            return Validate(value, this.Multiple, this.Internal, this.Mobile, this.AllowInternational, this.AllowExtension);
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
}