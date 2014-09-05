using System;
using System.Text.RegularExpressions;

namespace Serenity.ComponentModel
{
    public partial class EmailEditorAttribute : CustomEditorAttribute, ICustomValidator
    {
        public EmailEditorAttribute()
            : base("Email")
        {
        }

        public String Domain
        {
            get { return GetOption<String>("domain"); }
            set { SetOption("domain", value); }
        }

        public Boolean ReadOnlyDomain
        {
            get { return GetOption<Boolean>("readOnlyDomain"); }
            set { SetOption("readOnlyDomain", value); }
        }

        public static readonly Regex EmailPattern = 
            new Regex(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+" + 
                    @"@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$",
                RegexOptions.ECMAScript | RegexOptions.Compiled);

        public string Validate(IValidationContext context)
        {
            if (context.Value == null)
                return null;

            var value = context.Value.ToString();

            if (!EmailPattern.IsMatch(value))
                return Web.Texts.Validation.Email;

            return null;
        }
    }
}