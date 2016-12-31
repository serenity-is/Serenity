#if !COREFX
using System;
using System.IO;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Web;
using System.Web.Security;
using System.Security.Principal;

namespace Serenity.Services
{
    public class TokenAuthorizeAttribute : AuthorizeAttribute
    {
        public string FormKey { get; set; }
        public string ActionArgument { get; set; }

        private class RequestWithToken
        {
            public string AuthenticationToken { get; set; }
        }

        public TokenAuthorizeAttribute()
        {
            ActionArgument = "request";
            FormKey = "token";
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthenticated = base.AuthorizeCore(httpContext);
            if (!isAuthenticated)
            {
                var request = httpContext.Request;

                string token = null;

                if (!FormKey.IsNullOrEmpty())
                    token = httpContext.Request[FormKey].TrimToNull();

                if (token == null)
                {
                    string method = request.HttpMethod ?? "";

                    if (!ActionArgument.IsNullOrEmpty() &&
                        (method.Equals("POST", StringComparison.InvariantCultureIgnoreCase) ||
                         method.Equals("PUT", StringComparison.InvariantCultureIgnoreCase)) &&
                         (request.ContentType ?? string.Empty).Contains("application/json"))
                    {
                        if (httpContext.Request.InputStream.CanSeek)
                            httpContext.Request.InputStream.Seek(0, SeekOrigin.Begin);

                        using (var sr = new StreamReader(httpContext.Request.InputStream,
                            httpContext.Request.ContentEncoding, true, 4096, true))
                        {
                            var js = JsonSerializer.Create(JsonSettings.Tolerant);
                            using (var jr = new JsonTextReader(sr))
                            {
                                var obj = js.Deserialize<RequestWithToken>(jr);
                                token = obj.AuthenticationToken.TrimToNull();
                            }
                        }
                    }
                }

                if (token != null)
                {
                    var ticket = FormsAuthentication.Decrypt(token);
                    if (!ticket.Expired)
                    {
                        var principal = new GenericPrincipal(new GenericIdentity(ticket.Name), new string[] { });
                        httpContext.User = principal;
                        isAuthenticated = true;
                    }
                }
            }

            return isAuthenticated;
        }
    }
}
#endif