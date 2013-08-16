using Newtonsoft.Json;
using Serenity.Data;
using Serenity.Web;
using System;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Serenity.Services
{
    public class Result<TResponse> : ActionResult
        where TResponse: ServiceResponse
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

    public static class EndpointExtensions
    {
        public static TResponse ConvertToResponse<TResponse>(this Exception exception)
            where TResponse: ServiceResponse, new()
        {
            exception.Log();

            var response = new TResponse();
            var error = new ServiceError();
            var ve = exception as ValidationError;
            if (ve != null)
            {
                error.Code = ve.ErrorCode;
                error.Arguments = ve.Arguments;
                error.Message = ve.Message;
                if (!WebApplicationSettings.HideExceptionDetails)
                    error.Details = ve.ToString();
            }
            else
            {
                error.Code = "Exception";

                if (!WebApplicationSettings.HideExceptionDetails)
                {
                    error.Message = exception.Message;
                    error.Details = exception.ToString();
                }
                else
                    error.Message = "İsteğin işlenmesi esnasında bir hata oluştu!";
            }

            response.Error = error;
            return response;
        }

        public static Result<TResponse> Execute<TResponse>(this Controller controller, Func<TResponse> handler)
            where TResponse: ServiceResponse, new()
        {
            TResponse response;
            try
            {
                response = handler();
            }
            catch (Exception exception)
            {
                response = exception.ConvertToResponse<TResponse>();
            }

            return new Result<TResponse>(response);
        }

        public static Result<TResponse> UseConnection<TResponse>(this Controller controller, string connectionKey, Func<IDbConnection, TResponse> handler)
            where TResponse : ServiceResponse, new()
        {
            TResponse response;
            try
            {
                using (var connection = SqlConnections.NewByKey(connectionKey))
                    response = handler(connection);
            }
            catch (Exception exception)
            {
                response = exception.ConvertToResponse<TResponse>();
            }

            return new Result<TResponse>(response);
        }


        public static Result<TResponse> InTransaction<TResponse>(this Controller controller, string connectionKey, Func<IUnitOfWork, TResponse> handler)
            where TResponse : ServiceResponse, new()
        {
            TResponse response;
            try
            {
                using (var connection = SqlConnections.NewByKey(connectionKey))
                using (var uow = new UnitOfWork(connection))
                {
                    response = handler(uow);
                    uow.Commit();
                }
            }
            catch (Exception exception)
            {
                response = exception.ConvertToResponse<TResponse>();
            }

            return new Result<TResponse>(response);
        }
    }
}