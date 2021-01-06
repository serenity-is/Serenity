using Microsoft.AspNetCore.Mvc.Filters;

namespace Serenity.Services
{
    public class HandleServiceExceptionAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            context.ExceptionHandled = true;
            context.Result = new ResultWithStatus<ServiceResponse>(context.Exception is ValidationError ? 400 : 500,
                context.Exception.ConvertToResponse<ServiceResponse>(context.HttpContext));
        }
    }
}
