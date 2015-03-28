using Serenity.Abstractions;
using System.Collections;
using System.Web;

namespace Serenity.Web
{
    public class RequestContext : IRequestContext
    {
        public IDictionary Items
        {
            get 
            {
                if (HttpContext.Current != null)
                    return HttpContext.Current.Items;

                return null;
            }
        }
    }
}