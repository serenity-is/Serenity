var target = Argument("target", "NuGet");
var configuration = Argument("configuration", "Release");

string serenityVersion = null;
string jsonNetVersion = null;

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
    
    var vi = System.Diagnostics.FileVersionInfo.GetVersionInfo("./Serenity.Core/bin/" + configuration + "/Serenity.Core.dll");
    serenityVersion = vi.FileMajorPart + "." + vi.FileMinorPart + "." + vi.FileBuildPart;
    
    vi = System.Diagnostics.FileVersionInfo.GetVersionInfo("./Serenity.Core/bin/" + configuration + "/Newtonsoft.Json.dll");
    jsonNetVersion = vi.FileMajorPart + "." + vi.FileMinorPart + "." + vi.FileBuildPart;
    
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
    Action<string> myPack = s => {
        var nuspec = File.ReadAllText("./" + s + "/" + s + ".nuspec");
      
        nuspec = nuspec.Replace("${version}", serenityVersion);
        nuspec = nuspec.Replace("${jsonNetVersion}", jsonNetVersion);
        nuspec = nuspec.Replace("${id}", s);
        
        foreach (var p in nuspecParams)
            nuspec = nuspec.Replace("${" + p.Key + "}", p.Value);
          
        var assembly = "./" + s + "/bin/" + configuration + "/" + s + ".dll";
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
    myPack("Serenity.Reporting");
    myPack("Serenity.Web");
    myPack("Serenity.CodeGeneration");
    myPack("Serenity.Testing");
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