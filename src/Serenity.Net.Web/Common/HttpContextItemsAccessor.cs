using Microsoft.AspNetCore.Http;
using Serenity.Abstractions;
using System.Collections.Generic;

namespace Serenity.Web
{
    public class HttpContextItemsAccessor : IHttpContextItemsAccessor
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public HttpContextItemsAccessor(IHttpContextAccessor httpContextAccessor = null)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public IDictionary<object, object> Items
        {
            get 
            {
                return httpContextAccessor?.HttpContext?.Items;
            }
        }
    }
}

