
namespace BasicApplication
{
    using System;

    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            SiteInitialization.ApplicationStart();
        }

        protected void Application_End(object sender, EventArgs e)
        {
            SiteInitialization.ApplicationEnd();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
        }
    }
}