using System.IO;

namespace Build;

partial class Program
{
    public static partial class Targets
    {
        public static void Push(bool pushOnly)
        {
            if (!pushOnly)
                Shared.Targets.Pack();

            foreach (var nupkg in Directory.GetFiles(Shared.PackageOutDir, "*.nupkg"))
                if (!Shared.IsProPackage(Path.GetFileName(nupkg)))
                {
                    Shared.PushToRemoteSource(nupkg, Shared.NugetOrgPushSource);
                    Shared.PushToRemoteSource(nupkg, Shared.SerenityIsPackageSource.Source);
                }
        }
    }
}