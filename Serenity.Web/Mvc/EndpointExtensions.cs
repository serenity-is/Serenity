#if !COREFX
using Newtonsoft.Json;
using Serenity.Configuration;
using Serenity.Data;
using System;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Serenity.Services
{
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
                if (HttpContext.Current != null && !HttpContext.Current.IsCustomErrorEnabled)
                    error.Details = ve.ToString();
            }
            else
            {
                error.Code = "Exception";

                if (HttpContext.Current != null && !HttpContext.Current.IsCustomErrorEnabled)
                {
                    error.Message = exception.Message;
                    error.Details = exception.ToString();
                }
                else
                    error.Message = "An error occured while processing your request.";
            }

            response.Error = error;
            return response;
        }

        public static Result<TResponse> ExecuteMethod<TResponse>(this Controller controller, Func<TResponse> handler)
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
                controller.HttpContext.Response.Clear();
                controller.HttpContext.Response.StatusCode = exception is ValidationError ? 400 : 500;
                controller.HttpContext.Response.TrySkipIisCustomErrors = true;
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
                controller.HttpContext.Response.Clear();
                controller.HttpContext.Response.StatusCode = exception is ValidationError ? 400 : 500;
                controller.HttpContext.Response.TrySkipIisCustomErrors = true;
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
                controller.HttpContext.Response.Clear();
                controller.HttpContext.Response.StatusCode = exception is ValidationError ? 400 : 500;
                controller.HttpContext.Response.TrySkipIisCustomErrors = true;

            }

            return new Result<TResponse>(response);
        }
    }
}
#endif