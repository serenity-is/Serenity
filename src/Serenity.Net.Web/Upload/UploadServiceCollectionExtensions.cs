using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Serenity.Web;

/// <summary>
/// DI extension methods related to upload services
/// </summary>
public static class UploadServiceCollectionExtensions
{
    /// <summary>
    /// Registers the default implementations of <see cref="IUploadStorage"/>,
    /// <see cref="IUploadValidator"/>, <see cref="IImageProcessor"/> and
    /// <see cref="IUploadProcessor"/> interfaces.
    /// </summary>
    /// <param name="collection">Service collection</param>
    /// <exception cref="ArgumentNullException">Collection is null</exception>
    public static void AddUploadStorage(this IServiceCollection collection)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        collection.AddOptions();
        collection.TryAddSingleton<IFilenameFormatSanitizer, DefaultFilenameFormatSanitizer>();
        collection.TryAddSingleton<IUploadStorage, DefaultUploadStorage>();
        collection.TryAddSingleton<IUploadValidator, DefaultUploadValidator>();
        collection.TryAddSingleton<IImageProcessor, DefaultImageProcessor>();
        collection.TryAddSingleton<IUploadProcessor, DefaultUploadProcessor>();
        collection.TryAddSingleton<IUploadFileResponder, DefaultUploadFileResponder>();
    }

    /// <summary>
    /// Registers the default implementations of <see cref="IUploadStorage"/>,
    /// <see cref="IUploadValidator"/>, <see cref="IImageProcessor"/> and
    /// <see cref="IUploadProcessor"/> interfaces.
    /// </summary>
    /// <param name="collection">Service collection</param>
    /// <param name="setupAction">Callback to edit options</param>
    /// <exception cref="ArgumentNullException">Collection is null</exception>
    public static void AddUploadStorage(this IServiceCollection collection,
        Action<UploadSettings> setupAction)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        if (setupAction == null)
            throw new ArgumentNullException(nameof(setupAction));

        collection.AddUploadStorage();
        collection.Configure(setupAction);
    }
}
