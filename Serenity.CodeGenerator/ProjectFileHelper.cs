using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Serenity.CodeGenerator
{
    public class ProjectFileHelper
    {
        public static void AddFileToProject(string projectFile, string codeFile, string dependentUpon = null)
        {
            if (File.Exists(projectFile))
            {
                XElement doc;
                using (var sr = new StreamReader(projectFile))
                    doc = XElement.Parse(sr.ReadToEnd(), LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);

                var ns = doc.GetDefaultNamespace();

                Func<string, XElement> findItemGroupOf = fileName =>
                {
                    foreach (var group in doc.Elements(ns + "ItemGroup"))
                    {
                        foreach (var c in group.Elements(ns + "Compile")
                                    .Concat(group.Elements(ns + "TypeScriptCompile"))
                                    .Concat(group.Elements(ns + "Content"))
                                    .Concat(group.Elements(ns + "None")))
                            if (c.Attribute("Include").Value.ToLowerInvariant() == fileName.ToLowerInvariant())
                            {
                                return group;
                            }
                    }

                    return null;
                };

                if (findItemGroupOf(codeFile) != null)
                    return;

                XElement dependentGroup = null;
                if (dependentUpon != null)
                {
                    dependentGroup = findItemGroupOf(dependentUpon);
                    if (dependentGroup == null)
                        dependentUpon = null;
                }

                string contentType;
                if (String.Compare(Path.GetExtension(codeFile), ".cs") == 0)
                    contentType = "Compile";
                else if (String.Compare(Path.GetExtension(codeFile), ".ts") == 0)
                    contentType = "TypeScriptCompile";
                else
                    contentType = "Content";

                XElement targetGroup = dependentGroup;
                if (targetGroup == null)
                {
                    foreach (var group in doc.Elements(ns + "ItemGroup"))
                    {
                        var compiles = group.Elements(ns + contentType);
                        if (compiles.Count() > 0)
                        {
                            targetGroup = group;
                            break;
                        }
                    }
                }

                if (targetGroup == null)
                    return; // create a group??

                var newElement = new XElement(ns + contentType, new XAttribute("Include", codeFile));

                var lastElement = targetGroup.Elements().LastOrDefault();
                XText space = null;
                if (lastElement != null)
                    space = lastElement.PreviousNode as XText;
                if (lastElement != null)
                {
                    if (space != null)
                        lastElement.AddAfterSelf(new XText(space.Value), newElement);
                    else
                        lastElement.AddAfterSelf(newElement);
                }
                else
                    targetGroup.Add(newElement);

                if (dependentUpon != null)
                {
                    newElement.Add(new XText("  "));
                    newElement.Add(new XElement(ns + "DependentUpon", Path.GetFileName(dependentUpon)));
                }

                using (var sw = new StreamWriter(projectFile, false, new UTF8Encoding(true)))
                {
                    sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    sw.Write(doc.ToString());
                }
            }
        }
    }
}