using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Serenity.Testing;

public static partial class TestServiceCollectionExtensions
{
    public class CustomWebHostEnvironment : IWebHostEnvironment
    {
        public string WebRootPath { get; set; }
        public IFileProvider WebRootFileProvider { get; set; }
        public string ApplicationName { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; }
        public string ContentRootPath { get; set; }
        public string EnvironmentName { get; set; }
    }

    public static IServiceCollection AddWebHostEnvironment(this IServiceCollection collection, string enviroment)
    {
        collection.TryAddSingleton<IWebHostEnvironment>(new CustomWebHostEnvironment()
        {
            EnvironmentName = enviroment
        });

        return collection;
    }
}
