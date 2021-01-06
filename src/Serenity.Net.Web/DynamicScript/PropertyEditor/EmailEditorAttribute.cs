using System.Text.RegularExpressions;

namespace Serenity.ComponentModel
{
    public partial class EmailEditorAttribute : CustomEditorAttribute, ICustomValidator
    {
        public EmailEditorAttribute()
            : base("Email")
        {
        }

        public string Domain
        {
            get { return GetOption<string>("domain"); }
            set { SetOption("domain", value); }
        }

        public bool ReadOnlyDomain
        {
            get { return GetOption<bool>("readOnlyDomain"); }
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
                return Web.Texts.Validation.Email.ToString(context.Localizer);

            return null;
        }
    }
}