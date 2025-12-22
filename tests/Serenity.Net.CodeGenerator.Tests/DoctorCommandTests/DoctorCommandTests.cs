using System.IO;
using System.Security.Cryptography;

namespace Serenity.CodeGenerator;

public partial class DoctorCommandTests
{
    [Fact]
    public void Prints_Project_File()
    {
        var (command, env) = CreateCommand(projectFile: @"/MyProjectName.csproj");
        var exitCode = command.Run();
        Assert.Equal(ExitCodes.Success, exitCode);
        Assert.Contains(env.Console.WriteCalls, x =>
            x.data is ConsoleColor.Cyan &&
            x.message.Contains("Project File:"));
        Assert.Contains(env.Console.WriteCalls, x =>
            x.data is null &&
            x.message.Contains("MyProjectName.csproj"));
    }

    [Fact]
    public void Checks_If_Project_FileName_IsPascalCase()
    {
        var (command, env) = CreateCommand(projectFile: @"/MyTest.Web/myTest.Web.csproj");
        var exitCode = command.Run();
        Assert.Equal(ExitCodes.Exception, exitCode);
        Assert.Contains(env.Console.WriteCalls, x =>
            x.data is ConsoleColor.Red &&
            x.message.Contains("Project filename should start with a capital letter"));
    }

    [Fact]
    public void DoesNotError_If_Project_FileName_IsPascalCase()
    {
        var (command, env) = CreateCommand(projectFile: @"/MyTest.Web/MyTest.Web.csproj");
        var exitCode = command.Run();
        Assert.Equal(ExitCodes.Success, exitCode);
        Assert.DoesNotContain(env.Console.WriteCalls, x =>
            x.message.Contains("Project filename should start with a capital letter"));
    }

    private class MockEnv
    {
        public static readonly string SergenVersion = typeof(DoctorCommand).Assembly.GetName().Version.ToString();

        public string ProjectFile { get; set; }
        public string ProjectDirectory => FileSystem.GetDirectoryName(ProjectFile);
        public ProjectFileInfo Project { get; set; }
        public MockFileSystem FileSystem { get; set; } = new();
        public MockGeneratorConsole Console { get; set; } = new();
        public MockProcessExecutor Executor = new();

        public Dictionary<string, string> MsBuildPackageReferences { get; set; } = new()
        {
            ["Serenity.Assets"] = SergenVersion,
            ["Serenity.Corelib"] = SergenVersion,
            ["Serenity.Net.Web"] = SergenVersion,
            ["Serenity.Pro.Extensions"] = SergenVersion,
            ["Serenity.Pro.Coder"] = SergenVersion
        };

        public List<string> MsBuildProjectReferences { get; set; } = [];

        public ProjectFileInfo.ProjectProperties MsBuildProperties { get; set; } = new()
        {
            TargetFramework = "net10.0",
            EsmAssetBasePath = null,
            OutDir = @"bin\Debug\net10.0\"
        };

        public Dictionary<string, string> NpmDependencies { get; set; } = new()
        {
            ["@serenity-is/corelib"] = "./node_modules/.dotnet/serenity.corelib",
            ["@serenity-is/domwise"] = "./node_modules/.dotnet/serenity.domwise",
            ["@serenity-is/extensions"] = "./node_modules/.dotnet/serenity.extensions",
            ["@serenity-is/pro.extensions"] = "./node_modules/.dotnet/serenity.pro.extensions",
            ["@serenity-is/sleekgrid"] = "./node_modules/.dotnet/serenity.sleekgrid"
        };

        public Dictionary<string, string> NpmDevDependencies { get; set; } = new()
        {
            ["@serenity-is/tsbuild"] = "9.1.6"
        };

        public string SergenExtends { get; set; } = "defaults@9.0.0";
        public string SergenRootNamespace { get; set; }

        public void WriteSergenJson()
        {
            FileSystem.AddFile(FileSystem.Combine(ProjectDirectory, "sergen.json"),
                JSON.StringifyIndented(new
                {
                    Extends = SergenExtends,
                    RootNamespace = SergenRootNamespace,
                    UpgradeInfo = new
                    {
                        InitialType = "Premium",
                        InitialVersion = SergenVersion
                    }
                }));
        }

        public void WritePackageJson()
        {
            FileSystem.AddFile(
                FileSystem.Combine(ProjectDirectory, "package.json"),
                JSON.StringifyIndented(new
                {
                    name = Path.GetFileNameWithoutExtension(ProjectFile).ToLowerInvariant(),
                    dependencies = NpmDependencies,
                    devDependencies = NpmDevDependencies,
                    type = "module",
                    version = "1.0.0"
                }));
        }

        public Dictionary<string, object> TsConfigCompilerOptions { get; set; } = new()
        {
            ["esModuleInterop"] = true,
            ["experimentalDecorators"] = true,
            ["forceConsistentCasingInFileNames"] = true,
            ["module"] = "esnext",
            ["moduleResolution"] = "bundler",
            ["target"] = "ES2022",
            ["useDefineForClassFields"] = false
        };

        public void WriteTsConfigJson()
        {
            FileSystem.AddFile(FileSystem.Combine(ProjectDirectory, "tsconfig.json"),
                JSON.StringifyIndented(new
                {
                    compilerOptions = TsConfigCompilerOptions,
                    include = modulesIncludeArray
                }));
        }

        public string NodeVersion = "24.10.0";
        public string NpmVersion = "11.6.2";
        
        internal static readonly string[] modulesIncludeArray = ["Modules"];
    }

    private (DoctorCommand, MockEnv) CreateCommand(
        string projectFile = @"/Repos/MyTest.Web/MyTest.Web.csproj")
    {
        var environment = new MockEnv
        {
            FileSystem = new MockFileSystem(),
            ProjectFile = projectFile
        };

        environment.MsBuildProperties.AssemblyName = environment.MsBuildProperties.RootNamespace =
            environment.FileSystem.GetFileNameWithoutExtension(environment.ProjectFile);

        environment.Executor.OnStart((s, i) =>
        {
            string cmd = s.StartInfo.FileName + " " + s.StartInfo.Arguments;

            if (cmd.Contains("dotnet msbuild", StringComparison.Ordinal))
                return (true, JSON.StringifyIndented(new
                {
                    Properties = environment.MsBuildProperties,
                    Items = new
                    {
                        PackageReference = environment.MsBuildPackageReferences.Select(x => new
                        {
                            Identity = x.Key,
                            Version = x.Value
                        }).ToList(),
                        ProjectReference = environment.MsBuildProjectReferences.Select(x => new
                        {
                            Filename = x
                        }).ToList()
                    }
                }), "");


            if (cmd.Contains("node --version", StringComparison.Ordinal))
                return (true, environment.NodeVersion, "");

            if (cmd.Contains("npm --version", StringComparison.Ordinal))
                return (true, environment.NpmVersion, "");

            if (cmd.Contains("tsc --showConfig"))
                return (true, JSON.StringifyIndented(new
                {
                    compilerOptions = environment.TsConfigCompilerOptions,
                    include = MockEnv.modulesIncludeArray
                }), "");

            return (true, "", "");
        });

        environment.Project = new ProjectFileInfo(environment.FileSystem, environment.ProjectFile);
        environment.FileSystem.AddFile(environment.ProjectFile, "<Project Sdk=\"Microsoft.NET.Sdk.Web\"></Project>");
        environment.WriteSergenJson();
        environment.WritePackageJson();
        environment.WriteTsConfigJson();

        return (new DoctorCommand(environment.Project, environment.Console, environment.Executor), environment);
    }

}
