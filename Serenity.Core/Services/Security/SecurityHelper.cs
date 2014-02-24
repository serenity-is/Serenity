using Serenity.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Security;
using UserIdName = System.Tuple<System.Int64, System.String>;

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
        public static readonly string MasterPassword;

        static SecurityHelper()
        {
            var s = ConfigurationManager.AppSettings["AdminUserId"].TrimToNull();
            Int64 i;
            if (s != null && Int64.TryParse(s, out i))
                AdminUserId = i;

            AdminUsername = ConfigurationManager.AppSettings["AdminUsername"].TrimToNull();
            MasterPassword = ConfigurationManager.AppSettings["MasterPassword"].TrimToNull();
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
        public static Int64? GetUserID(string username)
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
            return (AdminUsername != null && CurrentUsername == AdminUsername) || 
                   (AdminUserId != null && CurrentUserId == AdminUserId);
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

            if (CurrentUsername == null)
                return false;

            return HttpContext.Current.User.IsInRole(role);
        }

        /// <summary>
        ///   Checks if user is logged in.</summary>
        public static bool HttpContextLoggedIn
        {
            get
            {
                string loggedUser = HttpContextUsername;
                return (loggedUser != null && loggedUser.Length > 0);
            }
        }

        /// <summary>
        ///   Returns actual logged user, else if empty string or null.</summary>
        public static string HttpContextUsername
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

        public static Int64? HttpContextUserIdOrNull
        {
            get
            {
                string username = HttpContextUsername;
                if (username.IsNullOrEmpty())
                    return null;

                var user = IoC.Resolve<IUserRetrieveService>().ByUsername(username);
                if (user == null)
                    return null;

                return user.UserId;
            }
        }

        public static Int64 HttpContextUserId
        {
            get
            {
                var userId = HttpContextUserIdOrNull;
                if (userId == null)
                    throw new InvalidOperationException("Giriş yapmış bir kullanıcı yok!");

                return userId.Value;
            }
        }

        /// <summary>
        ///   Checks if user is logged in.</summary>
        public static bool IsLoggedIn
        {
            get
            {
                string loggedUser = CurrentUsername;
                return (loggedUser != null && loggedUser.Length > 0);
            }
        }

        public static string CurrentUsername
        {
            get
            {
                var username = ImpersonatedUsername;
                if (username != null)
                    return username;

                return HttpContextUsername;
            }
        }

        public static Int64 CurrentUserId
        {
            get
            {
                var userId = CurrentUserIdOrNull;
                if (userId == null)
                    throw new InvalidOperationException("Giriş yapmış bir kullanıcı yok!");

                return userId.Value;
            }
        }

        public static Int64? CurrentUserIdOrNull
        {
            get
            {
                var impersonated = ImpersonatedUserId;
                if (impersonated != null)
                    return impersonated;
                return HttpContextUserIdOrNull;
            }
        }

        private static Int64? ImpersonatedUserId
        {
            get
            {
                var stack = ContextItems.Get<List<UserIdName>>("ImpersonationStack", null);
                
                if (stack != null && stack.Count > 0)
                    return stack[stack.Count - 1].Item1;

                return null;
            }
        }

        private static string ImpersonatedUsername
        {
            get
            {
                var stack = ContextItems.Get<List<UserIdName>>("ImpersonationStack", null);

                if (stack != null && stack.Count > 0)
                    return stack[stack.Count - 1].Item2;

                return null;
            }
        }

        public static void Impersonate(Int64 userId, string username)
        {
            var stack = ContextItems.Get<List<UserIdName>>("ImpersonationStack", null);
            if (stack != null)
            {
                stack.Add(new UserIdName(userId, username));
                return;
            }
            else
            {
                stack = new List<UserIdName>();
                stack.Add(new UserIdName(userId, username));
                ContextItems.Set("ImpersonationStack", stack);
            }
        }

        public static void UndoImpersonate()
        {
            var stack = ContextItems.Get<List<UserIdName>>("ImpersonationStack", null);
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
                HandleRightError(errorHandling);
        }

        public static void EnsurePermission(string permission, RightErrorHandling errorHandling)
        {
            if (!HasPermission(permission))
                HandleRightError(errorHandling);
        }

        private static void HandleRightError(RightErrorHandling errorHandling)
        {
            bool isLoggedIn = SecurityHelper.HttpContextLoggedIn;

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