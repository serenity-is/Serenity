using Serenity.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Security;

namespace Serenity
{
    public enum RightErrorHandling
    {
        ThrowException,
        Redirect,
        RedirectIfNotSignedIn
    }

    public interface IPermissionService
    {
        bool HasPermission(string permission);
    }

    /// <summary>
    ///   Static class contains helper functions associated with user rights, login, encrypting </summary>
    public static class SecurityHelper
    {
        public static readonly Int64? AdminUserId;
        public static readonly string AdminUsername;

        static SecurityHelper()
        {
            var s = ConfigurationManager.AppSettings["AdminUserId"].TrimToNull();
            Int64 i;
            if (s != null && Int64.TryParse(s, out i))
                AdminUserId = i;

            AdminUsername = ConfigurationManager.AppSettings["AdminUsername"].TrimToNull();
        }

        /// <summary>
        ///   Validate user identity by checking username and password and sets 
        ///   authentication ticket that is based on cookie  
        /// </summary>
        /// <param name="username">
        ///   Username to be validated (required).</param>
        /// <param name="password">
        ///   User Password to be validated(required).</param>
        /// <param name="persist">
        ///   true to make ticket persistent? (beni hatırla seçeneği, güvenlik açısından pek kullanmıyoruz.)</param>
        /// <returns>
        ///   if validation is successful,returns true and sets ticket. if it is invalid, returns only false
        ///   ,doesn't change current ticket.</returns>
        public static bool Authenticate(string username, string password, bool persist)
        {
            if (username == null)
                throw new ArgumentNullException("username");
            if (password == null)
                throw new ArgumentNullException("password");

            if (!Membership.ValidateUser(username, password))
                return false;

            SetAuthenticationTicket(username, persist, Roles.GetRolesForUser(username));
            return true;
        }

        /// <summary>
        ///   Sets authentication cookie.</summary>
        /// <param name="username">
        ///   Validated Username (required).</param>
        /// <param name="persist">
        ///   is persistent authentication tikcet? (remember me, we don't use this for reasons considering with security)</param>
        /// <param name="roles">
        ///   Roles users has. Persisted in cookie for quick access</param>
        public static void SetAuthenticationTicket(string username, bool persist, params string[] roles)
        {
            if (username == null)
                throw new ArgumentNullException(username);

            HttpCookie authCookie = FormsAuthentication.GetAuthCookie(username, persist);
            FormsAuthenticationTicket tempTicket = FormsAuthentication.Decrypt(authCookie.Value);

            string userData = string.Join("|", roles);

            var cookiePath = HttpContext.Current.Request.ApplicationPath;
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                tempTicket.Version,
                tempTicket.Name,
                tempTicket.IssueDate,
                tempTicket.Expiration,
                persist,
                userData,
                cookiePath);
            authCookie.Value = FormsAuthentication.Encrypt(authTicket);
            authCookie.Name = FormsAuthentication.FormsCookieName;
            authCookie.Path = cookiePath;

            HttpContext.Current.Response.Cookies.Remove(authCookie.Name);
            HttpContext.Current.Response.Cookies.Add(authCookie);
        }

        private static string[] EmptyStringArray = new string[0];

        /// <summary>
        ///   Gets list of roles for current user from authentication cookie</summary>
        /// <returns>
        ///   An array of user roles.</returns>
        public static string[] GetAuthenticatedRoles()
        {
            var context = HttpContext.Current;
            if (context != null)
            {
                HttpCookie authCookie = context.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie != null && 
                    authCookie.Value != null && 
                    authCookie.Value.Length > 0)
                {
                    var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    if (authTicket.UserData != null)
                        return authTicket.UserData.Split('|');
                }
            }

