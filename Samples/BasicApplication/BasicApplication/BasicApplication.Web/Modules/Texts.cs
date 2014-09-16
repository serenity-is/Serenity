using Serenity;
using Serenity.Extensibility;

namespace BasicApplication
{
    [NestedLocalTexts]
    public static partial class Texts
    {
        public static class Site
        {
            public static class Dashboard
            {
                public static LocalText WelcomeMessage = "Welcome to Serenity BasicApplication home page. Use the navigation on left to browse other pages...";
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
    }
}