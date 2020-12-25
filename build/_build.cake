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

var msBuildPath = @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\msbuild.exe";
if (!System.IO.File.Exists(msBuildPath))
	msBuildPath = null;

string serenityVersion = null;

var nuspecParams = new Dictionary<string, string> {
    { "authors", "Volkan Ceylan" },
    { "owners", "Volkan Ceylan" },
    { "language", "en-US" },
    { "iconUrl", "https://raw.github.com/serenity-is/Serenity/master/Tools/Images/serenity-logo-128.png" },
    { "licenceUrl", "https://raw.github.com/serenity-is/Serenity/master/LICENSE.md" },
    { "projectUrl", "http://github.com/serenity-is/Serenity" },
    { "copyright", "Copyright (c) Serenity Software, Volkan Ceylan" },
    { "tags", "Serenity" },
    { "configuration", configuration }
};

Func<string, XElement> loadCsProj = (csproj) => {
        return XElement.Parse(System.IO.File.ReadAllText(csproj));
};

Func<string, string> getVersionFromNuspec = (filename) => {
    var nuspec = System.IO.File.ReadAllText(filename);
    var xml = XElement.Parse(nuspec);
    XNamespace ns = "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd";
    return xml.Descendants(ns + "version").First().Value;
};

nuspecParams["assetsVer"] = getVersionFromNuspec(System.IO.Path.Combine(src, @"Serenity.Assets", "Serenity.Assets.nuspec"));

Func<string, System.Xml.XmlDocument> loadXml = path => 
{
    var xml = new System.Xml.XmlDocument();
    xml.LoadXml(System.IO.File.ReadAllText(path));
    return xml;
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
    var prm = new Dictionary<string, string>(nuspecParams);
        
	id = id ?? s;
	project = project ?? s;
    var csproj = System.IO.Path.Combine(src, s, project + ".csproj");
    if (!System.IO.File.Exists(csproj))
        csproj = null;

    var nuspecFile = System.IO.Path.Combine(src, s, id + ".nuspec");
    string version;
    if (!System.IO.File.Exists(nuspecFile) &&
        csproj != null) {
        writeHeader("dotnet pack " + csproj);

        var exitCode = StartProcess("dotnet", "pack " + csproj + " -c:" + configuration + " -o:" + nupkgDir);
        if (exitCode > 0)
            throw new Exception("Error while packing " + csproj);
        version = serenityVersion;
    }
    else {        
        var nuspec = System.IO.File.ReadAllText(nuspecFile);
      
        if (nuspec.IndexOf("<version>${version}</version>") < 0) {
            version = getVersionFromNuspec(nuspecFile);
        }
        else {
            version = serenityVersion;
            nuspec = nuspec.Replace("${version}", version);
        }
        nuspec = nuspec.Replace("${id}", id);
        
        foreach (var p in prm)
            nuspec = nuspec.Replace("${" + p.Key + "}", p.Value);
          
        var assembly = System.IO.Path.Combine(src, s, "bin", configuration, project + ".dll");
        if (!System.IO.File.Exists(assembly))
            assembly = System.IO.Path.Combine(src, s, "bin", configuration, project + ".exe");
          
        if (System.IO.File.Exists(assembly)) 
        {
            var vi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly);
            nuspec = nuspec.Replace("${title}", vi.FileDescription);
            nuspec = nuspec.Replace("${description}", vi.Comments);
        }

        var temp = System.IO.Path.Combine(nupkgDir, id + ".temp.nuspec");
        System.IO.File.WriteAllText(temp, nuspec);
         
        var basePath = System.IO.Path.Combine(src, s);
         
        NuGetPack(temp, new NuGetPackSettings {
            BasePath = basePath,
            OutputDirectory = nupkgDir,
            NoPackageAnalysis = true
        });
        
        System.IO.File.Delete(temp);
    }
    var pkg = System.IO.Path.Combine(nupkgDir, id + "." + version + ".nupkg");
    if (!System.IO.File.Exists(pkg))
        throw new Exception("Package " + pkg + " is not found!");
    nugetPackages.Add(pkg);
};

Action fixNugetCache = delegate() {
	var nugetFeed = @"C:\Sandbox\MyNugetFeed";
	if (!System.IO.Directory.Exists(nugetFeed))
		nugetFeed = System.IO.Path.GetFullPath(System.IO.Path.Combine(root, "..", "MyNugetFeed"));
	if (!System.IO.Directory.Exists(nugetFeed))
		nugetFeed = System.IO.Path.GetFullPath(System.IO.Path.Combine(root, "..", "..", "MyNugetFeed"));
	if (!System.IO.Directory.Exists(nugetFeed))
		nugetFeed = System.IO.Path.GetFullPath(System.IO.Path.Combine(root, "..", "..", "..", "MyNugetFeed"));
    if (System.IO.Directory.Exists(nugetFeed))
    {
        foreach (var package in nugetPackages)
            System.IO.File.Copy(package, System.IO.Path.Combine(nugetFeed, System.IO.Path.GetFileName(package)), true);
    }
};

Action myPush = delegate() {
    foreach (var package in nugetPackages)
    {
        NuGetPush(package, new NuGetPushSettings {
            Source = "https://www.nuget.org/api/v2/package"
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
		
	StartProcess("powershell", new ProcessSettings 
	{ 
		Arguments = "npm install", 
		WorkingDirectory = System.IO.Path.Combine(src, "Serenity.Scripts") 
	});
});

Task("Compile")
    .IsDependentOn("Restore")
    .Does(context => 
{

	StartProcess("powershell", new ProcessSettings 
	{ 
		Arguments = @"npx tsc -p ..\Serenity.Net.CodeGenerator\Resource\tsconfig.json", 
		WorkingDirectory = System.IO.Path.Combine(src, "Serenity.Scripts") 
	});
    var sasmi = System.IO.File.ReadAllText(System.IO.Path.Combine(src, "SharedAssemblyInfo.cs"));
    var sasmi1 = sasmi.IndexOf("AssemblyVersion(\"");
    serenityVersion = sasmi.Substring(sasmi1 + "AssemblyVersion(\"".Length).Split('"')[0];
               
	patchProjectVer(System.IO.Path.Combine(src, "SharedProperties.xml"), serenityVersion);

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
			WorkingDirectory = System.IO.Path.Combine(src, "Serenity.Scripts") 
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
    
    fixNugetCache();
});

Task("Push")
    .IsDependentOn("Pack")
    .Does(() => 
    {
        myPush();
    });
 
Task("Assets-Pack")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        myPack("Serenity.Assets", null, null);
        fixNugetCache();
    });

Task("Assets-Push")
    .IsDependentOn("Assets-Pack")
    .Does(() =>
    {
        myPush();
    });

RunTarget(target);