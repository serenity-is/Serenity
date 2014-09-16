using System;
using System.IO;
using System.Web.Mvc;
using Newtonsoft.Json;
using Serenity.Data;
using System.Web;
using System.Web.Security;
using System.Security.Principal;

namespace Serenity.Services
{
    public class ServiceAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new Result<ServiceResponse>(new ServiceResponse
            {
                Error = new ServiceError
                {
                    Code = "NotLoggedIn",
                    Message = "Bu işlem için giriş yapmış olmalısınız!"
                }
            });
        }
    }
}