
namespace Serenity.Web
{
    using ComponentModel;
    using Newtonsoft.Json;
    using Serenity.Services;
    using System;
    using System.IO;
    using System.Net;
#if !NET45
    using System.Threading.Tasks;
#endif
    public static class RecaptchaValidation
    {
        /// <summary>
        /// Validates a recaptcha token
        /// </summary>
        /// <param name="token"></param>
        /// <remarks>Inspired from https://github.com/tanveery/recaptcha-net/blob/master/src/Recaptcha.Web/RecaptchaVerificationHelper.cs</remarks>
        public static void Validate(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ValidationError("Recaptcha", LocalText.Get("Validation.Recaptcha"));

            var config = Config.Get<RecaptchaSettings>();

            var postData = String.Format("secret={0}&response={1}",
                Uri.EscapeDataString(config.SecretKey),
                Uri.EscapeDataString(token));

            byte[] postDataBuffer = System.Text.Encoding.ASCII.GetBytes(postData);
            var verifyUri = new Uri("https://www.google.com/recaptcha/api/siteverify", UriKind.Absolute);

            var webRequest = (HttpWebRequest)WebRequest.Create(verifyUri);
            webRequest.ContentType = "application/x-www-form-urlencoded";
#if !NET45
            webRequest.Headers["Content-Length"] = postDataBuffer.Length.ToString();
#else
            var proxy = WebRequest.GetSystemWebProxy();
            proxy.Credentials = CredentialCache.DefaultCredentials;
            webRequest.Proxy = proxy;
#endif

            webRequest.Method = "POST";
#if !NET45
            using (var requestStream = Task.Run(() => webRequest.GetRequestStreamAsync()).Result)
                requestStream.Write(postDataBuffer, 0, postDataBuffer.Length);

            using (var webResponse = Task.Run(() => webRequest.GetResponseAsync()).Result)
            { 
#else
            using (var requestStream = webRequest.GetRequestStream())
                requestStream.Write(postDataBuffer, 0, postDataBuffer.Length);

            using (var webResponse = (HttpWebResponse)webRequest.GetResponse())
            {
#endif
                string responseJson;
                using (var sr = new StreamReader(webResponse.GetResponseStream()))
                    responseJson = sr.ReadToEnd();

                var response = JSON.ParseTolerant<RecaptchaResponse>(responseJson);
                if (response == null ||
                    !response.Success)
                {
                    throw new ValidationError("Recaptcha", LocalText.Get("Validation.Recaptcha"));
                }
            }
        }

        private class RecaptchaResponse
        {
            public bool Success { get; set; }
            [JsonProperty("error-codes")]
            public string[] ErrorCodes { get; set; }
        }
    }
}