using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace Serenity.Services
{
    public class ResultWithStatus<TResponse> : StatusCodeResult
    {
        public Encoding ContentEncoding { get; set; }
        public string ContentType { get; set; }
        public JsonSerializerSettings SerializerSettings { get; set; }
        public TResponse Data { get; set; }
        public Formatting Formatting { get; set; }

        public ResultWithStatus(int statusCode, TResponse data)
            : base(statusCode)
        {
            Data = data;
            SerializerSettings = JsonSettings.Strict;
        }

        public override void ExecuteResult(ActionContext context)
        {
            base.ExecuteResult(context);
            
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var response = context.HttpContext.Response;
            response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";

            if (ContentEncoding != null)
                response.Headers["Content-Encoding"] = ContentEncoding.WebName;
     
            if (Data != null)
            {
                JsonTextWriter writer = new(new StreamWriter(response.Body)) { Formatting = Formatting };
                JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
                serializer.Serialize(writer, Data);
                writer.Flush();
            }
        }
    }
}