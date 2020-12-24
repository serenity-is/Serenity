using Newtonsoft.Json;
using System;
using System.Text;
#if !ASPNETMVC
using System.IO;
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

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
            Data = data;
            SerializerSettings = JsonSettings.Strict;
        }

#if !ASPNETMVC
        public override void ExecuteResult(ActionContext context)
#else
        public override void ExecuteResult(ControllerContext context)
#endif
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var response = context.HttpContext.Response;
            response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";

            if (ContentEncoding != null)
#if !ASPNETMVC
                response.Headers["Content-Encoding"] = ContentEncoding.WebName;
#else
                response.ContentEncoding = this.ContentEncoding;
#endif
            if (Data != null)
            {
#if !ASPNETMVC
                JsonTextWriter writer = new JsonTextWriter(new StreamWriter(response.Body)) { Formatting = Formatting };
#else
                JsonTextWriter writer = new JsonTextWriter(response.Output) { Formatting = this.Formatting };
#endif
                JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
                serializer.Serialize(writer, Data);
                writer.Flush();
            }
        }
    }
}