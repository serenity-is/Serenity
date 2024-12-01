using Cake.Common;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Pack;
using Cake.Core;
using Cake.Frosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Build;

public class BuildContext : FrostingContext
{
    public string BuildConfiguration { get; }
    public bool LocalPush { get; }
    public string SerenityDir { get; }
    public string SerenitySrc => Path.Combine(SerenityDir, "src");
    public string SerenityNetSln => Path.Combine(SerenitySrc, "Serenity.Net.slnf");
    public string SerenityPackages => Path.Combine(SerenityDir, "packages");
    public string SerenityNupkg => Path.Combine(SerenityDir, "build", ".nupkg");
    public string CorelibDir => Path.Combine(SerenityDir, "packages", "corelib");
    public static string MyPackagesDir => Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile),
        ".nuget", "my-packages");

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

    public BuildContext(ICakeContext context) : base(context)
    {
        BuildConfiguration = context.Argument("conguration", "Release");
        LocalPush = context.Argument("localpush", false);

        string workingDir = context.Environment.WorkingDirectory.ToString();

        while (SkipDirsToRoot.Contains(Path.GetFileName(workingDir)))
            Path.Combine(workingDir, "..");

        string tryPath(string relative)
        {
            var path = Path.Combine(workingDir, relative);
            if (File.Exists(Path.Combine(path, "Serenity.sln")))
                return path;
            return null;
        }

        SerenityDir = tryPath(".") ?? tryPath("Serenity") ?? tryPath("..") ??
            throw new Exception("Couldn't locate Serenity.sln from current directory!");

        context.Environment.WorkingDirectory = SerenityDir;
    }
}

public static partial class Utilities
{
    public static void WriteHeader(string header)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("*** " + header + " ***");
        Console.ResetColor();
    }

    public static void PushToMyPackages(this BuildContext context, string nupkgDir)
    {
        var myPackagesDir = BuildContext.MyPackagesDir;
        if (!Directory.Exists(myPackagesDir))
        {
            Directory.CreateDirectory(myPackagesDir);
            context.DotNetNuGetAddSource("MyPackages", new()
            {
                Source = myPackagesDir
            });
        }

        foreach (var package in Directory.GetFiles(nupkgDir, "*.nupkg"))
        {
            var filename = Path.ChangeExtension(Path.GetFileName(package), null);
            var match = NuPkgIdVersionRegex().Match(filename);
            if (match != null && match.Groups.Count >= 2)
            {
                var id = match.Groups[1].Value;
                var version = match.Groups[2].Value[1..];
                var dir = Path.Combine(myPackagesDir, id, version);
                if (Directory.Exists(dir))
                    Directory.Delete(dir, true);
            }

            context.DotNetNuGetPush(package, new()
            {
                Source = myPackagesDir
            });
        }
    }

    public static void PushPackages(this ICakeContext context, string nupkgDir, params string[] toSources)
    {
        foreach (var package in Directory.GetFiles(nupkgDir, "*.nupkg"))
        {
            foreach (var toSource in toSources)
            {
                context.DotNetNuGetPush(package, new()
                {
                    Source = toSource,
                    SkipDuplicate = true
                });
            }
        }
    }

    [System.Text.RegularExpressions.GeneratedRegex(@"(.+?)((\.[0-9]+)+)")]
    private static partial System.Text.RegularExpressions.Regex NuPkgIdVersionRegex();
}

public sealed class Clean : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.CleanDirectory(context.SerenityNupkg);
        context.CreateDirectory(context.SerenityNupkg);
        context.CleanDirectories(context.SerenitySrc + "/Serenity.*/**/bin/" + context.Configuration);
        context.CleanDirectories(context.SerenityDir + "/tests/**/bin/");
        context.CleanDirectories(context.SerenityDir + "/packages/*/out");
    }
}


[IsDependentOn(typeof(Clean))]
public sealed class Compile : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        Utilities.WriteHeader($"Building {context.SerenityNetSln}");
        context.DotNetBuild(context.SerenityNetSln, new()
        {
            Configuration = context.BuildConfiguration,
            Verbosity = DotNetVerbosity.Minimal
        });
    }
}

[IsDependentOn(typeof(Compile))]
public sealed class DotNetTests : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var projects = context.GetFiles(context.SerenityDir + "tests/**/*.csproj");
        foreach (var project in projects)
        {
            context.DotNetTest(project.FullPath, new()
            {
                Configuration = context.BuildConfiguration,
                NoBuild = true
            });
        }
    }
}

[IsDependentOn(typeof(Compile))]
public sealed class JsTests : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    { 
        context.StartProcess("powershell", new Cake.Core.IO.ProcessSettings
        {
            Arguments = "pnpm test",
            WorkingDirectory = context.CorelibDir
        });
    }
}

internal class DotNetPacker(BuildContext context, string baseDir, string nupkgDir)
{
    private readonly string baseDir = baseDir ?? throw new ArgumentNullException(nameof(baseDir));
    private readonly string nupkgDir = nupkgDir ?? throw new ArgumentNullException(nameof(DotNetPacker.nupkgDir));

    public void Pack(string project)
    {
        var projectFile = Path.Combine(baseDir, project);
        if (!File.Exists(projectFile))
        {
            Console.Error.WriteLine(projectFile + " not found!");
            Environment.Exit(1);
        }

        Utilities.WriteHeader("dotnet pack " + projectFile);
        context.DotNetPack(projectFile, new DotNetPackSettings
        {
            Configuration = context.BuildConfiguration,
            OutputDirectory = nupkgDir,
            ArgumentCustomization = args => args.Append("-p:ContinuousIntegrationBuild=true")
        });
    }
}

[IsDependentOn(typeof(DotNetTests))]
[IsDependentOn(typeof(JsTests))]
[TaskDescription("Pack Serenity.Net projects")]
public sealed class Pack : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var packer = new DotNetPacker(context, context.SerenitySrc, context.SerenityNupkg);
        packer.Pack("Serenity.Net.Core/Serenity.Net.Core.csproj");
        packer.Pack("Serenity.Net.Services/Serenity.Net.Services.csproj");
        packer.Pack("Serenity.Net.Web/Serenity.Net.Web.csproj");
        packer.Pack("Serenity.Net.CodeGenerator/Serenity.Net.CodeGenerator.csproj");
        packer.Pack("Serenity.Assets/Serenity.Assets.csproj");
        packer.Pack("../packages/corelib/Serenity.Corelib.csproj");
    }
}

public sealed class LocalPushOnly : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.PushToMyPackages(context.SerenityNupkg);
    }
}

[IsDependentOn(typeof(Pack))]
public sealed class LocalPush : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context)
    {
        return context.LocalPush;
    }

    public override void Run(BuildContext context)
    {
        context.PushToMyPackages(context.SerenityNupkg);
    }
}

[IsDependentOn(typeof(Pack))]
[IsDependentOn(typeof(LocalPush))]
public sealed class Push : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.PushPackages(context.SerenityNupkg,
            "https://api.nuget.org/v3/index.json",
            "serenity.is");
    }
}

[IsDependentOn(typeof(Pack))]
public sealed class Default : FrostingTask<BuildContext>
{
}

public static class Program
{
    public static int Main(string[] args)
    {
        if (args?.Length > 0 && args[0].All(char.IsLetter))
            args[0] = "--target=" + args[0];

        return new CakeHost()
            .UseContext<BuildContext>()
            .Run(args);
    }
}