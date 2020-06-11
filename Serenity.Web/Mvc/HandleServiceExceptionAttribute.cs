#if !ASPNETMVC
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serenity.Data;
using System;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Serenity.Services
{
    public class HandleServiceExceptionAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            context.ExceptionHandled = true;
            context.Result = new ResultWithStatus<ServiceResponse>(context.Exception is ValidationError ? 400 : 500,
                context.Exception.ConvertToResponse<ServiceResponse>());
        }
    }
}
#endif
