#if !ASPNETMVC
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Serenity.Data;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Serenity.Services
{
    public class ServiceEndpointModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var type = context.Metadata.ModelType;
            if (typeof(IDbConnection).IsAssignableFrom(type) ||
                typeof(IUnitOfWork).IsAssignableFrom(type))
            {
                return ServiceEndpointNullModelBinder.Instance;
            }

            return null;
        }
    }

    public class ServiceEndpointNullModelBinder : IModelBinder
    {
        public static readonly ServiceEndpointNullModelBinder Instance = new();

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            bindingContext.Result = ModelBindingResult.Success(null);
            return Task.CompletedTask;
        }
    }

    public class ServiceEndpointActionModelConvention : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            if (!action.Controller.ControllerType.IsSubclassOf(typeof(ServiceEndpoint)))
                return;

            foreach (var parameter in action.Parameters)
            {
                var paramType = parameter.ParameterInfo.ParameterType;
                if (typeof(ServiceRequest).IsAssignableFrom(typeof(ServiceRequest)))
                {
                    if (!action.Filters.Any(x => x is JsonRequestAttribute))
                        action.Filters.Add(new JsonRequestAttribute());

                    break;
                }
            }
        }
    }
}
#endif