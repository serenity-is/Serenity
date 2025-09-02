using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Serenity.Services;

/// <summary>
/// An exception filter attribute to handle controller exceptions and return as view.
/// </summary>
public class HandleControllerExceptionAttribute : ExceptionFilterAttribute
{
    /// <inheritdoc/>
    public override void OnException(ExceptionContext context)
    {
        context.ExceptionHandled = true;
        var result = context.Exception.ConvertToResponse<ServiceResponse>(context.HttpContext);
        context.Result = new ViewResult()
        {
            ViewName = "~/Views/Errors/ValidationError.cshtml",
            ViewData = new ViewDataDictionary<ValidationError>(new EmptyModelMetadataProvider(), context.ModelState)
            {
                Model = new ValidationError(result.Error.Code, result.Error.Message),
            }
        };
    }
}
