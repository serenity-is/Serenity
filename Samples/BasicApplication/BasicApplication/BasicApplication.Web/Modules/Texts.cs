using Serenity;
using Serenity.Extensibility;

namespace BasicApplication
{
    [NestedLocalTexts]
    public static partial class Texts
    {
        public static class Db
        {
            public static class Administration
            {
                public static class Translation
                {
                    public static LocalText EntityPlural = "Translations";
                    public static LocalText Key = "Local Text Key";
                    public static LocalText SourceLanguage = "Source Language";
                    public static LocalText SourceText = "Effective Translation in Source Language";
                    public static LocalText TargetLanguage = "Target Language";
                    public static LocalText TargetText = "Effective Translation in Target Language";
                    public static LocalText CustomText = "User Translation in Target Language";
                    public static LocalText OverrideConfirmation = "Overwrite user translation with clicked text?";
                    public static LocalText SaveChangesButton = "Save Changes";
                }
            }
        }

        public static class Forms
        {
            public static class Membership
            {
                public static class Login
                {
                    public static LocalText FormTitle = "WELCOME TO SERENITY BASIC APPLICATION";
                    public static LocalText SignInButton = "Sign In";
                }
            }
        }

        public static class Navigation
        {
            public static LocalText LogoutLink = "Logout";
            public static LocalText SiteTitle = "Serenity Sample Site";
        }

        public static class Site
        {
            public static class Dashboard
            {
                public static LocalText WelcomeMessage = "Welcome to Serenity BasicApplication home page. " + 
                    "Use the navigation on left to browse other pages...";
            }
        }

        public static class Validation
        {
            public static LocalText DeleteForeignKeyError = "Can't delete record. '{0}' table has " +
                "records that depends on this one!";
            public static LocalText NorthwindPhone = "Phone numbers should be entered in format '(503) 555-9831'.";
            public static LocalText NorthwindPhoneMultiple = "Phone numbers should be entered in format '(503) 555-9831. " +
                "Multiple numbers can be separated with comma.";
            public static LocalText SavePrimaryKeyError = "Can't save record. There is another record with the same {1} value!";
        }
    }
}