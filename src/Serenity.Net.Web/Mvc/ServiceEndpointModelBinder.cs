using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Serenity.Services;

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