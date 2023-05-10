using Microsoft.AspNetCore.Mvc.Filters;

namespace Serenity.Services;

/// <summary>
/// An exception filter attribute to handle service exceptions and return them to ServiceResponse objects
/// </summary>
public class HandleServiceExceptionAttribute : ExceptionFilterAttribute
{
    /// <inheritdoc/>
    public override void OnException(ExceptionContext context)
    {
        context.ExceptionHandled = true;
        context.Result = new ResultWithStatus<ServiceResponse>(context.Exception is ValidationError ? 400 : 500,
            context.Exception.ConvertToResponse<ServiceResponse>(context.HttpContext));
    }
}
