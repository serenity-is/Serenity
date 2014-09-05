using System.Text.RegularExpressions;

namespace Serenity.ComponentModel
{
    public partial class EmailEditorAttribute : ICustomValidator
    {
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