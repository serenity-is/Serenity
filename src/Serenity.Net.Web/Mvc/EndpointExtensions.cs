using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Serenity.Services;

/// <summary>
/// Contains some extensions for service endpoints
/// </summary>
public static class EndpointExtensions
{
    /// <summary>
    /// Converts the exception object to a service response
    /// </summary>
    /// <typeparam name="TResponse">Response object</typeparam>
    /// <param name="exception">Exception</param>
    /// <param name="httpContext">HTTP context</param>
    public static TResponse ConvertToResponse<TResponse>(this Exception exception, HttpContext httpContext)
        where TResponse : ServiceResponse, new()
    {
        return ConvertToResponse<TResponse>(exception,
            httpContext?.RequestServices?.GetService<ILogger<ServiceEndpoint>>(),
            httpContext?.RequestServices?.GetService<ITextLocalizer>(),
            string.Equals(httpContext?.RequestServices.GetService<IWebHostEnvironment>()?
                .EnvironmentName, "development", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Convert the exception to a service response
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="exception">Exception</param>
    /// <param name="logger">Exception logger</param>
    /// <param name="localizer">Text localizer</param>
    /// <param name="showDetails">True to show details</param>
    public static TResponse ConvertToResponse<TResponse>(this Exception exception, ILogger logger, 
        ITextLocalizer localizer, bool showDetails)
        where TResponse: ServiceResponse, new()
    {
        logger?.LogError(exception, "Error occured during service request!");

        var response = new TResponse();
        
        var error = new ServiceError
        {
            Message = (showDetails || 
                (exception is IIsSensitiveMessage isSensitive && 
                 !isSensitive.IsSensitiveMessage)) ?
                    exception.Message : localizer?.TryGet("Services.GenericErrorMessage") ??
                        "An error occurred while processing your request."
        };

        if (exception is ValidationError ve)
        {
            error.Code = ve.ErrorCode;
            error.Arguments = ve.Arguments;
            if (showDetails)
                error.Details = ve.ToString();
        }
        else
        {
            error.Code = "Exception";

            if (showDetails)
                error.Details = exception?.ToString();
        }

        if (exception != null &&
            exception.Data != null &&
            exception.Data.Contains(nameof(error.ErrorId)))
            error.ErrorId = exception.Data[nameof(error.ErrorId)]?.ToString();

        response.Error = error;
        return response;
    }

    /// <summary>
    /// Executes an action method and converts any exception to a service response
    /// </summary>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <param name="controller">Controller</param>
    /// <param name="handler">Handler callback</param>
    /// <returns></returns>
    public static Result<TResponse> ExecuteMethod<TResponse>(this ControllerBase controller, Func<TResponse> handler)
        where TResponse: ServiceResponse, new()
    {
        TResponse response;
        try
        {
            response = handler();
        }
        catch (Exception exception)
        {
            response = exception.ConvertToResponse<TResponse>(controller.HttpContext);
            controller.HttpContext.Response.Clear();
            controller.HttpContext.Response.StatusCode = exception is ValidationError ? 400 : 500;
        }

        return new Result<TResponse>(response);
    }

    /// <summary>
    /// Executes an action method and converts any exception to a service response
    /// </summary>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <param name="controller">Controller</param>
    /// <param name="handler">Handler callback</param>
    /// <returns></returns>
    public static async Task<Result<TResponse>> ExecuteMethodAsync<TResponse>(this ControllerBase controller, Func<Task<TResponse>> handler)
        where TResponse : ServiceResponse, new()
    {
        TResponse response;
        try
        {
            response = await handler();
        }
        catch (Exception exception)
        {
            response = exception.ConvertToResponse<TResponse>(controller.HttpContext);
            controller.HttpContext.Response.Clear();
            controller.HttpContext.Response.StatusCode = exception is ValidationError ? 400 : 500;
        }

        return new Result<TResponse>(response);
    }

    /// <summary>
    /// Executes a callback by passing a connection object and converts
    /// any exception raised inside to a service response.
    /// </summary>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <param name="controller">Controller</param>
    /// <param name="connectionKey">Connection key</param>
    /// <param name="handler">Handler callback</param>
    public static Result<TResponse> UseConnection<TResponse>(this ControllerBase controller, string connectionKey, Func<IDbConnection, TResponse> handler)
        where TResponse : ServiceResponse, new()
    {
        TResponse response;
        try
        {
            var factory = controller.HttpContext.RequestServices.GetRequiredService<ISqlConnections>();
            using var connection = factory.NewByKey(connectionKey);
            response = handler(connection);
        }
        catch (Exception exception)
        {
            response = exception.ConvertToResponse<TResponse>(controller.HttpContext);
            controller.HttpContext.Response.Clear();
            controller.HttpContext.Response.StatusCode = exception is ValidationError ? 400 : 500;
        }

        return new Result<TResponse>(response);
    }


    /// <summary>
    /// Executes a callback by passing a unit of work object and converts
    /// any exception raised inside to a service response.
    /// </summary>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <param name="controller">Controller</param>
    /// <param name="connectionKey">Connection key</param>
    /// <param name="handler">Handler callback</param>
    public static Result<TResponse> InTransaction<TResponse>(this ControllerBase controller, string connectionKey, Func<IUnitOfWork, TResponse> handler)
        where TResponse : ServiceResponse, new()
    {
        TResponse response;
        try
        {
            var factory = controller.HttpContext.RequestServices.GetRequiredService<ISqlConnections>();

            using var connection = factory.NewByKey(connectionKey);
            using var uow = new UnitOfWork(connection);
            response = handler(uow);
            uow.Commit();
        }
        catch (Exception exception)
        {
            response = exception.ConvertToResponse<TResponse>(controller.HttpContext);
            controller.HttpContext.Response.Clear();
            controller.HttpContext.Response.StatusCode = exception is ValidationError ? 400 : 500;
        }

        return new Result<TResponse>(response);
    }
}