using System.Configuration;

namespace Serenity.Web
{
    public static class WebApplicationSettings
    {
        public static bool IsDevelopmentMode { get; set; }
        public static bool HideExceptionDetails { get; set; }

        static WebApplicationSettings()
        {
            IsDevelopmentMode = ConfigurationManager.AppSettings["IsDevelopmentMode"] == "1";
            HideExceptionDetails = ConfigurationManager.AppSettings["HideExceptionDetails"] == "1";
        }
    }
}