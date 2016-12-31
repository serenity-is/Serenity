using System;
using Serenity.Abstractions;
#if ASPNETCORE
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Principal;
#else
using System.Web;
using System.Web.Security;
#endif

namespace Serenity
{

    /// <summary>
    ///   Static class contains helper functions associated with user rights, login, encrypting </summary>
    public static class WebSecurityHelper
    {

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
        public static bool Authenticate(ref string username, string password, bool persist)
        {
            if (username == null)
                throw new ArgumentNullException("username");
            if (password == null)
                throw new ArgumentNullException("password");

            if (!Dependency.Resolve<IAuthenticationService>().Validate(ref username, password))
                return false;
            
            SetAuthenticationTicket(username, persist);
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
        public static void SetAuthenticationTicket(string username, bool persist)
        {
            if (username == null)
                throw new ArgumentNullException(username);

#if ASPNETCORE
            var principal = new GenericPrincipal(new GenericIdentity(username), EmptyStringArray);
            var httpContext = Dependency.Resolve<IHttpContextAccessor>().HttpContext;
            httpContext.Authentication.SignInAsync("CookieAuthentication", principal).Wait();
#else
            HttpCookie authCookie = FormsAuthentication.GetAuthCookie(username, persist);
            HttpContext.Current.Response.Cookies.Remove(authCookie.Name);
            HttpContext.Current.Response.Cookies.Add(authCookie);
#endif
        }

        private static string[] EmptyStringArray = new string[0];

        /// <summary>
        ///   Logs out to logged user.</summary>
        public static void LogOut()
        {
#if ASPNETCORE
            var httpContext = Dependency.Resolve<IHttpContextAccessor>().HttpContext;
            httpContext.Authentication.SignOutAsync("CookieAuthentication").Wait();
#else
            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName);
            // Setting up a cookie which has expired, Enforce client to delete this cookie.
            authCookie.Expires = DateTime.Now.AddYears(-30);
            //authCookie.Path = UrlHelper.GetApplicationRootPath();

            // bu path /site olmalı, /site/ olduğunda eğer http://sunucu/site yazılırsa path cookie bu yola uygulanmıyor, sürekli login gerekiyor!
            authCookie.Path = HttpContext.Current.Request.ApplicationPath;
            HttpContext.Current.Response.Cookies.Add(authCookie);
            //FormsAuthentication.SignOut();
#endif
        }

#if !ASPNETCORE
        public static void EnsurePermission(string permission)
        {
            if (!Authorization.HasPermission(permission))
                FormsAuthentication.RedirectToLoginPage(
                    Authorization.IsLoggedIn ? "denied=1" :null);
        }
#endif

        /// <summary>
        ///   Returns actual logged user, else if empty string or null.</summary>
        public static string HttpContextUsername
        {
            get
            {
#if ASPNETCORE
                var httpContext = Dependency.Resolve<IHttpContextAccessor>().HttpContext;
                if (httpContext == null)
                    return null;
                try
                {
                    return httpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                }
                catch
                {
                    return null;
                }
#else
                var httpContext = HttpContext.Current;
                if (httpContext != null &&
                    httpContext.Request != null &&
                    httpContext.Request.IsAuthenticated)
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
#endif
            }
        }
    }
}