using System.Runtime.CompilerServices;

namespace Serenity
{
    public static partial class Q
    {
        [Imported]
        public static class Authorization
        {
            [IntrinsicProperty]
            public static UserDefinition UserDefinition
            {
                [InlineCode("Q.Authorization.userDefinition")]
                get;
            }

            [IntrinsicProperty]
            public static string Username
            {
                [InlineCode("Q.Authorization.username")]
                get;
            }

            [IntrinsicProperty]
            public static string IsLoggedIn
            {
                [InlineCode("Q.Authorization.isLoggedIn")]
                get;
            }

            [InlineCode("Q.Authorization.hasPermission({permission})")]
            public static bool HasPermission(string permission)
            {
                return false;
            }


            [InlineCode("Q.Authorization.validatePermission({permission})")]
            public static void ValidatePermission(string permission)
            {
            }
        }
    }
}