#if IsFeatureBuild
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Build;

public static partial class Shared
{
    public static partial class Targets
    {
        public static void Pack()
        {
            PatchPackageBuildProps();

            UpdateSergen(Src, SerenityVersion);

            PatchDirectoryBuildProps();

            CleanDirectory(PackageOutDir, true);

            if (StartProcess("pnpm", "all", Src) != 0)
                ExitWithError("Error while pnpm all " + Src);

            if (StartProcess("dotnet", "restore", Src) != 0)
                ExitWithError("Error while restoring " + SolutionFile);

            if (StartProcess("dotnet", "pack -v minimal " +
                $"-c Release -p:ContinuousIntegrationBuild=true -o \"{PackageOutDir}\"", Src) != 0)
                ExitWithError("Error while building solution!");

            if (LocalPush)
            try
            {
                var localFeed = GetLocalNugetFeed(create: true);
                if (localFeed != null)
                {
                    foreach (var nupkg in Directory.GetFiles(PackageOutDir, "*.nupkg"))
                        PushToLocalNugetFeed(localFeed, nupkg);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        static void PatchPackageBuildProps()
        {
            if (string.Equals(SerenityVersion, "latest", StringComparison.OrdinalIgnoreCase) ||
                (string.IsNullOrEmpty(SerenityVersion) && !IsPatch))
            {
                SerenityVersion = GetLatestVersionOf(SerenityNetWebPackage)?.ToString();
            }
            else if ((IsPatch && string.IsNullOrEmpty(SerenityVersion)) ||
                string.Equals(SerenityVersion, "ws", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(SerenityVersion, "workspace", StringComparison.OrdinalIgnoreCase))
            {
                var xes = XElement.Parse(File.ReadAllText(SerenityPackageBuildProps));
                SerenityVersion = xes.Descendants("Version").FirstOrDefault()?.Value?.ToString();
            }

            if (string.IsNullOrEmpty(SerenityVersion))
            {
                ExitWithError("Can't determine Serenity version to use!");
                return;
            }

            if (!NuGet.Versioning.NuGetVersion.TryParse(SerenityVersion, out var serVersion))
            {
                ExitWithError("Can't parse Serenity version: " + SerenityVersion);
                return;
            }

            if (PackageVersion != null &&
                !NuGet.Versioning.NuGetVersion.TryParse(PackageVersion, out _))
            {
                ExitWithError("Can't parse Package version: " + PackageVersion);
                return;
            }

            if (StartProcess("git", "restore " + PackageBuildProps, Root) != 0)
                ExitWithError("Error while restoring " + PackageBuildProps);

            var xe = XElement.Parse(File.ReadAllText(PackageBuildProps));

            var xeSerenityVer = xe.Descendants("SerenityVersion").FirstOrDefault();
            var changed = false;
            if (xeSerenityVer != null && xeSerenityVer.Value != SerenityVersion)
            {
                xeSerenityVer.SetValue(SerenityVersion);
                changed = true;
            }

            var xeVersion = xe.Descendants("Version").FirstOrDefault();
            if (xeVersion != null)
            {
                var value = (xeVersion.Value ?? "").Trim();
                if (value != (PackageVersion ?? SerenityVersion))
                {
                    changed = true;
                    if (PackageVersion != null)
                        xeVersion.Value = PackageVersion;
                    else if (value.Length > 0)
                    {
                        var projVersion = NuGet.Versioning.NuGetVersion.Parse(value);
                        if (projVersion < serVersion)
                            xeVersion.Value = serVersion.ToString();
                        else if (projVersion.Major == serVersion.Major &&
                            projVersion.Minor == serVersion.Minor &&
                            projVersion.Patch == serVersion.Patch)
                        {
                            xeVersion.Value = new NuGet.Versioning.NuGetVersion(projVersion.Major,
                                projVersion.Minor, projVersion.Patch, projVersion.Revision + 1).ToString();
                        }
                        else
                            xeVersion.Value = serVersion.ToString();
                    }
                    else
                        xeVersion.Value = serVersion.ToString();
                }
                else if (PackageVersion is null)
                {
                    xeVersion.Value = new NuGet.Versioning.NuGetVersion(serVersion.Major,
                        serVersion.Minor, serVersion.Patch, 1).ToString();
                    changed = true;
                }
            }

            if (changed)
                File.WriteAllText(PackageBuildProps, xe.ToString(SaveOptions.OmitDuplicateNamespaces));
        }

        static void PatchDirectoryBuildProps()
        {
            if (SerenityVersion == null)
                return;

            var xe = XElement.Parse(File.ReadAllText(DirectoryBuildProps));

            var xeSerenityVer = xe.Descendants("SerenityVersion").FirstOrDefault();
            var changed = false;
            if (xeSerenityVer != null && xeSerenityVer.Value != SerenityVersion)
            {
                xeSerenityVer.SetValue(SerenityVersion);
                changed = true;
            }

            var xeCFPackageVersion = xe.Descendants("CFPackageVersion").FirstOrDefault();
            if (xeCFPackageVersion?.Value != null)
            {
                var cfPackageBuildProps = Path.Combine(Root, "..", "common-features", "build", "Package.Build.props");
                var xeCF = XElement.Parse(File.ReadAllText(cfPackageBuildProps));
                var cfPackageVersion = xeCF.Descendants("Version").FirstOrDefault()?.Value;
                if (cfPackageVersion != null &&
                    xeCFPackageVersion != null &&
                    xeCFPackageVersion.Value != cfPackageVersion)
                {
                    xeCFPackageVersion.SetValue(cfPackageVersion);
                    changed = true;
                }
            }

            var xeProPackageVersion = xe.Descendants("ProPackageVersion").FirstOrDefault();
            if (xeProPackageVersion?.Value != null)
            {
                var proPackageBuildProps = Path.Combine(Root, "..", "pro-features", "build", "Package.Build.props");
                var xePro = XElement.Parse(File.ReadAllText(proPackageBuildProps));
                var proPackageVersion = xePro.Descendants("Version").FirstOrDefault()?.Value;
                if (proPackageVersion != null &&
                    xeProPackageVersion != null &&
                    xeProPackageVersion.Value != proPackageVersion)
                {
                    xeProPackageVersion.SetValue(proPackageVersion);
                    changed = true;
                }
            }

            if (changed)
                File.WriteAllText(DirectoryBuildProps, xe.ToString(SaveOptions.OmitDuplicateNamespaces));
        }
    }
}
#endif