using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace Serenity.Services;

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