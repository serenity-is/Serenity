namespace Serenity.Extensions;

[NestedLocalTexts]
public static class ExtensionsTexts
{
    public static class Forms
    {
        public static class Membership
        {
            public static class ChangePassword
            {
                public static LocalText FormTitle => ChangePasswordFormTexts.FormTitle;
            }
        }
    }

    public static class Site
    {
        public static class Translation
        {
            public static LocalText EntityPlural => TranslationTexts.EntityPlural;
        }
    }

    public static class Validation
    {
        public static readonly LocalText InvalidResetToken = "Your token to reset your password is invalid or has expired!";
        public static readonly LocalText PasswordConfirmMismatch = "The passwords entered doesn't match!";
    }
}