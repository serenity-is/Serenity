namespace Serenity.Web
{
    public static partial class Texts
    {
        public static class Validation
        {
            public static LocalText CaptchaMismatch = "Incorrect code entered. You can generate a new one by clicking image, if you can't see the characters clearly.";
            public static LocalText DayHourAndMin = "Please enter a valid timespan (e.g. 2.15:30)";
            public static LocalText DateInvalid = "Please enter a valid date.";
            public static LocalText Decimal = "Please enter a valid decimal value.";
            public static LocalText Digits = "Please only use digits 0-9.";
            public static LocalText Email = "Please enter a valid e-mail address.";
            public static LocalText EmailMultiple = "Please enter a valid e-mail address (if more than one, use ';' as separator)";
            public static LocalText EmailExists = "There is another user registered with this e-mail address. If you don't remember your password, use the forgot my password form.";
            public static LocalText HourAndMin = "Please enter a valid time (e.g. 15:30)";
            public static LocalText IncorrectPassword = "Incorrect password. Please check.";
            public static LocalText Integer = "Please enter a valid integer value.";
            public static LocalText MaxLength = "Please enter no more than {0} characters.";
            public static LocalText MinLength = "Please enter no less than {0} characters.";
            public static LocalText PasswordConfirm = "Password entered don't match.";
            public static LocalText Required = "This field is required.";
            public static LocalText Range = "Please enter a value between {0} and {1}.";
            public static LocalText Url = "Please enter a valid URL (e.g. http://www.site.com).";
            public static LocalText Username = "Not a valid username.";
            public static LocalText UsernameExists = "This username is already used. Please choose another.";
            public static LocalText Xss = "Invalid value! (don't use '&lt;' and '&amp;')";
        }
    }
}