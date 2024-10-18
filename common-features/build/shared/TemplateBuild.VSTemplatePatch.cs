#if IsTemplateBuild
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Build;

public static partial class Shared
{
    private static readonly GlobFilter ExcludeFilesGlob = new(new[] {
        "App_Data/",
        "bin/",
        "obj/",
        "node_modules/",
        "packages/",
        "PublishProfiles/",
        "TestResults/",
        ".git/",
        ".vs/",
        ".vscode/",
        "*.bak",
        "*.csproj",
        "*.dg",
        "*.DotSettings*",
        "*.log",
        "*.lock.json",
        "*.orig",
        "*.mdf",
        "*.sqlite",
        "*.suo",
        "*.user",
        "*.vstemplate",
        "*.xproj",
        "*.zip",
        ".syncache.sqlite",
        "_trigger.ts",
        "appsettings*.machine.json",
        "Thumbs.db",
        "ErrorLog.db",
        "StyleCop.Cache",
        "Serene.Web.js.map",
        "StartSharp.Web.js.map",
        "launchSettings.json",
        "**/wwwroot/esm/**/*.js.map",
        "*.tsbuildinfo"
    });

    private static readonly HashSet<string> ReplaceParamsInExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".cs" ,
        ".ts" ,
        ".d.ts" ,
        ".tsx" ,
        ".config" ,
        ".tt" ,
        ".css" ,
        ".map" ,
        ".less" ,
        ".csproj" ,
        ".sql" ,
        ".ttinclude" ,
        ".txt" ,
        ".js" ,
        ".json" ,
        ".asax" ,
        ".cshtml" ,
        ".html"
    };

    private static void ConvertToTemplateParams(string path)
    {
        var content = File.ReadAllText(path);
        if (content.Contains(ProjectId, StringComparison.Ordinal))
        {
            content = content.Replace(@$"{ProjectId}.Web\", @"$ext_projectname$.Web\");
            content = content.Replace(@$"\{ProjectId}", @"\$ext_projectname$");
            content = content.Replace(@$"{ProjectId}\", @"$ext_projectname$\");
            content = content.Replace(@$"{ProjectId}.Web", @"$ext_safeprojectname$.Web");
            content = content.Replace(ProjectId, "$ext_safeprojectname$");
            File.WriteAllText(path, content, Shared.UTF8Bom);
        }
    }

    public static void PatchVsTemplateAndCopyFiles()
    {
        List<string> fileList;
        var csprojXml = XElement.Parse(File.ReadAllText(ProjectFile));

        var vsTemplateFile = Path.Combine(VSIXTemplateFolder, ProjectName + ".vstemplate");
        fileList = Directory.GetFiles(ProjectFolder, "*.*", SearchOption.AllDirectories)
            .Select(x => x[(ProjectFolder.Length + 1)..])
            .Where(x => !ExcludeFilesGlob.IsMatch(x))
            .OrderBy(x => x, StringComparer.InvariantCultureIgnoreCase)
            .ToList();

        var vsTemplateXml = XElement.Parse(File.ReadAllText(vsTemplateFile));

        XNamespace ns = "http://schemas.microsoft.com/developer/vstemplate/2005";
        var conditionalsElement = vsTemplateXml.Descendants(ns + "conditionals").First();

        var project = vsTemplateXml.Descendants(ns + "Project").First();
        project.Elements().Remove();
        var byFolder = new Dictionary<string, XElement>();

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
                continue;

            var item = new XElement(ns + "ProjectItem");
            var extension = (Path.GetExtension(file) ?? "").ToLowerInvariant();
            bool replaceParameters = ReplaceParamsInExtensions.Contains(extension);

            item.SetAttributeValue("ReplaceParameters", replaceParameters ? "true" : "false");
            item.SetAttributeValue("TargetFileName", parts[^1]
                .Replace(ProjectId, "$ext_projectname$"));
            if (file == "Welcome.htm")
                item.SetAttributeValue("OpenInWebBrowser", "true");
            item.SetValue(parts[^1]);
            folder.Add(item);

            var targetFile = Path.Combine(TemplateZipWebFolder, file);
            Directory.CreateDirectory(Path.GetDirectoryName(targetFile));

            var sourceFile = Path.Combine(ProjectFolder, file);
            File.Copy(sourceFile, targetFile);

            if (replaceParameters)
            {
                ConvertToTemplateParams(targetFile);

                var text = File.ReadAllText(targetFile);

                if (HasConditionals(text) &&
                    !conditionalsElement.Elements(ns + "files")
                        .Any(x => string.Equals(x.Attribute("include")?.Value?.Trim(), file)))
                {
                    var el = new XElement(ns + "files");
                    el.SetAttributeValue("include", file);
                    conditionalsElement.Add(el);
                }
            }
        }

        File.WriteAllText(vsTemplateFile, vsTemplateXml.ToString(SaveOptions.OmitDuplicateNamespaces));
        File.Copy(vsTemplateFile, Path.Combine(TemplateZipWebFolder, Path.GetFileName(vsTemplateFile)));

        foreach (var z in csprojXml.Descendants("ItemGroup")
            .Concat(csprojXml.Descendants("PackageReference"))
            .Concat(csprojXml.Descendants("ProjectReference"))
            .Concat(csprojXml.Descendants("Import"))
            .Where(x =>
            {
                if (x.Attribute("Condition") == null ||
                    x.Attribute("Condition").Value == null)
                    return false;

                return true;
            }).ToList())
        {
            var value = z.Attribute("Condition").Value.Trim()
                .Replace(" ", "");

            if (string.Equals(value, "'$(UseProjectRefs)'=='false'", StringComparison.OrdinalIgnoreCase))
                z.Attribute("Condition").Remove();
            else if (string.Equals(value, "'$(UseProjectRefs)'!='false'", StringComparison.OrdinalIgnoreCase))
                z.Remove();
        }

        var targetProj = Path.Combine(TemplateZipWebFolder, Path.GetFileName(ProjectFile));
        File.WriteAllText(targetProj,
            csprojXml.ToString(SaveOptions.OmitDuplicateNamespaces)
                .Replace("http://localhost:55555/", "")
                .Replace(
                "<DevelopmentServerPort>55556</DevelopmentServerPort>", 
                "<DevelopmentServerPort></DevelopmentServerPort>"));
        ConvertToTemplateParams(targetProj);
    }

}
#endif