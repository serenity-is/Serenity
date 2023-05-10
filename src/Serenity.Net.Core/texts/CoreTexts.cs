namespace Serenity;

[NestedLocalTexts]
public static class CoreTexts
{
    public static class Authorization
    {
        public static readonly LocalText AccessDenied = "Authorization has been denied for this request!";
        public static readonly LocalText NotLoggedIn = "Authorization is required for this request!";
    }

    public static partial class Db
    {
        public static class Shared
        {
            public static readonly LocalText RecordId = "ID";
        }
    }

    public static partial class Services
    {
        public static readonly LocalText GenericErrorMessage = "An error occurred while processing your request.";
    }
}