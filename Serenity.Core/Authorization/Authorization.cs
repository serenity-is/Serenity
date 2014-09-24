using Serenity.Abstractions;
using Serenity.Services;
using System;

namespace Serenity
{
    /// <summary>
    /// Provides a common access point for authorization related services
    /// </summary>
    public class Authorization
    {
        /// <summary>
        /// Returns true if user is logged in.
        /// </summary>
        /// <remarks>
        /// Uses the IAuthorizationService dependency.
        /// </remarks>
        public static bool IsLoggedIn
        {
            get
            {
                return Dependency.Resolve<IAuthorizationService>().IsLoggedIn;
            }
        }

        /// <summary>
        /// Returns user definition for currently logged user.
        /// </summary>
        /// <remarks>
        /// Uses IUserRetrieveService to get definition of current user by
        /// its username.
        /// </remarks>
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

        /// <summary>
        /// Returns currently logged user ID
        /// </summary>
        /// <remarks>
        /// This is a shortcut to UserDefinition.UserId.
        /// </remarks>
        public static string UserId
        {
            get
            {
                var user = UserDefinition;
                return user == null ? null : user.Id;
            }
        }

        /// <summary>
        /// Returns currently logged username.
        /// </summary>
        /// <remarks>Uses IAuthorizationService dependency.</remarks>
        public static string Username
        {
            get
            {
                return Dependency.Resolve<IAuthorizationService>().Username;
            }
        }

        /// <summary>
        /// Returns true if current user has given permission.
        /// </summary>
        /// <param name="permission">Permission key (e.g. Administration)</param>
        public static bool HasPermission(string permission)
        {
            return Dependency.Resolve<IPermissionService>().HasPermission(permission);
        }

        /// <summary>
        /// Checks if there is a currently logged user and throws a validation error with
        /// "NotLoggedIn" error code if not.
        /// </summary>
        public static void ValidateLoggedIn()
        {
            if (!IsLoggedIn)
                throw new ValidationError("NotLoggedIn", null, Serenity.Core.Texts.Authorization.NotLoggedIn);
        }

        /// <summary>
        /// Checks if current user has given permission and throws a validation error with 
        /// "AccessDenied" error code if not.
        /// </summary>
        /// <param name="permission"></param>
        public static void ValidatePermission(string permission)
        {
            if (!HasPermission(permission))
                throw new ValidationError("AccessDenied", null, Serenity.Core.Texts.Authorization.AccessDenied);
        }
    }
}