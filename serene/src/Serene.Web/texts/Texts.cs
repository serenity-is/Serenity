namespace Serene;

[NestedLocalTexts]
public static partial class Texts
{
    public static class Db
    {
        public static class Administration
        {
            public static class Translation
            {
                public static readonly LocalText EntityPlural = "Translations";
                public static readonly LocalText Key = "Local Text Key";
                public static readonly LocalText SourceLanguage = "Source Language";
                public static readonly LocalText SourceText = "Effective Translation in Source Language";
                public static readonly LocalText TargetLanguage = "Target Language";
                public static readonly LocalText TargetText = "Effective Translation in Target Language";
                public static readonly LocalText CustomText = "User Translation in Target Language";
                public static readonly LocalText OverrideConfirmation = "Overwrite user translation with clicked text?";
                public static readonly LocalText SaveChangesButton = "Save Changes";
            }
        }
    }

    public static class Forms
    {
        public static class Membership
        {
            public static class Login
            {
                public static readonly LocalText ForgotPassword = "Forgot password?";
                public static readonly LocalText LoginToYourAccount = "Login to your account";
                public static readonly LocalText RememberMe = "Remember Me";
                public static readonly LocalText SignInButton = "Sign In";
                public static readonly LocalText SignUpButton = "Don't have account yet? Sign up.";
            }

            public static class SendActivation
            {
                public static readonly LocalText FormTitle = "Send Activation Mail";
                public static readonly LocalText Success = "Activation mail sent to your email address";
                public static readonly LocalText SubmitButton = "Send Activation";
                public static readonly LocalText FormInfo = "Please enter the email you used to sign up.";
            }

            public static class SignUp
            {
                public static readonly LocalText ActivateEmailSubject = "Activate Your Serene Account";
                public static readonly LocalText ActivationCompleteMessage = "Your account is now activated. " +
                    "Use the email and password you used while signing up to login.";
                public static readonly LocalText FormInfo = "Enter your details to create a free account.";
                public static readonly LocalText FormTitle = "Create a new account";
                public static readonly LocalText SubmitButton = "Sign Up";
                public static readonly LocalText Success = "An email with instructions to activate your account has been " +
                    "sent to your email address. Please check your emails.";

                public static readonly LocalText DisplayName = "Full Name";
                public static readonly LocalText Email = "Email";
                public static readonly LocalText ConfirmEmail = "Confirm Email";
                public static readonly LocalText Password = "Password";
                public static readonly LocalText ConfirmPassword = "Confirm Password";
            }
        }

        public static readonly LocalText SiteTitle = "Serene";
    }

    public static class Navigation
    {
        public static readonly LocalText Dashboard = "Dashboard";
        public static readonly LocalText LogoutLink = "Logout";
        public static readonly LocalText SiteTitle = "Serene";
    }

    public static class Site
    {
        public static class AccessDenied
        {
            public static readonly LocalText PageTitle = "Unauthorized Access";
            public static readonly LocalText LackPermissions = "You don't have the required permissions to access this page!";
            public static readonly LocalText NotLoggedIn = "You need to be logged in to access this page!";
            public static readonly LocalText ClickToChangeUser = "click here to login with another user...";
            public static readonly LocalText ClickToLogin = "click here to return to login page...";
        }

        public static class UserDialog
        {
            public static readonly LocalText EditPermissionsButton = "Edit Permissions";
            public static readonly LocalText EditRolesButton = "Edit Roles";
        }

        public static class UserPermissionDialog
        {
            public static readonly LocalText DialogTitle = "Edit User Permissions ({0})";
            public static readonly LocalText SaveSuccess = "Updated user permissions.";
            public static readonly LocalText Permission = "Permission";
            public static readonly LocalText Grant = "Grant";
            public static readonly LocalText Revoke = "Revoke";
        }

        public static class RolePermissionDialog
        {
            public static readonly LocalText EditButton = "Edit Permissions";
            public static readonly LocalText DialogTitle = "Edit Role Permissions ({0})";
            public static readonly LocalText SaveSuccess = "Updated role permissions.";
        }

        public static class Layout
        {
            public static readonly LocalText Language = "Language";
            public static readonly LocalText Theme = "Theme";
        }

        public static class ValidationError
        {
            public static readonly LocalText Title = "ERROR";
        }
    }

    public static partial class Validation
    {
        public static readonly LocalText AuthenticationError = "Invalid username or password!";
        public static readonly LocalText CurrentPasswordMismatch = "Your current password is not valid!";
        public static readonly LocalText MinRequiredPasswordLength = "Entered password doesn't have enough characters (min {0})!";
        public static readonly LocalText PasswordConfirmMismatch = "The passwords entered don't match!";
        public static readonly LocalText InvalidResetToken = "Your token to reset your password is invalid or has expired!";
        public static readonly LocalText InvalidActivateToken = "Your token to activate your account is invalid or has expired!";
        public static readonly LocalText EmailInUse = "A user with this email already exists!";
        public static readonly LocalText EmailConfirm = "Emails entered don't match!";
        public static readonly LocalText DeleteForeignKeyError = "Can't delete record. '{0}' table has " +
            "records that depend on this one!";
        public static readonly LocalText SavePrimaryKeyError = "Can't save record. There is another record with the same {1} value!";
    }
}
