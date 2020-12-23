using Newtonsoft.Json;
using Serenity.Services;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Serenity.Web
{
    public static class RecaptchaValidation
    {
        /// <summary>
        /// Validates a recaptcha token
        /// </summary>
        /// <param name="token"></param>
        /// <remarks>Inspired from https://github.com/tanveery/recaptcha-net/blob/master/src/Recaptcha.Web/RecaptchaVerificationHelper.cs</remarks>
        public static void Validate(string secretKey, string token, ITextLocalizer localizer)
        {
            if (string.IsNullOrEmpty(token))
                throw new ValidationError("Recaptcha", localizer.Get("Validation.Recaptcha"));

            var postData = string.Format(CultureInfo.InvariantCulture, "secret={0}&response={1}",
                Uri.EscapeDataString(secretKey),
                Uri.EscapeDataString(token));

            byte[] postDataBuffer = System.Text.Encoding.ASCII.GetBytes(postData);
            var verifyUri = new Uri("https://www.google.com/recaptcha/api/siteverify", UriKind.Absolute);

            var webRequest = (HttpWebRequest)WebRequest.Create(verifyUri);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Headers["Content-Length"] = postDataBuffer.Length.ToString(CultureInfo.InvariantCulture);

            webRequest.Method = "POST";
            using (var requestStream = Task.Run(() => webRequest.GetRequestStreamAsync()).Result)
                requestStream.Write(postDataBuffer, 0, postDataBuffer.Length);

            using (var webResponse = Task.Run(() => webRequest.GetResponseAsync()).Result)
            { 
                string responseJson;
                using (var sr = new StreamReader(webResponse.GetResponseStream()))
                    responseJson = sr.ReadToEnd();

                var response = JSON.ParseTolerant<RecaptchaResponse>(responseJson);
                if (response == null ||
                    !response.Success)
                {
                    throw new ValidationError("Recaptcha", localizer.Get("Validation.Recaptcha"));
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