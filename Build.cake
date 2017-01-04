#addin "nuget:https://www.nuget.org/api/v2?package=Newtonsoft.Json&version=9.0.1"

using System.Xml.Linq;

var target = Argument("target", "NuGet");
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

var nugetPackages = new List<string>();

Func<string, Newtonsoft.Json.Linq.JObject> loadJson = path => {
    var content = System.IO.File.ReadAllText(path, Encoding.UTF8);
    return Newtonsoft.Json.Linq.JObject.Parse(content);
};

Action<string, string> patchProjectVer = (projectjson, version) => {
    var node = loadJson(projectjson);
    if (node["version"].ToString() != "version" + "-*")
    {
        node["version"].Replace(version + "-*");
        System.IO.File.WriteAllText(projectjson, node.ToString(), Encoding.UTF8);
    }
};

Func<string, System.Xml.XmlDocument> loadXml = path => 
{
    var xml = new System.Xml.XmlDocument();
    xml.LoadXml(System.IO.File.ReadAllText(path));
    return xml;
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
    
Task("Clean")
    .Does(() =>
{
    CleanDirectories("./.nupkg");
    CreateDirectory("./.nupkg");
    CreateDirectory("./.nupkg/.temp");
    CleanDirectories("./Serenity.*.Net45/**/bin/" + configuration);
    CleanDirectories("./Serenity.*/**/bin/" + configuration);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(context =>
{
    NuGetRestore("./Serenity.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(context => 
{
    MSBuild("./Serenity.Net45.sln", s => {
        s.SetConfiguration(configuration);
    });
    
    var vi = System.Diagnostics.FileVersionInfo.GetVersionInfo("./Serenity.Core.Net45/bin/" + configuration + "/Serenity.Core.dll");
    serenityVersion = vi.FileMajorPart + "." + vi.FileMinorPart + "." + vi.FileBuildPart;   
    
    DotNetCoreBuild("./**/project.json", new DotNetCoreBuildSettings 
    {
        Configuration = configuration,
        NoIncremental = true
    });
    
    foreach (var projectjson in System.IO.Directory.GetFiles(".\\", "project.json", System.IO.SearchOption.AllDirectories)) 
    {
        var fn = System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(projectjson));
        if (fn.StartsWith("Serenity.", StringComparison.OrdinalIgnoreCase))
            patchProjectVer(projectjson, serenityVersion);
    }
});

Task("Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    XUnit2("./Serenity.Test*/**/bin/" + configuration + "/*.Test.dll");
});

Task("Copy-Files")
    .IsDependentOn("Unit-Tests")
    .Does(() =>
{
});

Task("Pack")
    .IsDependentOn("Copy-Files")
    .Does(() =>
{
    if ((target ?? "").ToLowerInvariant() == "nuget-push")
        runGitLink();
});

Func<string, string> getVersionFromNuspec = (filename) => {
    var nuspec = System.IO.File.ReadAllText(filename);
    var xml = XElement.Parse(nuspec);
    XNamespace ns = "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd";
    return xml.Descendants(ns + "version").First().Value;
};

Action<string, string> myPack = (s, id) => {
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
    
    foreach (var p in nuspecParams)
        nuspec = nuspec.Replace("${" + p.Key + "}", p.Value);
      
    var assembly = "./" + s + "/bin/" + configuration + "/" + s + ".dll";
    if (!System.IO.File.Exists(assembly))
        assembly = "./" + s + "/bin/" + configuration + "/" + s + ".exe";
      
    if (System.IO.File.Exists(assembly)) 
    {
        var vi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly);
        nuspec = nuspec.Replace("${title}", vi.FileDescription);
        nuspec = nuspec.Replace("${description}", vi.Comments);
    }
  
    System.IO.File.WriteAllText("./nupkg/.temp/" + s + ".temp.nuspec", nuspec);
   
    NuGetPack("./nupkg/.temp/" + s + ".temp.nuspec", new NuGetPackSettings {
        BasePath = "./" + s + "/bin/" + configuration,
        OutputDirectory = "./nupkg",
        NoPackageAnalysis = true
    });
    
    nugetPackages.Add("./nupkg/" + (id ?? s) + "." + version + ".nupkg");
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

Action setPackageVersions = delegate() {
    nuspecParams["jsonNetVersion"] = getPackageVersion("Serenity.Core", "Newtonsoft.Json");
    nuspecParams["couchbaseNetClientVersion"] = getPackageVersion("Serenity.Caching.Couchbase", "CouchbaseNetClient");
    nuspecParams["stackExchangeRedisVersion"] = getPackageVersion("Serenity.Caching.Redis", "StackExchange.Redis");
    nuspecParams["microsoftAspNetMvcVersion"] = getPackageVersion("Serenity.Web", "Microsoft.AspNet.Mvc");
    nuspecParams["microsoftAspNetRazorVersion"] = getPackageVersion("Serenity.Web", "Microsoft.AspNet.Razor");
    nuspecParams["microsoftAspNetWebPagesVersion"] = getPackageVersion("Serenity.Web", "Microsoft.AspNet.WebPages");
    nuspecParams["microsoftWebInfrastructureVersion"] = getPackageVersion("Serenity.Web", "Microsoft.Web.Infrastructure");
    nuspecParams["saltarelleCompilerVersion"] = getPackageVersion("Serenity.Script.Imports", "Saltarelle.Compiler");
    nuspecParams["saltarellejQueryVersion"] = getPackageVersion("Serenity.Script.Imports", "Saltarelle.jQuery");
    nuspecParams["saltarellejQueryUIVersion"] = getPackageVersion("Serenity.Script.Imports", "Saltarelle.jQuery.UI");
    nuspecParams["saltarelleLinqVersion"] = getPackageVersion("Serenity.Script.Imports", "Saltarelle.Linq");
    nuspecParams["saltarelleRuntimeVersion"] = getPackageVersion("Serenity.Script.Imports", "Saltarelle.Runtime");
    nuspecParams["saltarelleWebVersion"] = getPackageVersion("Serenity.Script.Imports", "Saltarelle.Web");
    nuspecParams["scriptFramework"] = loadXml(@".\Serenity.Script.Imports\packages.config").SelectSingleNode("//package[@id='Saltarelle.Runtime']/@targetFramework").Value; 
    nuspecParams["serenityWebAssetsVersion"] = getVersionFromNuspec(@".\Serenity.Web\Serenity.Web.Assets.nuspec");
    nuspecParams["serenityWebToolingVersion"] = getVersionFromNuspec(@".\Serenity.Web\Serenity.Web.Tooling.nuspec");
    nuspecParams["jQueryVersion"] = getPackageVersion("Serenity.Test", "jQuery");
    nuspecParams["jQueryUIVersion"] = getPackageVersion("Serenity.Test", "jQuery.UI.Combined");
    nuspecParams["validationVersion"] = getPackageVersion("Serenity.Test", "jQuery.Validation");
    nuspecParams["msieEngineVersion"] = getPackageVersion("Serenity.Test", "MsieJavaScriptEngine");
    nuspecParams["bootstrapVersion"] = getPackageVersion("Serenity.Test", "bootstrap");
    nuspecParams["toastrVersion"] = getPackageVersion("Serenity.Test", "toastr");
    nuspecParams["blockUITSVersion"] = getPackageVersion("Serenity.Web", "jquery.blockUI.TypeScript.DefinitelyTyped");
    nuspecParams["cookieTSVersion"] = getPackageVersion("Serenity.Web", "jquery.cookie.TypeScript.DefinitelyTyped");
    nuspecParams["jQueryTSVersion"] = getPackageVersion("Serenity.Web", "jquery.TypeScript.DefinitelyTyped");
    nuspecParams["jQueryUITSVersion"] = getPackageVersion("Serenity.Web", "jqueryui.TypeScript.DefinitelyTyped");
    nuspecParams["validationTSVersion"] = getPackageVersion("Serenity.Web", "jquery.validation.TypeScript.DefinitelyTyped");
    nuspecParams["sortableTSVersion"] = getPackageVersion("Serenity.Web", "sortablejs.TypeScript.DefinitelyTyped");
    nuspecParams["toastrTSVersion"] = getPackageVersion("Serenity.Web", "toastr.TypeScript.DefinitelyTyped");
};

Task("NuGet")
    .IsDependentOn("Pack")
    .Does(() =>
{
    setPackageVersions();
    
    myPack("Serenity.Core", null);
    //myPack("Serenity.Caching.Couchbase", null);
    //myPack("Serenity.Caching.Redis", null);
    //myPack("Serenity.Data", null);
    //myPack("Serenity.Data.Entity", null);
    //myPack("Serenity.Services", null);
    //myPack("Serenity.Testing", null);
    
    //myPack("Serenity.Script.UI", "Serenity.Script");

    //myPack("Serenity.Web", null);
    //myPack("Serenity.CodeGenerator", null);
    
    fixNugetCache();
});

Task("NuGet-Push")
    .IsDependentOn("NuGet")
    .Does(() => 
    {
        myPush();
    });
 
Task("Assets")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        myPack("Serenity.Web", "Serenity.Web.Assets");
        fixNugetCache();
    });

Task("Assets-Push")
    .IsDependentOn("Assets")
    .Does(() =>
    {
        myPush();
    });

Task("Tooling")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        myPack("Serenity.Web", "Serenity.Web.Tooling");
        fixNugetCache();
    });

Task("Tooling-Push")
    .IsDependentOn("Tooling")
    .Does(() =>
    {
        myPush();
    });
    
RunTarget(target);