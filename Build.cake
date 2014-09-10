var target = Argument("target", "NuGet");
var configuration = Argument("configuration", "Release");

string serenityVersion = null;

var nuspecParams = new Dictionary<string, string> {
  { "authors", "Volkan Ceylan" },
  { "owners", "Volkan Ceylan" },
  { "language", "en-US" },
  { "iconUrl", "https://raw.github.com/volkanceylan/Serenity/master/Tools/Images/serenity-icon.png" },
  { "licenceUrl", "https://raw.github.com/volkanceylan/Serenity/master/LICENSE.md" },
  { "projectUrl", "http://github.com/volkanceylan/Serenity" },
  { "copyright", "Copyright (c) Volkan Ceylan" },
  { "tags", "Serenity" },
  { "framework", "net45" }
};

var nugetPackages = new List<string>();

Func<string, System.Xml.XmlDocument> loadXml = path => 
{
    var xml = new System.Xml.XmlDocument();
    xml.LoadXml(File.ReadAllText(path));
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
    
Task("Clean")
    .Does(() =>
{
    CleanDirectories("./Bin");
    CreateDirectory("./Bin");
    CreateDirectory("./Bin/Packages");
    CreateDirectory("./Bin/Temp");
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
    MSBuild("./Serenity.sln", s => {
        s.SetConfiguration(configuration);
    });
    
    minimizeJs("./Serenity.Script.Core/bin/" + configuration + "/Serenity.Script.Core.js");
    minimizeJs("./Serenity.Script.UI/bin/" + configuration + "/Serenity.Script.UI.js");
    
    var vi = System.Diagnostics.FileVersionInfo.GetVersionInfo("./Serenity.Core/bin/" + configuration + "/Serenity.Core.dll");
    serenityVersion = vi.FileMajorPart + "." + vi.FileMinorPart + "." + vi.FileBuildPart;   
});

Task("Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    XUnit("./Serenity.Test*/**/bin/" + configuration + "/*.Test.dll");
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
});

Task("NuGet")
    .IsDependentOn("Pack")
    .Does(() =>
{
    nuspecParams["jsonNetVersion"] = getPackageVersion("Serenity.Core", "Newtonsoft.Json");
    nuspecParams["couchbaseNetClientVersion"] = getPackageVersion("Serenity.Caching.Couchbase", "CouchbaseNetClient");
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

    Action<string> myPack = s => {
        var nuspec = File.ReadAllText("./" + s + "/" + s + ".nuspec");
      
        nuspec = nuspec.Replace("${version}", serenityVersion);
        nuspec = nuspec.Replace("${id}", s);
        
        foreach (var p in nuspecParams)
            nuspec = nuspec.Replace("${" + p.Key + "}", p.Value);
          
        var assembly = "./" + s + "/bin/" + configuration + "/" + s + ".dll";
        if (!File.Exists(assembly))
            assembly = "./" + s + "/bin/" + configuration + "/" + s + ".exe";
          
        if (File.Exists(assembly)) 
        {
            var vi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly);
            nuspec = nuspec.Replace("${title}", vi.FileDescription);
            nuspec = nuspec.Replace("${description}", vi.Comments);
        }
      
        File.WriteAllText("./Bin/Temp/" + s + ".temp.nuspec", nuspec);
       
        NuGetPack("./Bin/Temp/" + s + ".temp.nuspec", new NuGetPackSettings {
            BasePath = "./" + s + "/bin/" + configuration,
            OutputDirectory = "./Bin/Packages",
            NoPackageAnalysis = true
        });
        
        nugetPackages.Add("./Bin/Packages/" + s + "." + serenityVersion + ".nupkg");
    };
    
    myPack("Serenity.Core");
    myPack("Serenity.Munq");
    myPack("Serenity.Caching.Web");
    myPack("Serenity.Caching.Couchbase");
    myPack("Serenity.Data");
    myPack("Serenity.Data.Entity");
    myPack("Serenity.Data.Filtering");
    myPack("Serenity.Services");
    myPack("Serenity.Services.Mvc");
    myPack("Serenity.Reporting");
    myPack("Serenity.Web");
    myPack("Serenity.CodeGeneration");
    myPack("Serenity.CodeGeneration.Mvc");
    myPack("Serenity.Testing");
    
    myPack("Serenity.Script.Imports");
    myPack("Serenity.Script.Core");
    myPack("Serenity.Script.UI");
    
    myPack("Serenity.CodeGenerator");
});

Task("NuGet-Push")
  .IsDependentOn("NuGet")
  .Does(() => 
  {
      foreach (var package in nugetPackages)
      {
          NuGetPush(package, new NuGetPushSettings {
          });
      }
  });

RunTarget(target);