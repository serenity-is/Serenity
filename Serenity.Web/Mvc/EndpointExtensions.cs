using Serenity.Data;
using System;
using System.Data;
#if ASPNETCORE
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
#else
using System.Web;
using System.Web.Mvc;
#endif

namespace Serenity.Services
{
    public static class EndpointExtensions
    {
        public static TResponse ConvertToResponse<TResponse>(this Exception exception)
            where TResponse: ServiceResponse, new()
        {
            exception.Log();

#if ASPNETCORE
            var context = Dependency.Resolve<IHttpContextAccessor>().HttpContext;
#if ASPNETCORE22
            bool showDetails = context != null && context.RequestServices.GetService<IHostingEnvironment>().IsDevelopment();
#else
            bool showDetails = context != null && context.RequestServices
                .GetService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>().EnvironmentName?.ToLowerInvariant() == "development";
#endif
#else
            bool showDetails = HttpContext.Current != null && !HttpContext.Current.IsCustomErrorEnabled;
#endif

            var response = new TResponse();
            var error = new ServiceError();
            if (exception is ValidationError ve)
            {
                error.Code = ve.ErrorCode;
                error.Arguments = ve.Arguments;
                error.Message = ve.Message;
                if (showDetails)
                    error.Details = ve.ToString();
            }
            else
            {
                error.Code = "Exception";

                if (showDetails)
                {
                    error.Message = exception.Message;
                    error.Details = exception.ToString();
                }
                else
                    error.Message = "An error occurred while processing your request.";
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
#if !ASPNETCORE
                controller.HttpContext.Response.TrySkipIisCustomErrors = true;
#endif
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
#if !ASPNETCORE
                controller.HttpContext.Response.TrySkipIisCustomErrors = true;
#endif
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
#if !ASPNETCORE
                controller.HttpContext.Response.TrySkipIisCustomErrors = true;
#endif

            }

            return new Result<TResponse>(response);
        }
    }
}