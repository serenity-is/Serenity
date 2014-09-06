var target = Argument("target", "NuGet");
var configuration = Argument("configuration", "Release");

Task("Clean")
    .Does(() =>
{
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
    NuGetPack("./Serenity.Core/Serenity.Core.nuspec", new NuGetPackSettings {
        Version = "0.1.0",
        BasePath = "./Serenity.Core/bin/" + configuration,
        OutputDirectory = "./Build",
        NoPackageAnalysis = true
    });
});

RunTarget(target);