            return EmptyStringArray;
        }

        /// <summary>
        ///   Logs out to logged user.</summary>
        public static void LogOut()
        {
            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName);
            // Setting up a cookie which has expired, Enforce client to delete this cookie.
            authCookie.Expires = DateTime.Now.AddYears(-30);
            //authCookie.Path = UrlHelper.GetApplicationRootPath();

            // bu path /site olmalı, /site/ olduğunda eğer http://sunucu/site yazılırsa path cookie bu yola uygulanmıyor, sürekli login gerekiyor!
            authCookie.Path = HttpContext.Current.Request.ApplicationPath;
            HttpContext.Current.Response.Cookies.Add(authCookie);
            //FormsAuthentication.SignOut();
        }

        /// <summary>
        ///   Gets user unique ID (as GUID).</summary>
        /// <param name="username">
        ///   Username, its ID will be found (required).</param>
        /// <returns>
        ///   Kullanıcı bulunursa ID'si. Bulunamazsa Guid.Empty.</returns>
        /// <returns>
        ///   Returns user id if it is found, else Guid.Empty .</returns>
        public static Int32? GetUserID(string username)
        {
            if (username == null)
                throw new ArgumentNullException("username");

            var user = IoC.Resolve<IUserRetrieveService>().ByUsername(username);
            if (user == null)
                return null;
            else
                return user.UserId;
        }

        /// <summary>
        ///   Checks whether current user is admin or not</summary>
        /// <summary>
        ///   Allows Active .</summary>
        /// <returns>
        ///   if it is "Admin" returns true.</returns>
        public static bool IsAdmin()
        {
            return (AdminUsername != null && LoggedUser == AdminUsername) || 
                   (AdminUserId != null && ImpersonatedUserId == AdminUserId);
        }

        /// <summary>
        ///   Checks whether given username is admin</summary>
        /// <param name="username">
        ///   Username will be checked.</param>
        /// <returns>
        ///   Returns true if given user is admin.</returns>
        public static bool IsAdmin(string username)
        {
            return AdminUsername != null && 
                username == AdminUsername;
        }

        /// <summary>
        ///   Indicates whether logged user is in given role or not .</summary>
        /// <param name="role">
        ///   Role will be checked (required).</param>
        /// <returns>
        ///   Returns true if the user in role</returns>
        public static bool IsInRole(string role)
        {
            if (role == null || role.Length == 0)
                throw new ArgumentNullException("role");

            if (LoggedUser == null)
                return false;

            return HttpContext.Current.User.IsInRole(role);
        }

        /// <summary>
        ///   Checks if user is logged in.</summary>
        public static bool IsLoggedIn
        {
            get
            {
                string loggedUser = LoggedUser;
                return (loggedUser != null && loggedUser.Length > 0);
            }
        }

        /// <summary>
        ///   Returns logged user, else if empty string or null.</summary>
        public static string LoggedUser
        {
            get
            {
                if (HttpContext.Current != null && 
                    HttpContext.Current.Request != null &&
                    HttpContext.Current.Request.IsAuthenticated)
                {
                    try
                    {
                        return HttpContext.Current.User.Identity.Name;
                    }
                    catch
                    { 
                    }
                }
                return null;
            }
        }

        public static Int32 CurrentUserId
        {
            get
            {
                var userId = CurrentUserIdOrNull;
                if (userId == null)
                    throw new InvalidOperationException("Giriş yapmış bir kullanıcı yok!");

                return userId.Value;
            }
        }

        public static Int32 ActualUserId
        {
            get
            {
                var userId = ActualUserIdOrNull;
                if (userId == null)
                    throw new InvalidOperationException("Giriş yapmış bir kullanıcı yok!");

                return userId.Value;
            }
        }

        public static Int32? ActualUserIdOrNull
        {
            get
            {
                string username = LoggedUser;
                if (username.IsEmptyOrNull())
                    return null;

                var user = IoC.Resolve<IUserRetrieveService>().ByUsername(username);
                if (user == null)
                    return null;

                return user.UserId;
            }
        }

        public static Int32? CurrentUserIdOrNull
        {
            get
            {
                var impersonated = ImpersonatedUserId;
                if (impersonated != null)
                    return impersonated;
                return ActualUserIdOrNull;
            }
        }

        private static Int32? ImpersonatedUserId
        {
            get
            {
                var stack = ContextItems.Get<List<Int32>>("ImpersonationStack", null);
                if (stack != null && stack.Count > 0)
                    return stack[stack.Count - 1];
                return null;
            }
        }

        public static void Impersonate(Int32 userId)
        {
            var stack = ContextItems.Get<List<Int32>>("ImpersonationStack", null);
            if (stack != null)
            {
                stack.Add(userId);
                return;
            }
            else
            {
                stack = new List<int>();
                stack.Add(userId);
                ContextItems.Set("ImpersonationStack", stack);
            }
        }

        public static void UndoImpersonate()
        {
            var stack = ContextItems.Get<List<Int32>>("ImpersonationStack", null);
            if (stack != null && stack.Count > 0)
                stack.RemoveAt(stack.Count - 1);
        }

        public static bool HasPermission(string permission)
        {
            return IoC.Resolve<IPermissionService>().HasPermission(permission);
        }

        public static void EnsureLoggedIn(RightErrorHandling errorHandling)
        {
            if (!IsLoggedIn)
                HandleRightError(RightErrorHandling.Redirect);
        }

        public static void EnsurePermission(string permission, RightErrorHandling errorHandling)
        {
            if (!HasPermission(permission))
                HandleRightError(errorHandling);
        }

        private static void HandleRightError(RightErrorHandling errorHandling)
        {
            bool isLoggedIn = SecurityHelper.IsLoggedIn;

            switch (errorHandling)
            {
                case RightErrorHandling.RedirectIfNotSignedIn:
                    if (!isLoggedIn)
                        FormsAuthentication.RedirectToLoginPage();
                    else
                        throw new ValidationError("NotLoggedIn", null, "Bu işlem için giriş yapmış olmalısınız!");
                    break;
                case RightErrorHandling.Redirect:
                    FormsAuthentication.RedirectToLoginPage();
                    break;
                default:
                    if (isLoggedIn)
                        throw new ValidationError("AccessDenied", null, "Bu işlem için gerekli haklara sahip değilsiniz!");
                    else
                        throw new ValidationError("NotLoggedIn", null, "Bu işlem için giriş yapmış olmalısınız!");
            }
        }
    }
}