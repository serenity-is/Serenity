var target = Argument("target", "NuGet");
var configuration = Argument("configuration", "Release");
var serenityVersion = Argument("Version", (string)null);

Task("Clean")
    .Does(() =>
{
    CleanDirectories("./Build");
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
      File.WriteAllText("./Build/" + s + ".nuspec", 
          File.ReadAllText("./" + s + "/" + s + ".nuspec")
              .Replace("${Version}", serenityVersion));
              
      NuGetPack("./Build/" + s + ".nuspec", new NuGetPackSettings {
          BasePath = "./" + s + "/bin/" + configuration,
          OutputDirectory = "./Build",
          NoPackageAnalysis = true
      });
    };
    
    myPack("Serenity.Core");
    myPack("Serenity.Data");
    myPack("Serenity.Data.Entity");
});

RunTarget(target);