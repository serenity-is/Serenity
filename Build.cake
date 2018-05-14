#addin "nuget:https://www.nuget.org/api/v2?package=Newtonsoft.Json&version=9.0.1"

using System.Xml.Linq;
using Newtonsoft.Json.Linq;

var target = Argument("target", "Pack");
if (target == "")
	target = "Pack";
	
var configuration = Argument("configuration", "Release");
var msBuildPath = @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe";
if (!System.IO.File.Exists(msBuildPath))
	msBuildPath = null;

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

Action runSourceLink = () => {

    MSBuild(@".\Serenity.Web\Serenity.Web.Net45.csproj", s => {
        s.SetConfiguration(configuration);
		s.ToolPath = msBuildPath;
        s.WithProperty("SourceLinkCreate", "true");
    });

    MSBuild(@".\Serenity.Testing\Serenity.Testing.Net45.csproj", s => {
        s.SetConfiguration(configuration);
		s.ToolPath = msBuildPath;
        s.WithProperty("SourceLinkCreate", "true");
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

Action<string, string, string> myPack = (s, id, project) => {
    var prm = new Dictionary<string, string>(nuspecParams);

    var packagesConfig = "./" + s + ".Net45/packages.config";
    if (!System.IO.File.Exists(packagesConfig))
        packagesConfig = "./" + s + "/packages." + s + ".Net45.config";
    if (!System.IO.File.Exists(packagesConfig))
        packagesConfig = "./" + s + "/packages.config";
    if (!System.IO.File.Exists(packagesConfig))
		packagesConfig = null;
        
	id = id ?? s;
	project = project ?? s;
    var csproj = "./" + s + "/" + project + ".csproj";
    if (!System.IO.File.Exists(csproj))
        csproj = null;

    if (id == "Serenity.Web" || id == "Serenity.Web.AspNetCore")
        setPackageVersions(prm, null, "./Serenity.Test/packages.Serenity.Test.Net45.config");
        
    setPackageVersions(prm, csproj, packagesConfig);
    
    var filename = "./" + s + "/" + id + ".nuspec";
    string version;
    if (!System.IO.File.Exists(filename) &&
        csproj != null) {
        writeHeader("dotnet pack " + csproj);
        var exitCode = StartProcess("dotnet", "pack " + csproj + " -c:" + configuration + " -o:../.nupkg/");
        if (exitCode > 0)
            throw new Exception("Error while packing " + csproj);
        version = serenityVersion;
    }
    else {        
        var nuspec = System.IO.File.ReadAllText(filename);
      
        if (nuspec.IndexOf("<version>${version}</version>") < 0) {
            version = getVersionFromNuspec(filename);
        }
        else {
            version = serenityVersion;
            nuspec = nuspec.Replace("${version}", version);
        }
        nuspec = nuspec.Replace("${id}", id);
        
        foreach (var p in prm)
            nuspec = nuspec.Replace("${" + p.Key + "}", p.Value);
          
        var assembly = "./" + s + ".Net45/bin/" + configuration + "/" + project + ".dll";
        if (!System.IO.File.Exists(assembly))
            assembly = "./" + s + ".Net45/bin/" + configuration + "/" + project + ".exe";
        if (!System.IO.File.Exists(assembly))
            assembly = "./" + s + "/bin/" + configuration + "/" + project + ".dll";
        if (!System.IO.File.Exists(assembly))
            assembly = "./" + s + "/bin/" + configuration + "/" + project + ".exe";
          
        if (System.IO.File.Exists(assembly)) 
        {
            var vi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly);
            nuspec = nuspec.Replace("${title}", vi.FileDescription);
            nuspec = nuspec.Replace("${description}", vi.Comments);
        }

        var temp = "./.nupkg/" + id + ".temp.nuspec";
        System.IO.File.WriteAllText(temp, nuspec);
         
        var basePath = @"./" + s;
         
        NuGetPack(temp, new NuGetPackSettings {
            BasePath = basePath,
            OutputDirectory = "./.nupkg",
            NoPackageAnalysis = true
        });
        
        System.IO.File.Delete(temp);
    }
    var pkg = "./.nupkg/" + id + "." + version + ".nupkg";
    if (!System.IO.File.Exists(pkg))
        throw new Exception("Package " + pkg + " is not found!");
    nugetPackages.Add(pkg);
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
	var dotnetSln = @"./Serenity.sln";
	writeHeader("dotnet restore " + dotnetSln);
	var exitCode = StartProcess("dotnet", "restore " + dotnetSln);
	if (exitCode > 0)
		throw new Exception("Error while restoring " + dotnetSln);
});

Task("Compile")
    .IsDependentOn("Restore")
    .Does(context => 
{

    var sasmi = System.IO.File.ReadAllText(@".\SharedAssemblyInfo.cs");
    var sasmi1 = sasmi.IndexOf("AssemblyVersion(\"");
    serenityVersion = sasmi.Substring(sasmi1 + "AssemblyVersion(\"".Length).Split('"')[0];
               
	patchProjectVer(@"./SharedProperties.xml", serenityVersion);

	writeHeader("Building Serenity.sln");
    MSBuild("./Serenity.sln", s => {
        s.SetConfiguration(configuration);
		s.ToolPath = msBuildPath;
    });
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
        runSourceLink();
});

Task("Pack")
    .IsDependentOn("PdbPatch")
    .Does(() =>
{   
    myPack("Serenity.Core", null, null);
    myPack("Serenity.Configuration", null, null);
    myPack("Serenity.Caching.Couchbase", null, null);
    myPack("Serenity.Caching.Redis", null, null);
    myPack("Serenity.Data", null, null);
    myPack("Serenity.Data.Entity", null, null);
    myPack("Serenity.Services", null, null);
    myPack("Serenity.Testing", null, null);
    myPack("Serenity.Web", null, null);
    myPack("Serenity.Web", "Serenity.Web.AspNetCore", "Serenity.Web");
    myPack("Serenity.CodeGenerator", null, null);
    
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
        myPack("Serenity.Web", "Serenity.Web.Assets", null);
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
        myPack("Serenity.Web", "Serenity.Web.Tooling", null);
        fixNugetCache();
    });

Task("Tooling-Push")
    .IsDependentOn("Tooling-Pack")
    .Does(() =>
    {
        myPush();
    });
	
Task("Script-Pack")
    .IsDependentOn("Clean")
    .Does(() =>
    {
		NuGetRestore("./Serenity.Script.UI/Serenity.Script.UI.sln");
		MSBuild("./Serenity.Script.UI/Serenity.Script.UI.sln", s => {
			s.SetConfiguration(configuration);
			s.ToolPath = msBuildPath;
		});

        myPack("Serenity.Script.UI", "Serenity.Script", null);
        fixNugetCache();
    });

Task("Script-Push")
    .IsDependentOn("Script-Pack")
    .Does(() =>
    {
        myPush();
    });
    
RunTarget(target);