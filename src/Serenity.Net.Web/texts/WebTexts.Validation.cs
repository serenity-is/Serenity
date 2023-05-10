namespace Serenity.Web;

public static partial class WebTexts
{
    public static class Validation
    {
        public static readonly LocalText CaptchaMismatch = "Incorrect code entered. You can generate a new one by clicking image, if you can't see the characters clearly.";
        public static readonly LocalText DayHourAndMin = "Please enter a valid timespan (e.g. 2.15:30)";
        public static readonly LocalText DateInvalid = "Please enter a valid date.";
        public static readonly LocalText Decimal = "Please enter a valid decimal value.";
        public static readonly LocalText Digits = "Please only use digits 0-9.";
        public static readonly LocalText Email = "Please enter a valid e-mail address.";
        public static readonly LocalText EmailMultiple = "Please enter a valid e-mail address (if more than one, use ';' as separator)";
        public static readonly LocalText EmailExists = "There is another user registered with this e-mail address. If you don't remember your password, use the forgot my password form.";
        public static readonly LocalText HourAndMin = "Please enter a valid time (e.g. 15:30)";
        public static readonly LocalText IncorrectPassword = "Incorrect password. Please check.";
        public static readonly LocalText Integer = "Please enter a valid integer value.";
        public static readonly LocalText MaxLength = "Please enter no more than {0} characters.";
        public static readonly LocalText MinLength = "Please enter no less than {0} characters.";
        public static readonly LocalText PasswordConfirm = "Password entered don't match.";
        public static readonly LocalText Required = "This field is required.";
        public static readonly LocalText Range = "Please enter a value between {0} and {1}.";
        public static readonly LocalText Url = "Please enter a valid URL (e.g. http://www.site.com).";
        public static readonly LocalText Username = "Not a valid username.";
        public static readonly LocalText UsernameExists = "This username is already used. Please choose another.";
        public static readonly LocalText Xss = "Invalid value! (don't use '<' and '&')";
        public static readonly LocalText InvalidFormMessage = "Please validate empty or invalid inputs (marked with red) before submitting the form.";
        public static readonly LocalText UniqueConstraint = "Another record with the same '{0}' value(s) exists!";
        public static readonly LocalText MinDate = "Please enter a date after '{0}'.";
        public static readonly LocalText MaxDate = "Please enter a date before '{0}'.";
    }
}