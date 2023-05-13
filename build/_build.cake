#addin "nuget:https://www.nuget.org/api/v2?package=Newtonsoft.Json&version=9.0.1"

using System.Xml.Linq;
using Newtonsoft.Json.Linq;

var target = Argument("target", "Pack");
if (target == "")
    target = "Pack";
    
var configuration = Argument("configuration", "Release");
var nupkgDir = System.IO.Path.Combine(System.IO.Path.GetFullPath("."), ".nupkg");
var root = System.IO.Path.GetFullPath(@"..");
var src = System.IO.Path.Combine(root, "src");

var msBuildPath = @"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\msbuild.exe";
if (!System.IO.File.Exists(msBuildPath))
    msBuildPath = null;

Func<string, XElement> loadCsProj = (csproj) => {
        return XElement.Parse(System.IO.File.ReadAllText(csproj));
};

var nugetPackages = new List<string>();

Func<string, JObject> loadJson = path => {
    var content = System.IO.File.ReadAllText(path, Encoding.UTF8);
    return JObject.Parse(content);
};

Action<string, string> patchProjectVer = (csproj, version) => {
    var changed = false;
    var csprojElement = loadCsProj(csproj);
    var versionElement = csprojElement.Descendants("VersionPrefix").First();
    var current = versionElement.Value;
    
    if (current != version)
    {
        versionElement.Value = version;
        changed = true;
    }

    if (changed)
        System.IO.File.WriteAllText(csproj, csprojElement.ToString(SaveOptions.OmitDuplicateNamespaces), Encoding.UTF8);
};

Action<string> writeHeader = (header) => {
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("*** " + header + " ***");
    Console.ResetColor();
};

Action<string, string, string> myPack = (s, id, project) => {
        
    id = id ?? s;
    project = project ?? s;
    var csproj = System.IO.Path.Combine(src, s, project + ".csproj");
    if (!System.IO.File.Exists(csproj))
	{
		Console.Error.WriteLine(csproj + " not found!");
        Environment.Exit(1);
	}

	writeHeader("dotnet pack " + csproj);

	var exitCode = StartProcess("dotnet", "pack " + csproj + " -p:ContinuousIntegrationBuild=true -c:" + configuration + " -o:\"" + nupkgDir + "\"");
	if (exitCode > 0)
		throw new Exception("Error while packing " + csproj);
};

Action fixNugetCache = delegate() {
    var myPackagesDir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "my-packages");
    if (!System.IO.Directory.Exists(myPackagesDir)) 
    {
        System.IO.Directory.CreateDirectory(myPackagesDir);
        StartProcess("nuget", new ProcessSettings 
        {
            Arguments = "sources add -Name MyPackages -Source \"" + myPackagesDir + "\""
        });
    }
    foreach (var package in System.IO.Directory.GetFiles(nupkgDir, "*.nupkg"))
    {
        var filename = System.IO.Path.ChangeExtension(System.IO.Path.GetFileName(package), null);
        var match = System.Text.RegularExpressions.Regex.Match(filename, @"(.+?)((\.[0-9]+)+)");
        if (match != null && match.Groups.Count >= 2)
        {
            var id = match.Groups[1].Value;
            var version = match.Groups[2].Value.Substring(1);
            var dir = System.IO.Path.Combine(myPackagesDir, id, version);
            if (System.IO.Directory.Exists(dir))
                System.IO.Directory.Delete(dir, true);
        }
    
        NuGetPush(package, new NuGetPushSettings {
            Source = myPackagesDir
        });
    }
};

Action myPush = delegate() {
    foreach (var package in System.IO.Directory.GetFiles(nupkgDir, "*.nupkg"))
    {
        NuGetPush(package, new NuGetPushSettings {
            Source = "https://api.nuget.org/v3/index.json"
        });
    }       
};

Action<Dictionary<string, string>, JObject, string> addDeps = (p, deps, fw) => {
    if (deps == null)
        return;
    
    foreach (var pair in deps) {
        if (pair.Value is JObject) {
            var o = (pair.Value as JObject)["version"] as JValue;
            if (o != null && o.Value != null)
                p[fw + ":" + pair.Key] = o.Value.ToString();
        }
        else if (pair.Value is JValue && (pair.Value as JValue).Value != null) {
            p[fw + ":" + pair.Key] = (pair.Value as JValue).Value.ToString();
        }
    }

};

Task("Clean")
    .Does(() =>
{
    CleanDirectories(nupkgDir);
    CreateDirectory(nupkgDir);
    CleanDirectories(src + "/Serenity.*/**/bin/" + configuration);
    CleanDirectories(root + "tests/**/bin/");
    CleanDirectories(src + "/Serenity.Scripts/dist");
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(context =>
{
    var dotnetSln = System.IO.Path.Combine(src, "Serenity.Net.sln");
    writeHeader("dotnet restore " + dotnetSln);
    var exitCode = StartProcess("dotnet", "restore " + dotnetSln);
    if (exitCode > 0)
        throw new Exception("Error while restoring " + dotnetSln);
});

Task("Compile")
    .IsDependentOn("Restore")
    .Does(context => 
{

    StartProcess("powershell", new ProcessSettings 
    { 
        Arguments = @"npx tsc -p ..\..\Serenity.Net.CodeGenerator\Resource\tsconfig.json", 
        WorkingDirectory = System.IO.Path.Combine(root, "packages", "corelib") 
    });

    writeHeader("Building Serenity.Net.sln");
    MSBuild(System.IO.Path.Combine(src, "Serenity.Net.sln"), s => {
        s.SetConfiguration(configuration);
        s.ToolPath = msBuildPath;
        s.Verbosity = Verbosity.Minimal;
    });
});

Task("Test")
    .IsDependentOn("Compile")
    .Does(() =>
{
        var projects = GetFiles(root + "/tests/**/*.csproj");
        foreach(var project in projects)
        {
            DotNetCoreTest(
                project.FullPath,
                new DotNetCoreTestSettings()
                {
                    Configuration = configuration,
                    NoBuild = true
                });
        }
        
        StartProcess("powershell", new ProcessSettings 
        { 
            Arguments = "npx jest", 
            WorkingDirectory = System.IO.Path.Combine(root, "packages", "corelib") 
        });

});

Task("Pack")
    .IsDependentOn("Test")
    .Does(() =>
{   
    // https://github.com/NuGet/Home/issues/7001
    var dateTime = new DateTime(2001, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    foreach (var fileInfo in new System.IO.DirectoryInfo(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget")).GetFiles("*.*", System.IO.SearchOption.AllDirectories)) {
        if (fileInfo.Exists && fileInfo.LastWriteTimeUtc < dateTime)
        {
            try
            {
                fileInfo.LastWriteTimeUtc = dateTime;
            }
            catch (Exception)
            {
                Console.WriteLine(String.Format("Could not reset {LastWriteTime} {File} in nuspec",
                    nameof(fileInfo.LastWriteTimeUtc),
                    fileInfo.FullName));
            }
        }
    }
        
    myPack("Serenity.Net.Core", null, null);
    myPack("Serenity.Net.Data", null, null);
    myPack("Serenity.Net.Entity", null, null);
    myPack("Serenity.Net.Services", null, null);
    myPack("Serenity.Net.Web", null, null);
    myPack("Serenity.Scripts", null, null);
    myPack("Serenity.Net.CodeGenerator", "sergen", null);
    myPack("Serenity.Assets", null, null);
    
    fixNugetCache();
});

Task("Push")
    .IsDependentOn("Pack")
    .Does(() => 
    {
        myPush();
    });
 
RunTarget(target);