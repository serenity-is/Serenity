using Serenity.Abstractions;
using Serenity.Services;
using System;
using System.Security.Claims;

namespace Serenity
{
    /// <summary>
    /// Authorization extension methods
    /// </summary>
    public static class AuthorizationExtensions
    {
        /// <summary>
        /// Returns true if user is logged in (authenticated).
        /// </summary>
        public static bool IsLoggedIn(this IUserAccessor userAccessor)
        {
            return userAccessor?.User?.Identity?.IsAuthenticated == true;
        }

        /// <summary>
        /// Returns true if user is logged in (authenticated).
        /// </summary>
        public static bool IsLoggedIn(this ClaimsPrincipal user)
        {
            return user?.Identity?.IsAuthenticated == true;
        }

        /// <summary>
        /// Checks if current user has given permission and throws a validation error with 
        /// "AccessDenied" error code if not.
        /// </summary>
        /// <param name="permissions">Permissions service</param>
        /// <param name="permission">Permission key</param>
        /// <param name="localizer">Localizer</param>
        public static void ValidatePermission(this IPermissionService permissions, 
            string permission, ITextLocalizer localizer)
        {
            if (permissions == null)
                throw new ArgumentNullException(nameof(permissions));

            if (!permissions.HasPermission(permission))
                throw new ValidationError("AccessDenied", null, 
                    Core.Texts.Authorization.AccessDenied.ToString(localizer));
        }

        /// <summary>
        /// Checks if there is a currently logged user and throws a validation error with
        /// "NotLoggedIn" error code if not.
        /// </summary>
        public static void ValidateLoggedIn(this IUserAccessor userAccessor, ITextLocalizer localizer)
        {
            if (!IsLoggedIn(userAccessor))
                throw new ValidationError("NotLoggedIn", null, 
                    Core.Texts.Authorization.NotLoggedIn.ToString(localizer));
        }

        /// <summary>
        /// Gets name identifier claim from given identity
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static string GetIdentifier(this ClaimsPrincipal identity)
        {
            if (identity == null)
                throw new ArgumentNullException(nameof(identity));

            return identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}