using System.Xml.Linq;

var target = Argument("target", "PrepareVSIX");

Task("PrepareVSIX")
  .Does(() => 
{
    var r = System.IO.Path.GetFullPath(@"..\");
    var samplePackagesFolder = r + @"packages\";
    var vsixPackagesFolder = r + @"VSIX\packages\";
    var vsixProjFile = r + @"VSIX\BasicApplication.VSIX.csproj";
    var vsixManifestFile = r + @"VSIX\source.extension.vsixmanifest";
    var templateFolder = r + @"VSIX\obj\BasicApplication.Template";
    var sampleWebProj = r + @"BasicApplication\BasicApplication.Web\BasicApplication.Web.csproj";
    var sampleScriptProj = r + @"BasicApplication\BasicApplication.Script\BasicApplication.Script.csproj";

    Func<string, List<Tuple<string, string>>> parsePackages = path => {
        var xml = XElement.Parse(File.ReadAllText(path));
        var pkg = new List<Tuple<string, string>>();
        foreach (var x in xml.Descendants("package"))
            pkg.Add(new Tuple<string, string>(x.Attribute("id").Value, x.Attribute("version").Value));
        return pkg;
    };
    
    Func<string, List<Tuple<string, string>>> parseAndCopyPackages = path => {
        var list = parsePackages(path);
        foreach (var p in list)
        {
            var pkg = p.Item1 + "." + p.Item2;
            var pkgFile = pkg + ".nupkg";
            var src = System.IO.Path.Combine(System.IO.Path.Combine(samplePackagesFolder, pkg), pkgFile);
            var dst = System.IO.Path.Combine(System.IO.Path.Combine(vsixPackagesFolder, pkgFile));
            File.Copy(src, dst, overwrite: true);
        }
        return list;
    };
    
    Func<string, List<string>> getProjectFileList = (path) =>
    {
        XNamespace ns = "http://schemas.microsoft.com/developer/msbuild/2003";
        var xv = XElement.Parse(File.ReadAllText(path));
        var list = xv.Descendants(ns + "ItemGroup").Elements().Where(x => (
            x.Name == ns + "Content" ||
            x.Name == ns + "Compile" ||
            x.Name == ns + "Folder" ||
            x.Name == ns + "None")).Select(x => x.Attribute("Include").Value)
            .ToList();
            
        list.Sort(delegate(string x, string y) {
            var px = System.IO.Path.GetDirectoryName(x);
            var py = System.IO.Path.GetDirectoryName(y);
            if (string.Equals(px, py, StringComparison.OrdinalIgnoreCase))
            {
                if ((System.IO.Path.GetExtension(x) ?? "").ToLowerInvariant() == ".tt")
                    return -1;
                    
                if ((System.IO.Path.GetExtension(y) ?? "").ToLowerInvariant() == ".tt")
                    return 1;               
            }
            return x.CompareTo(y);
        });
        
        return list;
    };
    
    Action<List<Tuple<string, string>>, List<Tuple<string, string>>> updateVsixProj = (wp, sp) => {
        var hash = new HashSet<Tuple<string, string>>();
        foreach (var x in sp)
            hash.Add(x);
        foreach (var x in wp)
            hash.Add(x);
        var allPackages = new List<Tuple<string, string>>();
        allPackages.AddRange(hash);
        allPackages.Sort((x, y) => x.Item1.CompareTo(y.Item1));
    
        var xm = XElement.Parse(File.ReadAllText(vsixManifestFile));
        xm.Descendants(((XNamespace)"http://schemas.microsoft.com/developer/vsx-schema/2011") + "Identity")
            .First().SetAttributeValue("Version", allPackages.First(x => x.Item1.StartsWith("Serenity.Core")).Item2);
        File.WriteAllText(vsixManifestFile, xm.ToString(SaveOptions.OmitDuplicateNamespaces));
    
        var xv = XElement.Parse(File.ReadAllText(vsixProjFile));
        XNamespace ns = "http://schemas.microsoft.com/developer/msbuild/2003";
        var xp = xv.Descendants().Where(x => x.Name == ns + "Content" && x.Attribute("Include") != null &&
            x.Attribute("Include").Value.StartsWith(@"packages\"));
        var first = xp.First();
        var firstText = first.ToString();
        var itemGroup = first.Parent;
        xp.Remove();
        
        foreach (var p in allPackages)
        {
            var xu = XElement.Parse(firstText); 
            xu.Attribute("Include").SetValue(@"packages\" + p.Item1 + "." + p.Item2 + ".nupkg");
            itemGroup.Add(xu);
        }
        
        File.WriteAllText(vsixProjFile, xv.ToString(SaveOptions.OmitDuplicateNamespaces));  
    };

    Action<string> replaceParams = (path) => {
        var content = File.ReadAllText(path);
        if (content.IndexOf("BasicApplication") >= 0)
        {
            content = content.Replace(@"\BasicApplication", @"\$ext_projectname$");
            content = content.Replace(@"BasicApplication.Script\", @"$ext_projectname$.Script\");
            content = content.Replace(@"BasicApplication.Web\", @"$ext_projectname$.Web\");
            content = content.Replace(@"BasicApplication\", @"$ext_projectname$\");
            content = content.Replace("BasicApplication", "$ext_safeprojectname$");
            File.WriteAllText(path, content);
        }   
    };
    
    Action<string, List<Tuple<string, string>>> replaceTemplateFileList = (csproj, packages) => {
        var vsTemplate = System.IO.Path.ChangeExtension(csproj, ".vstemplate");
        var fileList = getProjectFileList(csproj);
        
        var xv = XElement.Parse(File.ReadAllText(vsTemplate));
        XNamespace ns = "http://schemas.microsoft.com/developer/vstemplate/2005";
        var project = xv.Descendants(ns + "Project").First();
        project.Elements().Remove();
        Dictionary<string, XElement> byFolder = new Dictionary<string, XElement>();
        
        var copySourceRoot = System.IO.Path.GetDirectoryName(csproj);
        var copyTargetRoot = System.IO.Path.Combine(templateFolder, System.IO.Path.GetFileNameWithoutExtension(csproj));
        
        foreach (var file in fileList)
        {
            var parts = file.Split(new char[] { '\\' });
            XElement folder = project;
            string f = "";
            for (var i = 0; i < parts.Length - 1; i++)
            {
                if (f.Length > 0)
                    f += "\\";
                f += parts[i];

                if (!byFolder.ContainsKey(f))
                {
                    var newFolder = new XElement(ns + "Folder");
                    newFolder.SetAttributeValue("Name", parts[i]);
                    newFolder.SetAttributeValue("TargetFolderName", parts[i]);
                    folder.Add(newFolder);
                    byFolder[f] = newFolder;
                    folder = newFolder;
                }
                else 
                    folder = byFolder[f];
            }
            
            if (file.EndsWith(@"\"))
            {
                continue;
            }
            
            var item = new XElement(ns + "ProjectItem");
            var extension = (System.IO.Path.GetExtension(file) ?? "").ToLowerInvariant();
            bool replaceParameters = extension == ".cs" ||
                extension == ".config" ||
                extension == ".tt" ||
                extension == ".css" ||
                extension == ".map" ||
                extension == ".less" ||
                extension == ".csproj" ||
                extension == ".sql" ||
                extension == ".ttinclude" ||
                extension == ".txt" ||
                extension == ".js" ||
                extension == ".json" ||
                extension == ".asax" ||
                extension == ".cshtml" ||
                extension == ".html";
            
            item.SetAttributeValue("ReplaceParameters", replaceParameters ? "true" : "false");
            item.SetAttributeValue("TargetFileName", parts[parts.Length - 1].Replace("BasicApplication", "$ext_projectname$"));
            item.SetValue(parts[parts.Length - 1]);
            folder.Add(item);
            
            var targetFile = System.IO.Path.Combine(copyTargetRoot, file);
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(targetFile));
            File.Copy(System.IO.Path.Combine(copySourceRoot, file), targetFile);
            
            if (replaceParameters) {
                replaceParams(targetFile);
            }
        }
        
        var pkg = xv.Descendants(ns + "packages").Single();
        pkg.Elements().Remove();
        foreach (var p in packages)
        {
            var pk = new XElement(ns + "package");
            pk.SetAttributeValue("id", p.Item1);
            pk.SetAttributeValue("version", p.Item2);
            pkg.Add(pk);
        }
        
        File.WriteAllText(vsTemplate, xv.ToString(SaveOptions.OmitDuplicateNamespaces));
        File.Copy(vsTemplate, System.IO.Path.Combine(copyTargetRoot, System.IO.Path.GetFileName(vsTemplate)));
        File.Copy(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(vsTemplate), "__TemplateIcon.png"), 
            System.IO.Path.Combine(copyTargetRoot, "__TemplateIcon.png"));
        var targetProj = System.IO.Path.Combine(copyTargetRoot, System.IO.Path.GetFileName(csproj));
        File.WriteAllText(targetProj, File.ReadAllText(csproj)
            .Replace("http://localhost:55555/", "")
            .Replace("<DevelopmentServerPort>55556</DevelopmentServerPort>", "<DevelopmentServerPort></DevelopmentServerPort>"));
        replaceParams(targetProj);
    };

    foreach (var file in Directory.GetFiles(vsixPackagesFolder, "*.nupkg"))
        File.Delete(file);
    
    var webPackages = parseAndCopyPackages(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(sampleWebProj), "packages.config"));  
    var scriptPackages = parseAndCopyPackages(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(sampleScriptProj), "packages.config"));
    updateVsixProj(webPackages, scriptPackages);
    
    if (Directory.Exists(templateFolder)) 
        Directory.Delete(templateFolder, true);
        
    Directory.CreateDirectory(templateFolder);
    Directory.CreateDirectory(System.IO.Path.Combine(templateFolder, "BasicApplication.Web"));
    Directory.CreateDirectory(System.IO.Path.Combine(templateFolder, "BasicApplication.Script"));
    
    
    replaceTemplateFileList(sampleScriptProj, scriptPackages);
    replaceTemplateFileList(sampleWebProj, webPackages);
    File.Copy(r + @"BasicApplication\__TemplateIcon.png", 
        System.IO.Path.Combine(templateFolder, "__TemplateIcon.png")); 
    File.Copy(r + @"BasicApplication\BasicApplication.vstemplate", 
        System.IO.Path.Combine(templateFolder, "BasicApplication.vstemplate")); 
        
    CleanDirectory("./ProjectTemplates");
    CleanDirectory("./bin/Debug");
    CleanDirectory("./bin/Release");
    Zip(templateFolder, r + @"VSIX\ProjectTemplates\BasicApplication.Template.zip");
});

RunTarget(target);