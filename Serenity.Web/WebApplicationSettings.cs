using Serenity.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;

namespace Serenity.Web
{
    public static class WebApplicationSettings
    {
        public static UploadMappingSettings UploadMapping;
        public static int BuildVersionNumber { get; set; }
        public static bool IsDevelopmentMode { get; set; }
        public static bool HideExceptionDetails { get; set; }

        /// <summary>
        ///   Static constructor.</summary>
        static WebApplicationSettings()
        {
            var settings = ConfigurationManager.AppSettings;
            IsDevelopmentMode = ToBool(settings.Get("IsDevelopmentMode"));
            HideExceptionDetails = ToBool(settings.Get("HideExceptionDetails"));

            UploadMapping = JsonConvert.DeserializeObject<UploadMappingSettings>(settings["UploadMapping"].TrimToNull() ?? "{}", JsonSettings.Strict);
            var p = JsonConvert.DeserializeObject<IDictionary<string, object>>(settings["UploadRoot"].TrimToNull() ?? "{}", JsonSettings.Strict);

            UploadRoot.Path = Relative((string)(p.GetDefault("path", String.Empty)));
            UploadRoot._url = (string)(p.GetDefault("url", String.Empty));
            UploadRoot._urlSecure = UploadRoot._url.Replace("http://", "https://");
        }
        
        ///   Dosya yükleme ortak bir ağ klasörüne yapılacaksa ayarları</summary>       
        public class UploadMappingSettings
        {
            /// <summary>
            ///   Dosya yükleme ortak bir ağ klasörüne yapılacaksa atanacak sürücü ismi</summary>
            public string Drive;
            /// <summary>
            ///   Dosya yükleme ortak bir ağ klasörüne yapılacaksa network konumu</summary>
            public string Path;
            /// <summary>
            ///   Dosya yükleme ortak bir ağ klasörüne yapılacaksa kullanıcı adı</summary>          
            public string User;
            /// <summary>
            ///   Dosya yükleme ortak bir ağ klasörüne yapılacaksa şifre</summary>
            public string Pass;
        }

        /// <summary>
        ///   Settings for upload root (where user files are uploaded)</summary>
        public static class UploadRoot
        {
            internal static string _url;
            internal static string _urlSecure;

            /// <summary>
            ///   Dosya yüklemenin yapılacağı klasörün tam yolu</summary>
            public static string Path;
            /// <summary>
            ///   Dosya yüklemenin yapılacağı klasörün URL'i</summary>
            public static string Url
            {
                get
                {
                    if (HttpContext.Current != null &&
                        HttpContext.Current.Request != null &&
                        HttpContext.Current.Request.IsSecureConnection)
                        return _urlSecure;
                    else
                        return _url;
                }
            }
        }
      
        /// <summary>
        ///   Metni boolean değere çevirir.</summary>
        /// <param name="text">
        ///   Boolean'a çevrilecek metin.</param>
        /// <returns>
        ///   String, "1" ya da "true" değerini içeriyorsa true.</returns>
        public static bool ToBool(string text)
        {
            return (text != null &&
                (text == "1" || text.Equals("true", StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        ///   Belirtilen yol boş değilse ve relatifse bu yolu web sitesinin kök klasörüne göre uygun tam 
        ///   konumuna çevirir.</summary>
        /// <param name="path">
        ///   Tam konumu bulunacak relatif yol.</param>
        /// <returns>
        ///   Tam konum.</returns>
        private static string Relative(string path)
        {
            if (!String.IsNullOrEmpty(path) &&
                HttpContext.Current != null)
            {
                return System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/"), path);
            }
            return path;
        }
    }
}