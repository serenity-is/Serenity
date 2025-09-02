using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Serenity.Services;

/// <summary>
/// Action model convention for <see cref="ServiceEndpoint"/> actions
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
            
            var isServiceRequest = typeof(ServiceRequest).IsAssignableFrom(paramType);
            if (isServiceRequest && 
                !action.Filters.OfType<JsonRequestAttribute>().Any())
            {
                action.Filters.Add(new JsonRequestAttribute());
            }

            var bindingSource = parameter.BindingInfo?.BindingSource;
            if (bindingSource == null)
            {
                if (isServiceRequest)
                {
                    parameter.BindingInfo ??= new BindingInfo() 
                    { 
                        BindingSource = BindingSource.Body,
                        BinderType = typeof(ServiceEndpointNullModelBinder)
                    };
                }
                else if (typeof(IDbConnection).IsAssignableFrom(paramType) ||
                    typeof(IUnitOfWork).IsAssignableFrom(paramType))
                {
                    parameter.BindingInfo ??= new BindingInfo() 
                    { 
                        BindingSource = BindingSource.Special,
                        BinderType = typeof(ServiceEndpointNullModelBinder)
                    };
                }
            }
        }
    }
}