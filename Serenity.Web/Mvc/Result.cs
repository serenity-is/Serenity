using Newtonsoft.Json;
using Serenity.Data;
using System;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Serenity.Services
{
    public class Result<TResponse> : ActionResult
    {
        public Encoding ContentEncoding { get; set; }
        public string ContentType { get; set; }
        public JsonSerializerSettings SerializerSettings { get; set; }
        public TResponse Data { get; set; }
        public Formatting Formatting { get; set; }

        public Result(TResponse data)
        {
            this.Data = data;
            this.SerializerSettings = JsonSettings.Strict;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";

            if (ContentEncoding != null)
                response.ContentEncoding = this.ContentEncoding;

            if (Data != null)
            {
                JsonTextWriter writer = new JsonTextWriter(response.Output) { Formatting = this.Formatting };
                JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
                serializer.Serialize(writer, Data);
                writer.Flush();
            }
        }
    }
}