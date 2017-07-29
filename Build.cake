﻿#addin "nuget:https://www.nuget.org/api/v2?package=Newtonsoft.Json&version=9.0.1"

using System.Xml.Linq;
using Newtonsoft.Json.Linq;

var target = Argument("target", "Pack");
if (target == "")
	target = "Pack";
	
var configuration = Argument("configuration", "Release");

string serenityVersion = null;

var nuspecParams = new Dictionary<string, string> {
    { "authors", "Volkan Ceylan" },
    { "owners", "Volkan Ceylan" },
    { "language", "en-US" },
    { "iconUrl", "https://raw.github.com/volkanceylan/Serenity/master/Tools/Images/serenity-logo-128.png" },
    { "licenceUrl", "https://raw.github.com/volkanceylan/Serenity/master/LICENSE.md" },
    { "projectUrl", "http://github.com/volkanceylan/Serenity" },
    { "copyright", "Copyright (c) Volkan Ceylan" },
    { "tags", "Serenity" },
    { "framework", "net45" },
    { "configuration", configuration }
};

var dotnetBuildOrder = new string[] {
    "Serenity.Core",
    "Serenity.Caching.Couchbase",
    "Serenity.Caching.Redis",
    "Serenity.Data",
    "Serenity.Data.Entity",
    "Serenity.Services",
    "Serenity.Web",
    "Serenity.CodeGenerator"
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

Func<string, System.Xml.XmlDocument> loadXml = path => 
{
    var xml = new System.Xml.XmlDocument();
    xml.LoadXml(System.IO.File.ReadAllText(path));
    return xml;
};

nuspecParams["scriptFramework"] = loadXml(@".\Serenity.Script.Imports\packages.config").SelectSingleNode("//package[@id='Saltarelle.Runtime']/@targetFramework").Value; 
nuspecParams["serenityWebAssetsVersion"] = getVersionFromNuspec(@".\Serenity.Web\Serenity.Web.Assets.nuspec");
nuspecParams["serenityWebToolingVersion"] = getVersionFromNuspec(@".\Serenity.Web\Serenity.Web.Tooling.nuspec");


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


Func<string, string, string> getPackageVersion = (project, package) => 
{
    var node = loadXml(@".\" + project + @"\packages.config").SelectSingleNode("//package[@id='" + package + "']/@version");
    if (node == null || node.Value == null)
        throw new InvalidOperationException("Couldn't find version for " + package + " in project " + project);
    return node.Value;
};

Action<string> minimizeJs = filename => {
    StartProcess("./Tools/Node/uglifyjs.cmd", new ProcessSettings 
    {
        Arguments = filename + 
            " -o " + System.IO.Path.ChangeExtension(filename, "min.js") + 
            " --comments --mangle"
    });
};

Action runGitLink = () => {
    StartProcess("./Tools/GitLink/GitLink.exe", new ProcessSettings
    { 
        Arguments = System.IO.Path.GetFullPath(@".\") + " -u https://github.com/volkanceylan/serenity"
    });
};

Func<string, List<Tuple<string, string, string>>> parsePackageVersions = (path) => {
    var config = System.IO.File.ReadAllText(path);
    var xml = XElement.Parse(config);
    var list = new List<Tuple<string, string, string>>();
    return xml.Descendants("package").Select(el =>
        new Tuple<string, string, string>(el.Attribute("id").Value, 
            el.Attribute("version").Value, el.Attribute("targetFramework").Value))
                .ToList();
};

Action<string, string, string> myPackEx = (projDir, s, id) => {
    var prm = new Dictionary<string, string>(nuspecParams);

    var packagesConfig = "./" + s + ".Net45/packages.config";
    if (!System.IO.File.Exists(packagesConfig))
        packagesConfig = "./" + s + "/packages." + s + ".Net45.config";
    if (!System.IO.File.Exists(packagesConfig))
        packagesConfig = "./" + s + "/packages.config";
    if (!System.IO.File.Exists(packagesConfig))
		packagesConfig = null;
        
    var csproj = "./" + projDir + "/" + s + ".csproj";
    if (!System.IO.File.Exists(csproj))
        csproj = null;

    if (s == "Serenity.Web" || s == "Serenity.Web.Core")
        setPackageVersions(prm, null, "./Serenity.Test/packages.Serenity.Test.Net45.config");
        
    setPackageVersions(prm, csproj, packagesConfig);
    
    var filename = "./" + projDir + "/" + (id ?? s) + ".nuspec";
    var nuspec = System.IO.File.ReadAllText(filename);
    string version;
  
    if (nuspec.IndexOf("<version>${version}</version>") < 0) {
        version = getVersionFromNuspec(filename);
    }
    else {
        version = serenityVersion;
        nuspec = nuspec.Replace("${version}", version);
    }
    nuspec = nuspec.Replace("${id}", (id ?? s));
    
    foreach (var p in prm)
        nuspec = nuspec.Replace("${" + p.Key + "}", p.Value);
      
    var assembly = "./" + projDir + ".Net45/bin/" + configuration + "/" + s + ".dll";
    if (!System.IO.File.Exists(assembly))
        assembly = "./" + projDir + ".Net45/bin/" + configuration + "/" + s + ".exe";
    
    if (!System.IO.File.Exists(assembly))
        assembly = "./" + projDir + "/bin/" + configuration + "/net46/" + s + ".dll";
    if (!System.IO.File.Exists(assembly))
        assembly = "./" + projDir + "/bin/" + configuration + "/net46/" + s + ".exe";
	if (!System.IO.File.Exists(assembly))
		assembly = "./" + projDir + "/bin/" + configuration + "/" + s + ".dll";
    if (!System.IO.File.Exists(assembly))
        assembly = "./" + projDir + "/bin/" + configuration + "/" + s + ".exe";
      
    if (System.IO.File.Exists(assembly)) 
    {
        var vi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly);
        nuspec = nuspec.Replace("${title}", vi.FileDescription);
        nuspec = nuspec.Replace("${description}", vi.Comments);
    }

    var temp = "./.nupkg/" + s + ".temp.nuspec";
    System.IO.File.WriteAllText(temp, nuspec);
     
    var basePath = @"./" + projDir;
     
    NuGetPack(temp, new NuGetPackSettings {
        BasePath = basePath,
        OutputDirectory = "./.nupkg",
        NoPackageAnalysis = true
    });
    
    System.IO.File.Delete(temp);
    nugetPackages.Add("./.nupkg/" + (id ?? s) + "." + version + ".nupkg");
};

Action fixNugetCache = delegate() {
    if (System.IO.Directory.Exists(@"C:\Sandbox\MyNugetFeed"))
    {
        foreach (var package in nugetPackages)
            System.IO.File.Copy(package, @"C:\Sandbox\MyNugetFeed\" + System.IO.Path.GetFileName(package), true);
            
        /*foreach (var package in System.IO.Directory.GetFiles(
            System.IO.Path.Combine(System.Environment.GetFolderPath(
                System.Environment.SpecialFolder.LocalApplicationData), @"nuget\cache"), "Seren*.nupkg"))
            System.IO.File.Delete(package);*/
    }
};

Action<string, string> myPack = (s, id) => {
    myPackEx(s, s, id);
};
Action<string, string> myPackCore = (s, id) => {
    myPackEx(s, s+".Core", id);
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

Action<Dictionary<string, string>, string, string> setPackageVersions = (p, csproj, packagesConfig) => {
    if (csproj != null) {
		var csprojElement = loadCsProj(csproj);
		foreach (var itemGroup in csprojElement.Descendants("ItemGroup")) {
			var condition = itemGroup.Attribute("Condition");
			var target = "*";
			if (condition != null && !string.IsNullOrEmpty(condition.Value)) {
				const string tf = "'$(TargetFramework)' == '";
				var idx = condition.Value.IndexOf(tf);
				if (idx >= 0) {
					var end = condition.Value.IndexOf("'", idx + tf.Length);
					if (end >= 0) {
						target = condition.Value.Substring(idx + + tf.Length, end - idx - + tf.Length);
					}
				}
			}
					
			foreach (var packageReference in itemGroup.Descendants("PackageReference")) {
				p[target + ':' + packageReference.Attribute("Include").Value] = packageReference.Attribute("Version").Value;
			}
		}
    }

	if (packagesConfig != null)
		foreach (var x in parsePackageVersions(packagesConfig))
			p[x.Item3 + ':' + x.Item1] = x.Item2;
};

Task("Clean")
    .Does(() =>
{
    CleanDirectories("./.nupkg");
    CreateDirectory("./.nupkg");
    CleanDirectories("./Serenity.*.Net45/**/bin/" + configuration);
    CleanDirectories("./Serenity.*/**/bin/" + configuration);
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(context =>
{
    NuGetRestore("./Serenity.Net45.sln");
    NuGetRestore("./Serenity.DotNet.sln");
    NuGetRestore("./Serenity.DotNet.Core.sln");
});

Task("Compile")
    .IsDependentOn("Restore")
    .Does(context => 
{
    MSBuild("./Serenity.Net45.sln", s => {
        s.SetConfiguration(configuration);
    });
    
    var vi = System.Diagnostics.FileVersionInfo.GetVersionInfo("./Serenity.Core/bin/" + configuration + "/Serenity.Core.dll");
    serenityVersion = vi.FileMajorPart + "." + vi.FileMinorPart + "." + vi.FileBuildPart;   
    
    foreach (var project in dotnetBuildOrder) 
    {
        var csproj = @"./" + project + "/" + project + ".csproj";
        patchProjectVer(csproj, serenityVersion);
    }
    foreach (var project in dotnetBuildOrder) 
    {
        var csproj = @"./" + project + "/" + project + ".Core.csproj";
        patchProjectVer(csproj, serenityVersion);
    }

	var dotnetSln = @"./Serenity.DotNet.sln";
	writeHeader("dotnet restore " + dotnetSln);
	var exitCode = StartProcess("dotnet", "restore " + dotnetSln);
	if (exitCode > 0)
		throw new Exception("Error while restoring " + dotnetSln);
	writeHeader("dotnet build " + dotnetSln);
	exitCode = StartProcess("dotnet", "build " + dotnetSln + " -c " + configuration);
	if (exitCode > 0)
		throw new Exception("Error while building " + dotnetSln);


	dotnetSln = @"./Serenity.DotNet.Core.sln";
	writeHeader("dotnet restore " + dotnetSln);
	exitCode = StartProcess("dotnet", "restore " + dotnetSln);
	if (exitCode > 0)
		throw new Exception("Error while restoring " + dotnetSln);
	writeHeader("dotnet build " + dotnetSln);
	exitCode = StartProcess("dotnet", "build " + dotnetSln + " -c " + configuration);
	if (exitCode > 0)
		throw new Exception("Error while building " + dotnetSln);
});

Task("Test")
    .IsDependentOn("Compile")
    .Does(() =>
{
    XUnit2("./Serenity.Test*/**/bin/" + configuration + "/*.Test.dll");
});

Task("PdbPatch")
    .IsDependentOn("Test")
    .Does(() =>
{
    if ((target ?? "").ToLowerInvariant() == "push" ||
        (target ?? "").ToLowerInvariant() == "pdbpatch")
        runGitLink();
});

Task("Pack")
    .IsDependentOn("PdbPatch")
    .Does(() =>
{   
    myPack("Serenity.Core", null);
    myPack("Serenity.Caching.Couchbase", null);
    myPack("Serenity.Caching.Redis", null);
    myPack("Serenity.Data", null);
    myPack("Serenity.Data.Entity", null);
    myPack("Serenity.Services", null);
    myPack("Serenity.Testing", null);
    myPack("Serenity.Script.UI", "Serenity.Script");
    myPack("Serenity.Web", null);
    myPack("Serenity.CodeGenerator", null);
    

    myPackCore("Serenity.Core", null);
    myPackCore("Serenity.Caching.Couchbase", null);
    myPackCore("Serenity.Caching.Redis", null);
    myPackCore("Serenity.Data", null);
    myPackCore("Serenity.Data.Entity", null);
    myPackCore("Serenity.Services", null);
    // myPack("Serenity.Testing", null);
    // myPack("Serenity.Script.UI", "Serenity.Script");
    myPackCore("Serenity.Web", null);
    myPackCore("Serenity.CodeGenerator", null);

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
        myPack("Serenity.Web", "Serenity.Web.Assets");
        fixNugetCache();
    });

Task("Assets-Push")
    .IsDependentOn("Assets-Pack")
    .Does(() =>
    {
        myPush();
    });

Task("Tooling-Pack")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        myPack("Serenity.Web", "Serenity.Web.Tooling");
        fixNugetCache();
    });

Task("Tooling-Push")
    .IsDependentOn("Tooling-Pack")
    .Does(() =>
    {
        myPush();
    });
    
RunTarget(target);