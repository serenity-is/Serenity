#if IsFeatureBuild || IsTemplateBuild
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Build;

public static partial class Shared
{
    static readonly string[] LocalFeedNames = ["MyPackages"];
    const string NugetOrgReadSource = "https://api.nuget.org/v3/index.json";
    public const string NugetOrgPushSource = "https://www.nuget.org/api/v2/package";

    const string SerenityIsSourceKey = "serenity.is";
    private static PackageSource serenityIsPackageSource;

    public static PackageSource SerenityIsPackageSource
    {
        get
        {
            if (serenityIsPackageSource is null)
            {
                var settings = Settings.LoadDefaultSettings(null);
                var packageSourceProvider = new PackageSourceProvider(settings);
                var packageSources = packageSourceProvider.LoadPackageSources();
                serenityIsPackageSource = packageSources.FirstOrDefault(x => string.Equals(x.Name, SerenityIsSourceKey) ||
                    x.Source.StartsWith("https://packages.serenity.is", StringComparison.OrdinalIgnoreCase));
            }

            return serenityIsPackageSource;
        }
    }

    private static PackageSource GetLocalNugetFeed(bool create)
    {
        try
        {
            var settings = Settings.LoadDefaultSettings(null);
            var packageSourceProvider = new PackageSourceProvider(settings);
            var packageSources = packageSourceProvider.LoadPackageSources();
            var localFeed = packageSources.FirstOrDefault(x => x.IsLocal &&
                LocalFeedNames.Contains(x.Name, StringComparer.OrdinalIgnoreCase));

            if (localFeed == null && create)
            {
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    ".nuget", "my-packages");
                var packageSource = new PackageSource(path, LocalFeedNames.First());
                packageSourceProvider.AddPackageSource(packageSource);
                localFeed = packageSource;
            }

            return localFeed;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
        }

        return null;
    }

    public static void PushToRemoteSource(string nupkg, string pushSource)
    {
        if (StartProcess("dotnet", $"nuget push \"{nupkg}\"" +
                " --source \"" + pushSource + "\"", Path.GetDirectoryName(nupkg)) != 0)
        {
            Console.Error.WriteLine("Error while pushing " + Path.GetFileName(nupkg));
            Environment.Exit(1);
        }
    }

#if IsFeatureBuild
    private static void PushToLocalNugetFeed(PackageSource localFeed, string nupkg)
    {
        Directory.CreateDirectory(localFeed.Source);
        var filename = Path.ChangeExtension(Path.GetFileName(nupkg).ToLowerInvariant(), null);
        var match = System.Text.RegularExpressions.Regex.Match(filename, @"(.+?)((\.[0-9]+)+)");
        if (match != null && match.Groups.Count >= 2)
        {
            var id = match.Groups[1].Value;
            var version = match.Groups[2].Value.Substring(1);
            var dir = Path.Combine(localFeed.Source, id, version);
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);

            var metadata = Path.Combine(dir, ".nupkg.metadata");

            if (StartProcess("dotnet", $"nuget push \"{nupkg}\"" +
                " --source " + localFeed.Name, Path.GetDirectoryName(nupkg)) != 0)
                Console.Error.WriteLine("Error while pushing " + Path.GetFileName(nupkg) +
                    " to local feed");
            else if (!File.Exists(metadata))
            {
                string nuget = "nuget";
                if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
                    File.Exists(NugetExePath))
                    nuget = NugetExePath;

                if (StartProcess(nuget, "init . .", localFeed.Source) != 0)
                    Console.Error.WriteLine("Error while initializing " + localFeed.Source);
                else if (File.Exists(metadata))
                    File.Delete(Path.Combine(localFeed.Source, Path.GetFileName(nupkg)));
            }
        }
    }
#endif

    public static bool IsProPackage(string packageId)
    {
        return packageId.StartsWith("Serenity.", StringComparison.OrdinalIgnoreCase) &&
            (packageId.StartsWith("Serenity.Pro", StringComparison.OrdinalIgnoreCase) ||
                packageId.Contains("Advanced", StringComparison.OrdinalIgnoreCase) ||
                packageId.Contains("Premium", StringComparison.OrdinalIgnoreCase));
    }

    private static NuGetVersion GetLatestVersionOf(string packageId)
    {
        if (IsProPackage(packageId))
            return GetLatestVersionOf(SerenityIsPackageSource, SerenityIsSourceKey, packageId);

        var version = GetLatestVersionOf(null, NugetOrgReadSource, packageId);
        var localSource = GetLocalNugetFeed(create: false);
        if (localSource != null && Directory.Exists(localSource.Source))
        {
            var localVersion = GetLatestVersionOf(localSource, null, packageId);
            if (localVersion != null && (version == null || localVersion > version))
                return localVersion;
        }
        return version;
    }

    private static SourceCacheContext sourceCacheContext;
    private static readonly Dictionary<string, AutoCompleteResource> findPackageByIdSource = [];

    private static NuGetVersion GetLatestVersionOf(PackageSource packageSource,
        string sourceKey, string packageId)
    {
        ILogger logger = NullLogger.Instance;
        CancellationToken cancellationToken = CancellationToken.None;
        sourceCacheContext ??= new SourceCacheContext().WithRefreshCacheTrue();

        var sourceCacheKey = packageSource?.Source ?? sourceKey;
        if (!findPackageByIdSource.TryGetValue(sourceCacheKey,
            out var resource))
        {
            var repository = packageSource != null ?
                Repository.Factory.GetCoreV3(packageSource) :
                Repository.Factory.GetCoreV3(sourceKey);
            resource = repository.GetResource<AutoCompleteResource>();
            findPackageByIdSource[sourceCacheKey] = resource;
        }

        var versions = resource.VersionStartsWith(packageId, "", false,
            sourceCacheContext,
            logger,
            cancellationToken).GetAwaiter().GetResult();

        return versions.Where(x => !x.IsPrerelease).Max(x => x);
    }
    public static void UpdateSergen(string dir, string targetVersion)
    {
        if (StartProcess("dotnet", "tool update sergen --version " + targetVersion, dir) != 0)
        {
            if (StartProcess("dotnet", "tool uninstall sergen", dir) != 0)
                ExitWithError("Error while uninstalling sergen at " + dir);
            if (StartProcess("dotnet", "tool install sergen --version " + targetVersion, dir) != 0)
                ExitWithError("Error while updating sergen at " + dir);
        }
    }
}
#endif