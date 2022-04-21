using Microsoft.AspNetCore.Http;

namespace Serenity.Web
{
    public class HttpContextItemsAccessor : IHttpContextItemsAccessor
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public HttpContextItemsAccessor(IHttpContextAccessor httpContextAccessor = null)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public IDictionary<object, object> Items => httpContextAccessor?.HttpContext?.Items;
    }
}

