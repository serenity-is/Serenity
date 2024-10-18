#if IsFeatureBuild || IsTemplateBuild
using System;
using System.IO;
using System.Linq;

namespace Build;

public static partial class Shared
{
    public static string Root { get; set; }
    public static bool IsStartSharp { get; set; }

    public static void DetermineRoot()
    {
        Root = Environment.CurrentDirectory;

        if (new[] { "debug", "release" }.Contains(
                Path.GetFileName(Root).ToLowerInvariant()))
            Root = Path.GetDirectoryName(Root);

        if (Path.GetFileName(Root).ToLowerInvariant() == "bin")
            Root = Path.GetDirectoryName(Root);

        if (Path.GetFileName(Root).ToLowerInvariant() == "build")
            Root = Path.GetDirectoryName(Root);

#if IsTemplateBuild
        IsStartSharp = IsStartSharp ||
            Directory.Exists(Path.Combine(Root, "StartSharp")) ||
            Directory.Exists(Path.Combine(Root, "src", "StartSharp")) ||
            Directory.Exists(Path.Combine(Root, "pro-features")) ||
            File.Exists(Path.Combine(Root, "StartSharp.sln")) ||
            Path.GetFileName(Root)?.ToLowerInvariant() == "startsharp";
#endif
    }

}
#endif