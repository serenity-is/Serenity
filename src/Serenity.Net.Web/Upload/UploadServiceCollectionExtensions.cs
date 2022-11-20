using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Serenity.Web
{
    public static class UploadServiceCollectionExtensions
    {
        public static void AddUploadStorage(this IServiceCollection collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            collection.AddOptions();
            collection.TryAddSingleton<IUploadStorage, DefaultUploadStorage>();
            collection.TryAddSingleton<IUploadValidator, DefaultUploadValidator>();
            collection.TryAddSingleton<IImageProcessor, DefaultImageProcessor>();
            collection.TryAddSingleton<IUploadProcessor, DefaultUploadProcessor>();
        }

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
}
