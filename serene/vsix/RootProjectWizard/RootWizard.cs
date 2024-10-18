using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml.Linq;

namespace RootProjectWizard
{
    public class RootWizard : IWizard
    {
        public static HashSet<string> AllKnowFeatureKeys = new HashSet<string>()
        {
            "IsNotTemplate"
        };

        public static HashSet<string> SelectedFeatures = new HashSet<string>();
        public static Dictionary<string, string> GlobalDictionary = new Dictionary<string, string>();

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            GlobalDictionary["$ext_safeprojectname$"] = replacementsDictionary["$safeprojectname$"];
            GlobalDictionary["$ext_projectname$"] = replacementsDictionary["$projectname$"];

            string wizardData;
            if (!replacementsDictionary.TryGetValue("$wizarddata$", out wizardData))
                wizardData = "";

            var data = XElement.Parse("<data>" + wizardData + "</data>");
            var dlg = new FeatureSelection();
            dlg.selfChange = 1;
            PopulateFeatureList(dlg.featureList, data);
            dlg.selfChange = 0;

            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                throw new WizardCancelledException("The wizard has been cancelled by the user.");

            SelectedFeatures.Clear();
            foreach (FeatureCheckItem item in dlg.featureList.CheckedItems)
                SelectedFeatures.Add(item.Key);

            CheckNodeNpmVersion();
        }

        private string GetNodeNpmVersion(bool npm)
        {
            var npmProcess = new System.Diagnostics.Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = "/c " + (npm ? "npm.cmd" : "node") + " --version",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };

            string npmOutput = null;
            try
            {
                npmProcess.Start();
                npmOutput = npmProcess.StandardOutput.ReadToEnd();
                if (!npmProcess.WaitForExit(50000))
                    npmOutput = null;
            }
            catch (Exception)
            {
                npmOutput = null;
            }

            npmOutput = (npmOutput ?? "").Trim();

            if (npmOutput.StartsWith("v", StringComparison.OrdinalIgnoreCase))
                npmOutput = npmOutput.Substring(1);

            return npmOutput;
        }

        private void CheckNodeNpmVersion()
        {
            var nodeVersion = GetNodeNpmVersion(false);
            var nodeParts = nodeVersion.Split('.');
            var npmVersion = GetNodeNpmVersion(true);
            var npmParts = npmVersion.Split('.');

            int i;
            if (nodeParts.Length < 2 || !int.TryParse(nodeParts[0], out i) || i < 16 ||
                npmParts.Length < 2 || !int.TryParse(npmParts[0], out i) || i < 8)
            {
                if (MessageBox.Show("You don't seem to have a recent version of NodeJS/NPM installed!\r\n\r\n" +
                    "It is required for TypeScript compilation and some script packages.\r\n\r\n" +
                    "Latest version can be downloaded from https://nodejs.org/en/download/\r\n\r\n" +
                    "Would you like to download and install it now?", "NodeJS/NPM Warning",
                        MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    WebClient client = new WebClient();
                    var npmBytes = client.DownloadData("https://nodejs.org/dist/v16.13.2/node-v16.13.2-" +
                        (Environment.Is64BitOperatingSystem ? "x64" : "x86") + ".msi");
                    var tmp = Path.GetTempFileName() + ".msi";
                    File.WriteAllBytes(tmp, npmBytes);
                    try
                    {
                        var msiProcess = new System.Diagnostics.Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = "msiexec",
                                Arguments = "/l* \"" + Path.GetFileName(tmp) + ".log\" /i \"" + Path.GetFileName(tmp) + "\"",
                                WorkingDirectory = Path.GetDirectoryName(tmp)
                            }
                        };

                        msiProcess.Start();
                        if (!msiProcess.WaitForExit(1000000) ||
                            msiProcess.ExitCode != 0)
                        {
                            MessageBox.Show("Error code " + msiProcess.ExitCode + " occured while installing NodeJS!");
                        }
                        else
                            MessageBox.Show("Please restart Visual Studio after project creation, so that Node/NPM will be in your PATH");
                    }
                    finally
                    {
                        File.Delete(tmp);
                    }
                }
            }
        }

        private void PopulateFeatureList(CheckedListBox featureList, XElement data)
        {
            XNamespace ns = "http://schemas.microsoft.com/developer/vstemplate/2005";
            var elFeatures = data.Elements(ns + "features");
            var elFeatureList = elFeatures.Elements(ns + "feature");
            featureList.Items.Clear();
            foreach (var elFeature in elFeatureList)
            {
                AllKnowFeatureKeys.Add(elFeature.Attribute("key").Value);
                var item = new FeatureCheckItem(elFeature.Attribute("key").Value, elFeature.Attribute("title").Value);
                foreach (var elDependency in elFeature.Elements(ns + "dependency"))
                    if (elDependency.Attribute("feature") != null)
                        item.FeatureDependencies.Add(elDependency.Attribute("feature").Value);

                featureList.Items.Add(item, true);
            }
        }

        public void BeforeOpeningFile(EnvDTE.ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(EnvDTE.Project project)
        {
        }

        public void ProjectItemFinishedGenerating(EnvDTE.ProjectItem projectItem)
        {
        }

        public void RunFinished()
        {
            
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}
