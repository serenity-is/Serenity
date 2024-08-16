using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Serenity.Services;

/// <summary>
/// An <see cref="IBindingMetadataProvider"/> which configures <c>ModelMetadata.IsBindingAllowed</c> to
/// <c>false</c> for <see cref="IDbConnection" /> and <see cref="IUnitOfWork" /> types.
/// </summary>
public class ServiceEndpointBindingMetadataProvider : IBindingMetadataProvider
{
    /// <inheritdoc />
    public void CreateBindingMetadata(BindingMetadataProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        // No-op if the metadata is not for the target type
        if (!typeof(IDbConnection).IsAssignableFrom(context.Key.ModelType) &&
            !typeof(IUnitOfWork).IsAssignableFrom(context.Key.ModelType))
        {
            return;
        }

        context.BindingMetadata.IsBindingAllowed = false;
    }
}