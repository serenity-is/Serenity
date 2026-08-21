using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serenity.Web;

namespace Serenity.Extensions.DependencyInjection;

/// <summary>
/// DI extension methods related to upload services.
/// </summary>
public static class UploadServiceCollectionExtensions
{
    /// <summary>
    /// Registers the default implementations of <see cref="IUploadStorage"/>,
    /// <see cref="IUploadValidator"/>, <see cref="IImageProcessor"/> and
    /// <see cref="IUploadProcessor"/> interfaces.
    /// </summary>
    /// <param name="collection">The service collection.</param>
    /// <returns>The same service collection so that calls can be chained.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c>.</exception>
    public static IServiceCollection AddUploadStorage(this IServiceCollection collection)
    {
        collection.AddOptions();
        collection.TryAddSingleton<IFilenameFormatSanitizer, DefaultFilenameFormatSanitizer>();
        collection.TryAddSingleton<IUploadStorage, DefaultUploadStorage>();
        collection.TryAddSingleton<IUploadValidator, DefaultUploadValidator>();
        collection.TryAddSingleton<IImageProcessor, DefaultImageProcessor>();
        collection.TryAddSingleton<IUploadProcessor, DefaultUploadProcessor>();
        collection.TryAddSingleton<IUploadFileResponder, DefaultUploadFileResponder>();
        return collection;
    }

    /// <summary>
    /// Registers the default implementations of <see cref="IUploadStorage"/>,
    /// <see cref="IUploadValidator"/>, <see cref="IImageProcessor"/> and
    /// <see cref="IUploadProcessor"/> interfaces.
    /// </summary>
    /// <param name="collection">The service collection.</param>
    /// <param name="setupAction">The callback to edit options.</param>
    /// <returns>The same service collection so that calls can be chained.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="collection"/> or <paramref name="setupAction"/> is <c>null</c>.</exception>
    public static IServiceCollection AddUploadStorage(this IServiceCollection collection,
        Action<UploadSettings> setupAction)
    {
        ArgumentNullException.ThrowIfNull(collection);

        ArgumentNullException.ThrowIfNull(setupAction);

        collection.AddUploadStorage();
        return collection.Configure(setupAction);
    }

}
