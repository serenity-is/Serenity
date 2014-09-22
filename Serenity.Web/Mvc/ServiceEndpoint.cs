using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Reflection;
using Serenity.Data;
using System.Data;

namespace Serenity.Services
{
    public abstract class ServiceEndpoint : Controller
    {
        protected override IActionInvoker CreateActionInvoker()
        {
            return new ServiceEndpointActionInvoker();
        }

        private class ServiceEndpointActionInvoker : ControllerActionInvoker
        {
            public const string JsonContentType = "application/json";

            protected override object GetParameterValue(ControllerContext controllerContext, ParameterDescriptor parameterDescriptor)
            {
                if (parameterDescriptor.ParameterType == typeof(IUnitOfWork) ||
                    parameterDescriptor.ParameterType == typeof(IDbConnection))
                {
                    return null;
                }

                return base.GetParameterValue(controllerContext, parameterDescriptor);
            }

            protected override FilterInfo GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
            {
                var info = base.GetFilters(controllerContext, actionDescriptor);
                
                if (!info.ActionFilters.Any(x => x is JsonFilter))
                    info.ActionFilters.Add(new JsonFilter());
                
                return info;
            }

            protected override ActionResult InvokeActionMethod(ControllerContext controllerContext, ActionDescriptor actionDescriptor, IDictionary<string, object> parameters)
            {
                var uowParam = actionDescriptor.GetParameters().FirstOrDefault(x => x.ParameterType == typeof(IUnitOfWork));
                if (uowParam != null)
                {
                    var connectionKey = controllerContext.Controller.GetType().GetCustomAttribute<ConnectionKeyAttribute>();
                    if (connectionKey == null)
                        throw new ArgumentNullException("connectionKey");

                    using (var connection = SqlConnections.NewByKey(connectionKey.Value))
                    using (var uow = new UnitOfWork(connection))
                    {
                        parameters[uowParam.ParameterName] = uow;
                        var result = base.InvokeActionMethod(controllerContext, actionDescriptor, parameters);
                        uow.Commit();
                    }
                }
                
                var cnnParam = actionDescriptor.GetParameters().FirstOrDefault(x => x.ParameterType == typeof(IDbConnection));
                if (cnnParam != null)
                {
                    var connectionKey = controllerContext.Controller.GetType().GetCustomAttribute<ConnectionKeyAttribute>();
                    if (connectionKey == null)
                        throw new ArgumentNullException("connectionKey");

                    using (var cnn = SqlConnections.NewByKey(connectionKey.Value))
                    {
                        parameters[cnnParam.ParameterName] = cnn;
                        return base.InvokeActionMethod(controllerContext, actionDescriptor, parameters);
                    }
                }

                return base.InvokeActionMethod(controllerContext, actionDescriptor, parameters);
            }

            protected override ActionResult CreateActionResult(ControllerContext controllerContext, ActionDescriptor actionDescriptor, object actionReturnValue)
            {
                return (actionReturnValue as ActionResult) ?? new Result<object>(actionReturnValue);
            }

            protected override ExceptionContext InvokeExceptionFilters(ControllerContext controllerContext, IList<IExceptionFilter> filters, Exception exception)
            {
                var result = base.InvokeExceptionFilters(controllerContext, filters.Where(x => !(x is HandleErrorAttribute)).ToList(), exception);
                if (result.IsChildAction ||
                    result.ExceptionHandled ||
                    new HttpException(null, result.Exception).GetHttpCode() != 500)
                {
                    return result;
                }

                result.Result = new Result<ServiceResponse>(exception.ConvertToResponse<ServiceResponse>());
                result.ExceptionHandled = true;
                result.HttpContext.Response.Clear();
                result.HttpContext.Response.StatusCode = exception is ValidationError ? 400 : 500;
                result.HttpContext.Response.TrySkipIisCustomErrors = true;
                
                return result;
            }
        }
    }
}