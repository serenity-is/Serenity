using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Threading.Tasks;

namespace Serenity.Services
{
    /// <summary>
    /// Provides model binding for service endpoints
    /// </summary>
    public class ServiceEndpointModelBinderProvider : IModelBinderProvider
    {
        /// <inheritdoc/>
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

    /// <summary>
    /// Null model binder for interface arguments
    /// </summary>
    public class ServiceEndpointNullModelBinder : IModelBinder
    {
        /// <summary>
        /// Default instance
        /// </summary>
        public static readonly ServiceEndpointNullModelBinder Instance = new();

        /// <inheritdoc/>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            bindingContext.Result = ModelBindingResult.Success(null);
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Action model convention for service endpoints
    /// </summary>
    public class ServiceEndpointActionModelConvention : IActionModelConvention
    {
        /// <inheritdoc/>
        public void Apply(ActionModel action)
        {
            if (!action.Controller.ControllerType.IsSubclassOf(typeof(ServiceEndpoint)))
                return;

            foreach (var parameter in action.Parameters)
            {
                var paramType = parameter.ParameterInfo.ParameterType;
                if (typeof(ServiceRequest).IsAssignableFrom(paramType))
                {
                    if (!action.Filters.Any(x => x is JsonRequestAttribute))
                        action.Filters.Add(new JsonRequestAttribute());

                    break;
                }
            }
        }
    }
}