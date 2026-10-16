using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Serenity.Services;

/// <summary>
/// Null model binder for interface arguments.
/// </summary>
public class ServiceEndpointNullModelBinder : IModelBinder
{
    /// <summary>
    /// The default instance.
    /// </summary>
    public static readonly ServiceEndpointNullModelBinder Instance = new();

    /// <inheritdoc/>
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        bindingContext.Result = ModelBindingResult.Success(null);
        return Task.CompletedTask;
    }
}