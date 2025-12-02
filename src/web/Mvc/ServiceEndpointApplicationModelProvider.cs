using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Serenity.Services;

/// <summary>
/// Applies action model conventions for <see cref="ServiceEndpoint"/> controllers
/// </summary>
public class ServiceEndpointApplicationModelProvider : IApplicationModelProvider
{
    private readonly List<IActionModelConvention> actionModelConventions =
    [
        new ServiceEndpointActionModelConvention()
    ];

    /// <summary>
    /// [ApiController] attribute model provider uses <c>-1000 + 100</c> -
    /// this needs to be applied first so that we get attribute routing.
    /// <see cref="AuthorizationApplicationModelProvider"/> uses +10 - so we're trying
    /// to split the difference here.
    /// </summary>
    public int Order => -1000 + 50;

    /// <inheritdoc/>
    public void OnProvidersExecuted(ApplicationModelProviderContext context)
    {
    }

    /// <inheritdoc/>
    public void OnProvidersExecuting(ApplicationModelProviderContext context)
    {
        foreach (var controller in context.Result.Controllers)
        {
            if (controller.ControllerType.IsAbstract ||
                !controller.ControllerType.IsSubclassOf(typeof(ServiceEndpoint)))
            {
                continue;
            }

            foreach (var action in controller.Actions)
            {
                foreach (var convention in actionModelConventions)
                {
                    convention.Apply(action);
                }
            }
        }
    }
}