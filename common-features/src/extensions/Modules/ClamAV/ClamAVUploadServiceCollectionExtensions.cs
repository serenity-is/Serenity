using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Serenity.Extensions.DependencyInjection;

/// <summary>
/// DI extension methods related to ClamAVUploadScanner
/// </summary>
public static class ClamAVUploadServiceCollectionExtensions
{
    /// <summary>
    /// Adds ClamAVUploadScanner as IUploadAVScanner
    /// </summary>
    /// <param name="collection">Service collection</param>
    public static IServiceCollection AddClamAVUploadScanner(this IServiceCollection collection)
    {
        collection.AddOptions();
        collection.TryAddSingleton<IUploadAVScanner, ClamAVUploadScanner>();
        return collection;
    }
}
