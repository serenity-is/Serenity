using Serenity.Abstractions;
#if COREFX
using Microsoft.AspNetCore.Http;
using IDictionary = System.Collections.Generic.IDictionary<object, object>;
#else
using System.Collections;
using System.Web;
#endif

namespace Serenity.Web
{
    public class RequestContext : IRequestContext
    {
        public IDictionary Items
        {
            get 
            {
#if COREFX
                var context = Dependency.Resolve<IHttpContextAccessor>().HttpContext;
                if (context != null)
                    return context.Items;
#else
                if (HttpContext.Current != null)
                    return HttpContext.Current.Items;
#endif

                return null;
            }
        }
    }
}