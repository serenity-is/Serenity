using System.Diagnostics;

namespace Serenity.CodeGenerator;

public partial class ProjectFileInfoTests
{
    const string msbuildJson = """
        {
          "Properties": {
            "AssemblyName": "FromMSBuild",
            "ESMAssetBasePath": "Modules/",
            "Nullable": "enable",
            "OutDir": "bin/Debug/net8.0/",
            "RootNamespace": "RootFromMSBuild",
            "TargetFramework": "net8.0"
          },
          "Items": {
            "Using": [
              { "Identity": "System", "Static": "true" },
              { "Identity": "My.Namespace", "Alias": "MyNs" },
              { "Identity": "", "Static": "false" }
            ]
          }
        }
        """;

    [Fact]
    public void Reads_Properties_From_MSBuild_Output()
    {
        var info = Create(out _, executeMSBuild: _ => msbuildJson);

        Assert.Equal("FromMSBuild", info.GetAssemblyName());
        Assert.Equal("Modules/", info.GetEsmAssetBasePath());
        Assert.Equal("enable", info.GetNullable());
        Assert.Equal("RootFromMSBuild", info.GetRootNamespace());
        Assert.Equal("net8.0", info.GetTargetFramework());
        Assert.EndsWith("bin/Debug/net8.0/", info.GetOutDir().Replace('\\', '/'));
    }

    [Fact]
    public void Executes_MSBuild_With_Expected_Arguments()
    {
        ProcessStartInfo captured = null;
        var info = Create(out _, executeMSBuild: psi =>
        {
            captured = psi;
            return msbuildJson;
        });

        info.GetAssemblyName();

        Assert.NotNull(captured);
        Assert.Equal("dotnet", captured.FileName);
        Assert.Contains("msbuild", captured.Arguments);
        Assert.Contains(projectFile, captured.Arguments);
        Assert.Contains("-getItem:Using", captured.Arguments);
        Assert.Contains("-getProperty:AssemblyName", captured.Arguments);
        Assert.Contains("-getProperty:ESMAssetBasePath", captured.Arguments);
        Assert.Contains("-getProperty:Nullable", captured.Arguments);
        Assert.Contains("-getProperty:OutDir", captured.Arguments);
        Assert.Contains("-getProperty:RootNamespace", captured.Arguments);
        Assert.Contains("-getProperty:TargetFramework", captured.Arguments);
        Assert.DoesNotContain("-property:Configuration=", captured.Arguments);
    }

    [Fact]
    public void Passes_Configuration_When_Provided()
    {
        ProcessStartInfo captured = null;
        var info = Create(out _,
            getProperty: name => name == "Configuration" ? "Release" : null,
            executeMSBuild: psi =>
            {
                captured = psi;
                return msbuildJson;
            });

        info.GetAssemblyName();

        Assert.NotNull(captured);
        Assert.Contains("-property:Configuration=Release", captured.Arguments);
    }

    [Fact]
    public void Reads_GlobalUsings_From_MSBuild_Items()
    {
        var info = Create(out _, executeMSBuild: _ => msbuildJson);

        var usings = info.GetGlobalUsings();

        Assert.Equal(2, usings.Count);
        Assert.True(usings.ContainsKey("System"));
        Assert.Null(usings["System"]);
        Assert.Equal("MyNs", usings["My.Namespace"]);
    }

    [Fact]
    public void Reads_GlobalUsings_From_Property_Argument()
    {
        var info = Create(out _, getProperty: name =>
            name == "GlobalUsings" ? "Foo=Bar;Baz;Quux=Alias" : null);

        var usings = info.GetGlobalUsings();

        Assert.Equal(3, usings.Count);
        Assert.Equal("Bar", usings["Foo"]);
        Assert.Null(usings["Baz"]);
        Assert.Equal("Alias", usings["Quux"]);
    }

    [Fact]
    public void Reports_Error_For_Unexpected_MSBuild_Output()
    {
        var errors = new List<string>();
        var info = Create(out _, onError: errors.Add, executeMSBuild: _ => "not json");

        info.GetAssemblyName();

        Assert.Contains(errors, e => e.Contains("Unexpected output from MSBuild"));
    }

    [Fact]
    public void Reports_Error_When_MSBuild_Throws()
    {
        var errors = new List<string>();
        var info = Create(out _, onError: errors.Add,
            executeMSBuild: _ => throw new InvalidOperationException("boom"));

        info.GetAssemblyName();

        Assert.Contains(errors, e => e.Contains("Error while executing MSBuild") && e.Contains("boom"));
    }

    [Fact]
    public void Does_Not_Execute_MSBuild_When_Project_File_Is_Missing()
    {
        var executed = false;
        var info = Create(out _, createProjectFile: false, executeMSBuild: _ =>
        {
            executed = true;
            return msbuildJson;
        });

        var usings = info.GetGlobalUsings();

        Assert.False(executed);
        Assert.Empty(usings);
    }
}
