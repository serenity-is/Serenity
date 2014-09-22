using Serenity.Abstractions;
using Serenity.Services;
using System;

namespace Serenity
{
    public class Authorization
    {
        public static bool IsLoggedIn
        {
            get
            {
                return Dependency.Resolve<IAuthorizationService>().IsLoggedIn;
            }
        }

        public static IUserDefinition UserDefinition
        {
            get
            {
                var username = Username;
                if (username == null)
                    return null;

                return Dependency.Resolve<IUserRetrieveService>().ByUsername(username);
            }
        }

        public static long? UserId
        {
            get
            {
                var user = UserDefinition;
                return user == null ? (Int64?)null : user.UserId;
            }
        }

        public static string Username
        {
            get
            {
                return Dependency.Resolve<IAuthorizationService>().Username;
            }
        }

        public static bool HasPermission(string permission)
        {
            return Dependency.Resolve<IPermissionService>().HasPermission(permission);
        }

        public static void ValidateLoggedIn()
        {
            if (!IsLoggedIn)
                throw new ValidationError("NotLoggedIn", null, Serenity.Core.Texts.Authorization.NotLoggedIn);
        }

        public static void ValidatePermission(string permission)
        {
            if (!HasPermission(permission))
                throw new ValidationError("AccessDenied", null, Serenity.Core.Texts.Authorization.AccessDenied);
        }
    }
}