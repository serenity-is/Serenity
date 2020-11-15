using Serenity.Extensibility;

namespace Serenity.Core
{
    [NestedLocalTexts]
    internal static class Texts
    {
        public static class Authorization
        {
            public static LocalText AccessDenied = "Authorization has been denied for this request!";
            public static LocalText NotLoggedIn = "Authorization is required for this request!";
        }

        public static partial class Db
        {
            public static class Shared
            {
                public static LocalText RecordId = "ID";
            }
        }
    }
}