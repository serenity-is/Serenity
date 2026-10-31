using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Text.Json;

namespace Serenity.Web;

/// <summary>
/// Serves static assets of packages that don't use the static web assets SDK (e.g. Serenity.Corelib)
/// while running from the build output. The build writes a manifest named
/// "[ApplicationName].serenitystaticassets.json" next to the application assembly, which lists the
/// asset folders (project wwwroot or package staticwebassets folders) and their base paths.
/// A published application has these files under its wwwroot, and no manifest.
/// </summary>
public static class SerenityStaticAssets
{
    /// <summary>
    /// Suffix of the manifest file name, which is prefixed with the application name.
    /// </summary>
    public const string ManifestFileSuffix = ".serenitystaticassets.json";

    /// <summary>
    /// An asset folder entry in the manifest.
    /// </summary>
    public class ManifestEntry
    {
        /// <summary>
        /// Gets or sets the base path the folder is served under, like "Serenity.Corelib".
        /// </summary>
        public string? BasePath { get; set; }

        /// <summary>
        /// Gets or sets the physical path of the folder.
        /// </summary>
        public string? Root { get; set; }

        /// <summary>
        /// Gets or sets the include patterns (; separated, project relative), the same ones used to select
        /// the files to publish / pack. Empty for package folders, which only contain the selected files.
        /// </summary>
        public string? Includes { get; set; }

        /// <summary>
        /// Gets or sets the exclude patterns (; separated, project relative).
        /// </summary>
        public string? Excludes { get; set; }

        /// <summary>
        /// Gets or sets if the .well-known folder is included regardless of <see cref="Excludes"/>,
        /// in which case <see cref="WellKnownExcludes"/> apply to it instead.
        /// </summary>
        public bool WellKnown { get; set; }

        /// <summary>
        /// Gets or sets the exclude patterns for the .well-known folder.
        /// </summary>
        public string? WellKnownExcludes { get; set; }
    }

    /// <summary>
    /// Creates the file provider for a manifest entry, which serves the files of the folder that match its
    /// patterns under its base path.
    /// </summary>
    /// <param name="entry">Manifest entry</param>
    public static IFileProvider CreateFileProvider(ManifestEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        IFileProvider provider = new PhysicalFileProvider(entry.Root!);
        if (!string.IsNullOrEmpty(entry.Includes) || !string.IsNullOrEmpty(entry.Excludes))
        {
            var filter = new StaticAssetPatternFilter(entry.Includes, entry.Excludes, entry.WellKnown, entry.WellKnownExcludes);
            provider = new FilteredFileProvider(provider, filter.IsMatch);
        }

        return new PrefixedFileProvider(entry.BasePath!, provider);
    }

    /// <summary>
    /// Gets the manifest file path for the application.
    /// </summary>
    /// <param name="environment">Host environment</param>
    public static string GetManifestPath(IWebHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(environment);
        return Path.Combine(AppContext.BaseDirectory, environment.ApplicationName + ManifestFileSuffix);
    }

    /// <summary>
    /// Reads the asset folder entries from a manifest file.
    /// </summary>
    /// <param name="manifestPath">Manifest file path</param>
    public static ManifestEntry[] ReadManifest(string manifestPath)
    {
        return JsonSerializer.Deserialize<ManifestEntry[]>(File.ReadAllText(manifestPath),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];
    }

    /// <summary>
    /// Adds the asset folders in the manifest to the web root file provider of the environment.
    /// Also enables the static web assets of other projects / packages when not running in
    /// development environment, as it is only done automatically for development. Does nothing
    /// if there is no manifest, e.g. for a published application.
    /// </summary>
    /// <param name="environment">Host environment</param>
    /// <param name="configuration">Configuration</param>
    /// <returns>True if a manifest is found and applied</returns>
    public static bool Apply(IWebHostEnvironment environment, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(environment);

        var manifestPath = GetManifestPath(environment);
        if (!File.Exists(manifestPath))
            return false;

        if (!environment.IsDevelopment())
            StaticWebAssetsLoader.UseStaticWebAssets(environment, configuration);

        var providers = new List<IFileProvider>();
        if (environment.WebRootFileProvider != null)
            providers.Add(environment.WebRootFileProvider);

        foreach (var entry in ReadManifest(manifestPath))
        {
            if (string.IsNullOrEmpty(entry.BasePath) ||
                string.IsNullOrEmpty(entry.Root) ||
                !Directory.Exists(entry.Root))
                continue;

            providers.Add(CreateFileProvider(entry));
        }

        environment.WebRootFileProvider = new CompositeFileProvider(providers);
        return true;
    }

    /// <summary>
    /// Serves static assets of the packages listed in the build output manifest, see
    /// <see cref="SerenityStaticAssets"/>. Use this instead of UseStaticWebAssets(), it
    /// also enables static web assets outside the development environment.
    /// </summary>
    /// <param name="builder">Web host builder</param>
    public static IWebHostBuilder UseSerenityStaticAssets(this IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ConfigureAppConfiguration((context, _) =>
            Apply(context.HostingEnvironment, context.Configuration));
        return builder;
    }
}
