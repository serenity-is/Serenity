using Cake.Common;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Pack;
using Cake.Common.Tools.NuGet;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace build;

public class Context : FrostingContext
{
    public string Config { get; }
    public bool LocalPush { get; }
    public DirectoryPath RootDir { get; }
    public DirectoryPath SrcDir { get; }
    public FilePath SerenityNetSln { get; }
    public string NuPkgDir { get; }

    private static readonly HashSet<string> SkipDirsToRoot = new(StringComparer.OrdinalIgnoreCase)
    {
        ".build",
        "artifacts",
        "bin",
        "build",
        "cake",
        "Debug",
        "obj",
        "Release"
    };

    public Context(ICakeContext context) : base(context)
    {
        Config = context.Argument("conguration", "Release");
        LocalPush = context.Argument("localpush", false);

        while (SkipDirsToRoot.Contains(System.IO.Path.GetFileName(context.Environment.WorkingDirectory.FullPath)))
            context.Environment.WorkingDirectory = context.Environment.WorkingDirectory.Combine("../");

        RootDir = context.Directory("./");
        NuPkgDir = RootDir + context.Directory("build") + context.Directory(".nupkg");
        SrcDir = RootDir + context.Directory("src");
        SerenityNetSln = SrcDir.CombineWithFilePath("Serenity.Net.slnf");
    }
}

public static partial class Utilities
{
    public static string ReadAllText(this ICakeContext context, FilePath path)
    {
        var file = context.FileSystem.GetFile(path);
        using var stream = file.OpenRead();
        using var reader = new System.IO.StreamReader(stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    public static void WriteHeader(string header)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("*** " + header + " ***");
        Console.ResetColor();
    }

    static void PackProject(this Context context, string projectFile, string packageId = null)
    {
        packageId ??= System.IO.Path.GetFileNameWithoutExtension(projectFile);

        var csproj = context.SrcDir.CombineWithFilePath(projectFile);
        if (!context.FileExists(csproj))
        {
            Console.Error.WriteLine(csproj.FullPath + " not found!");
            Environment.Exit(1);
        }

        WriteHeader("dotnet pack " + csproj);
        context.DotNetPack(csproj.FullPath, new DotNetPackSettings
        {
            Configuration = context.Config,
            OutputDirectory = context.NuPkgDir,
            ArgumentCustomization = args => args.Append("-p:ContinuousIntegrationBuild=true")
        });
    }

    public static void FixNugetCache(this Context context)
    {
        var myPackagesDir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "my-packages");
        if (!System.IO.Directory.Exists(myPackagesDir))
        {
            System.IO.Directory.CreateDirectory(myPackagesDir);
            context.NuGetAddSource("MyPackages", myPackagesDir);
        }

        foreach (var package in System.IO.Directory.GetFiles(context.NuPkgDir, "*.nupkg"))
        {
            var filename = System.IO.Path.ChangeExtension(System.IO.Path.GetFileName(package), null);
            var match = NuPkgIdVersionRegex().Match(filename);
            if (match != null && match.Groups.Count >= 2)
            {
                var id = match.Groups[1].Value;
                var version = match.Groups[2].Value[1..];
                var dir = System.IO.Path.Combine(myPackagesDir, id, version);
                if (System.IO.Directory.Exists(dir))
                    System.IO.Directory.Delete(dir, true);
            }

            context.NuGetPush(package, new()
            {
                Source = myPackagesDir
            });
        }
    }

    public static void PushPackages(this Context context)
    {
        foreach (var package in System.IO.Directory.GetFiles(context.NuPkgDir, "*.nupkg"))
        {
            context.NuGetPush(package, new()
            {
                Source = "https://api.nuget.org/v3/index.json",
                SkipDuplicate = true
            });

            context.NuGetPush(package, new()
            {
                Source = "serenity.is",
                SkipDuplicate = true
            });
        }
    }

