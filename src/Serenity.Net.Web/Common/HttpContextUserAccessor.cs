using Microsoft.AspNetCore.Http;
using Serenity.Abstractions;
using System.Security.Claims;

namespace Serenity.Web
{
    public class HttpContextUserAccessor : IUserAccessor
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public HttpContextUserAccessor(IHttpContextAccessor httpContextAccessor = null)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal User => httpContextAccessor?.HttpContext?.User;
    }
}

