#if IsFeatureBuild || IsTemplateBuild
using System;
using System.Collections.Generic;
using System.IO;

namespace Build;

public static partial class Shared
{
    public static string Root { get; set; }
    public static bool IsStartSharp { get; set; }

    private static readonly HashSet<string> skipFolders = new(StringComparer.OrdinalIgnoreCase) { "artifacts", "debug", "release", "bin", "obj", "build" };

    public static void DetermineRoot()
    {
        Root = Environment.CurrentDirectory;

        while (skipFolders.Contains(Path.GetFileName(Root)))
            Root = Path.GetDirectoryName(Root);

        Console.WriteLine($"Using root directory: {System.IO.Path.GetFullPath(Root)}");

#if IsTemplateBuild
        IsStartSharp = IsStartSharp ||
            Directory.Exists(Path.Combine(Root, "StartSharp")) ||
            Directory.Exists(Path.Combine(Root, "src", "StartSharp.Web")) ||
            Directory.Exists(Path.Combine(Root, "pro-features")) ||
            File.Exists(Path.Combine(Root, "StartSharp.sln")) ||
            Path.GetFileName(Root)?.StartsWith("start", StringComparison.OrdinalIgnoreCase) == true;
            
        Console.WriteLine($"Using Serenity directory: {System.IO.Path.GetFullPath(SerenityFolder)}");
#else
        Console.WriteLine($"Using Serenity directory: {System.IO.Path.GetFullPath(SerenityDir)}");
#endif
        Console.WriteLine($"Solution file: {System.IO.Path.GetFullPath(SolutionFile)}");
        Console.WriteLine($"Is Patch: {IsPatch}");
        Console.WriteLine($"Is StartSharp: {IsStartSharp}");
    }
}
#endif