    public static void PackAllProjects(this Context context)
    {
        // https://github.com/NuGet/Home/issues/7001
        var dateTime = new DateTime(2001, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        foreach (var fileInfo in new System.IO.DirectoryInfo(System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget"))
            .GetFiles("*.*", System.IO.SearchOption.AllDirectories))
        {
            if (fileInfo.Exists && fileInfo.LastWriteTimeUtc < dateTime)
            {
                try
                {
                    fileInfo.LastWriteTimeUtc = dateTime;
                }
                catch (Exception)
                {
                    Console.WriteLine($"Could not reset {nameof(fileInfo.LastWriteTimeUtc)} {fileInfo.FullName} in nuspec");
                }
            }
        }

        context.PackProject("Serenity.Net.Core/Serenity.Net.Core.csproj");
        context.PackProject("Serenity.Net.Data/Serenity.Net.Data.csproj");
        context.PackProject("Serenity.Net.Entity/Serenity.Net.Entity.csproj");
        context.PackProject("Serenity.Net.Services/Serenity.Net.Services.csproj");
        context.PackProject("Serenity.Net.Web/Serenity.Net.Web.csproj");
        context.PackProject("Serenity.Net.CodeGenerator/Serenity.Net.CodeGenerator.csproj", packageId: "sergen");
        context.PackProject("Serenity.Assets/Serenity.Assets.csproj");
        context.PackProject("../packages/corelib/Serenity.CoreLib.csproj");
    }

    [System.Text.RegularExpressions.GeneratedRegex(@"(.+?)((\.[0-9]+)+)")]
    private static partial System.Text.RegularExpressions.Regex NuPkgIdVersionRegex();
}

public sealed class Clean : FrostingTask<Context>
{
    public override void Run(Context context)
    {
        context.CleanDirectories(context.NuPkgDir);
        context.CreateDirectory(context.NuPkgDir);
        context.CleanDirectories(context.SrcDir + "/Serenity.*/**/bin/" + context.Configuration);
        context.CleanDirectories(context.RootDir + "tests/**/bin/");
        context.CleanDirectories(context.RootDir + "/packages/*/out");
    }
}


[IsDependentOn(typeof(Clean))]
public sealed class Compile : FrostingTask<Context>
{
    public override void Run(Context context)
    {
        Utilities.WriteHeader($"Building {context.SerenityNetSln}");
        context.DotNetBuild(context.SerenityNetSln.FullPath, new()
        {
            Configuration = context.Config,
            Verbosity = DotNetVerbosity.Minimal
        });
    }
}

[IsDependentOn(typeof(Compile))]
public sealed class DotNetTests : FrostingTask<Context>
{
    public override void Run(Context context)
    {
        var projects = context.GetFiles(context.RootDir + "tests/**/*.csproj");
        foreach (var project in projects)
        {
            context.DotNetTest(project.FullPath, new()
            {
                Configuration = context.Config,
                NoBuild = true
            });
        }
    }
}

[IsDependentOn(typeof(Compile))]
public sealed class JsTests : FrostingTask<Context>
{
    public override void Run(Context context)
    { 
        context.StartProcess("powershell", new ProcessSettings
        {
            Arguments = "pnpm test",
            WorkingDirectory = System.IO.Path.Combine(context.RootDir.FullPath, "packages", "corelib")
        });
    }
}

[IsDependentOn(typeof(DotNetTests))]
[IsDependentOn(typeof(JsTests))]
public sealed class Pack : FrostingTask<Context>
{
    public override void Run(Context context)
    {
        context.PackAllProjects();
    }
}

public sealed class LocalPushOnly : FrostingTask<Context>
{
    public override void Run(Context context)
    {
        context.FixNugetCache();
    }
}

[IsDependentOn(typeof(Pack))]
public sealed class LocalPush : FrostingTask<Context>
{
    public override bool ShouldRun(Context context)
    {
        return context.LocalPush;
    }

    public override void Run(Context context)
    {
        context.FixNugetCache();
    }
}

[IsDependentOn(typeof(Pack))]
[IsDependentOn(typeof(LocalPush))]
public sealed class Push : FrostingTask<Context>
{
    public override void Run(Context context)
    {
        context.PushPackages();
    }
}

[IsDependentOn(typeof(Pack))]
public sealed class Default : FrostingTask<Context>
{
}

public static class Program
{
    public static int Main(string[] args)
    {
        if (args?.Length > 0 && args[0].All(char.IsLetter))
            args[0] = "--target=" + args[0];

        return new CakeHost()
            .UseContext<Context>()
            .Run(args);
    }
}