#if !ASPNETCORE
using Serenity.Web;
using Serenity.Web.HttpHandlers;
using System;
using System.IO;
using System.Web;
using System.Web.Hosting;

namespace Serenity.Web.HttpHandlers
{
    /// <summary>
    /// Legacy CSS Bundle handler
    /// </summary>
    public class BundleCssHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var request = context.Request;
            DynamicScriptManager.IfNotRegistered("BundleCss", () =>
            {
                DynamicScriptManager.Register("BundleCss",
                    new ConcatenatedScript(new Func<string>[] {
                        () => {
                            using (var sr = new StreamReader(
                                HostingEnvironment.MapPath("~/Content/bundle.css")))
                                return sr.ReadToEnd();
                        }
                    }));
            });

            DynamicScriptHandler.ProcessScriptRequest(context, "BundleCss", "text/css");
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}
#endif