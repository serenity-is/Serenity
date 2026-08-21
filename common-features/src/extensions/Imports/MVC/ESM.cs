namespace Serenity.Extensions.MVC;

/// <summary>Provides paths to generated ECMAScript module entry points.</summary>
public static partial class ESM
{
    /// <summary>The module path for the <c>ChangePasswordPage</c> entry point.</summary>
    public const string ChangePasswordPage = "~/Serenity.Extensions/esm/Modules/Membership/PasswordActions/ChangePasswordPage.js";
    /// <summary>The module path for the <c>ForgotPasswordPage</c> entry point.</summary>
    public const string ForgotPasswordPage = "~/Serenity.Extensions/esm/Modules/Membership/PasswordActions/ForgotPasswordPage.js";
    /// <summary>The module path for the <c>ReportPage</c> entry point.</summary>
    public const string ReportPage = "~/Serenity.Extensions/esm/Modules/Reporting/ReportPage.js";
    /// <summary>The module path for the <c>ResetPasswordPage</c> entry point.</summary>
    public const string ResetPasswordPage = "~/Serenity.Extensions/esm/Modules/Membership/PasswordActions/ResetPasswordPage.js";
    /// <summary>The module path for the <c>SetPasswordPage</c> entry point.</summary>
    public const string SetPasswordPage = "~/Serenity.Extensions/esm/Modules/Membership/PasswordActions/SetPasswordPage.js";

    /// <summary>Provides module entry points in the <c>Modules</c> folder.</summary>
    public static partial class Modules
    {
        /// <summary>Provides module entry points in the <c>Modules/Membership</c> folder.</summary>
        public static partial class Membership
        {
            /// <summary>Provides module entry points in the <c>Modules/Membership/PasswordActions</c> folder.</summary>
            public static partial class PasswordActions
            {
                /// <summary>The module path for the <c>ChangePasswordPage</c> entry point.</summary>
                public const string ChangePasswordPage = "~/Serenity.Extensions/esm/Modules/Membership/PasswordActions/ChangePasswordPage.js";
                /// <summary>The module path for the <c>ForgotPasswordPage</c> entry point.</summary>
                public const string ForgotPasswordPage = "~/Serenity.Extensions/esm/Modules/Membership/PasswordActions/ForgotPasswordPage.js";
                /// <summary>The module path for the <c>ResetPasswordPage</c> entry point.</summary>
                public const string ResetPasswordPage = "~/Serenity.Extensions/esm/Modules/Membership/PasswordActions/ResetPasswordPage.js";
                /// <summary>The module path for the <c>SetPasswordPage</c> entry point.</summary>
                public const string SetPasswordPage = "~/Serenity.Extensions/esm/Modules/Membership/PasswordActions/SetPasswordPage.js";
            }
        }

        /// <summary>Provides module entry points in the <c>Modules/Reporting</c> folder.</summary>
        public static partial class Reporting
        {
            /// <summary>The module path for the <c>ReportPage</c> entry point.</summary>
            public const string ReportPage = "~/Serenity.Extensions/esm/Modules/Reporting/ReportPage.js";
        }
    }
}