using Newtonsoft.Json;
using Surat.Site.Services;
using System.IO;
using System.Net;

namespace Serenity.Services
{
    public class JsonServiceClient
    {
        protected CookieContainer cookies = new CookieContainer();

        public JsonServiceClient(string baseUrl)
        {
            this.BaseUrl = baseUrl;
            this.cookies = new CookieContainer();
        }

        protected string BaseUrl { get; set; }

        public virtual TResponse Post<TResponse>(string relativeUrl, object request)
            where TResponse : new()
        {
            return InternalPost<TResponse>(relativeUrl, request);
        }

        protected TResponse InternalPost<TResponse>(string relativeUrl, object request)
            where TResponse : new()
        {
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(UriHelper.Combine(BaseUrl, relativeUrl));
            wr.Method = "POST";
            var r = JsonConvert.SerializeObject(request, Formatting.None, JsonSettings.Strict);
            wr.ContentType = "application/json";
            var rb = System.Text.Encoding.UTF8.GetBytes(r);
            //wr.ContentLength = rb.Length; bunu yapma hata alınca çakılıyor! redirect lerle ilgili bug malesef!
            wr.CookieContainer = cookies;
            wr.Timeout = 10 * 60 * 1000;
            using (Stream requestStream = wr.GetRequestStream())
                requestStream.Write(rb, 0, rb.Length);

            wr.KeepAlive = true;
            using (var response = wr.GetResponse())
            {
                using (var rs = response.GetResponseStream())
                using (var sr = new StreamReader(rs))
                {
                    var rt = sr.ReadToEnd();
                    var resp = JsonConvert.DeserializeObject<TResponse>(rt, JsonSettings.Tolerant); // fazladan eklenmiş alan varsa önemseme
                    var serviceResponse = resp as ServiceResponse;

                    if (serviceResponse != null &&
                        serviceResponse.Error != null)
                    {
                        throw new ValidationError(
                            serviceResponse.Error.Code,
                            serviceResponse.Error.Arguments,
                            serviceResponse.Error.Message);
                    }

                    return resp;
                }
            }
        }
    }
}