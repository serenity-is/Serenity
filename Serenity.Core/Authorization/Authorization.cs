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
                var service = Dependency.TryResolve<IAuthorizationService>();
                return service != null && service.IsLoggedIn;
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

        public static Int64? UserId
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
                var service = Dependency.TryResolve<IAuthorizationService>();
                return service != null ? service.Username : null;
            }
        }

        public static bool HasPermission(string permission)
        {
            var service = Dependency.TryResolve<IPermissionService>();
            return service != null && service.HasPermission(permission);
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