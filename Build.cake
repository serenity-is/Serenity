#addin "nuget:https://www.nuget.org/api/v2?package=Newtonsoft.Json&version=9.0.1"

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

Action<string, string> patchProjectVer = (projectjson, version) => {
    var changed = false;
    var node = loadJson(projectjson);
    if (node["version"].ToString() != "version" + "-*")
    {
        node["version"].Replace(version + "-*");
        changed = true;
    }

    var deps = node["dependencies"] as JObject;
    if (deps != null) {
        foreach (var pair in (deps as IEnumerable<KeyValuePair<string, JToken>>).ToList()) {
            if (pair.Key.StartsWith("Serenity.") &&
                pair.Key != "Serenity.Web.Assets" &&
                pair.Key != "Serenity.Web.Tooling")
            {
                var v = pair.Value as JValue;
                if (v != null && (v.Value ?? "").ToString() != version + "-*")
                {
                    deps[pair.Key].Replace(version + "-*");
                    changed = true;
                }
            }
        }
    }

    if (changed)
        System.IO.File.WriteAllText(projectjson, node.ToString(), Encoding.UTF8);
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

Action<string, string> myPack = (s, id) => {
    var prm = new Dictionary<string, string>(nuspecParams);

    var packagesConfig = "./" + s + ".Net45/packages.config";
    if (!System.IO.File.Exists(packagesConfig))
        packagesConfig = "./" + s + "/packages.config";
        
    var projectJson = "./" + s + "/project.json";
    if (!System.IO.File.Exists(projectJson))
        projectJson = null;

    if (s == "Serenity.Web")
        setPackageVersions(prm, null, "./Serenity.Test/packages.config");
        
    setPackageVersions(prm, projectJson, packagesConfig);
    
    var filename = "./" + s + "/" + (id ?? s) + ".nuspec";
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
      
    var assembly = "./" + s + ".Net45/bin/" + configuration + "/" + s + ".dll";
    if (!System.IO.File.Exists(assembly))
        assembly = "./" + s + ".Net45/bin/" + configuration + "/" + s + ".exe";
	if (!System.IO.File.Exists(assembly))
		assembly = "./" + s + "/bin/" + configuration + "/" + s + ".dll";
    if (!System.IO.File.Exists(assembly))
        assembly = "./" + s + "/bin/" + configuration + "/" + s + ".exe";
      
    if (System.IO.File.Exists(assembly)) 
    {
        var vi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly);
        nuspec = nuspec.Replace("${title}", vi.FileDescription);
        nuspec = nuspec.Replace("${description}", vi.Comments);
    }

    var temp = "./.nupkg/" + s + ".temp.nuspec";
    System.IO.File.WriteAllText(temp, nuspec);
     
    var basePath = @"./" + s;
     
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

Action<Dictionary<string, string>, string, string> setPackageVersions = (p, projectJson, packagesConfig) => {
    if (projectJson != null) {
        var jv = loadJson(projectJson);
        addDeps(p, jv["dependencies"] as JObject, "*");
        var fworks = jv["frameworks"] as JObject;
        if (fworks != null)
            foreach (var pair in fworks)
            {
                var obj = pair.Value as JObject;
                if (obj != null)
                    addDeps(p, obj["dependencies"] as JObject, pair.Key);
            }
    }

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
    NuGetRestore("./Serenity.sln");
});

Task("Compile")
    .IsDependentOn("Restore")
    .Does(context => 
{
    MSBuild("./Serenity.Net45.sln", s => {
        s.SetConfiguration(configuration);
    });
    
    var vi = System.Diagnostics.FileVersionInfo.GetVersionInfo("./Serenity.Core.Net45/bin/" + configuration + "/Serenity.Core.dll");
    serenityVersion = vi.FileMajorPart + "." + vi.FileMinorPart + "." + vi.FileBuildPart;   
    
    foreach (var project in dotnetBuildOrder) 
    {
        var projectJson = @"./" + project + @"/project.json";
        patchProjectVer(projectJson, serenityVersion);
        writeHeader("dotnet restore " + projectJson);
        var exitCode = StartProcess("dotnet", "restore " + projectJson);
        if (exitCode > 0)
            throw new Exception("Error while restoring " + projectJson);
        writeHeader("dotnet build " + projectJson);
        exitCode = StartProcess("dotnet", "build " + projectJson + " -c " + configuration);
        if (exitCode > 0)
            throw new Exception("Error while building " + projectJson);
    }
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