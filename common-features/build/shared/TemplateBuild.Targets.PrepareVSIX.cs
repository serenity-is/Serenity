#if IsTemplateBuild
using System.IO;
using System.IO.Compression;

namespace Build;

public static partial class Shared
{
    public static partial class Targets
    {
        public static void PrepareVSIX()
        {
            Clean();

            UpdateSerenityPackages();
            UpdateCommonAndProPackages();

            if (StartProcess("dotnet", "restore", Root) != 0)
                ExitWithError("Error while restoring " + ProjectFile);

            if (StartProcess("dotnet", "build -v minimal -c Release " + ProjectId + ".sln", Root) != 0)
                ExitWithError("Error while building solution!");

            var projectPackages = ParsePackages(ProjectFile);

            CleanDirectory(TemporaryFilesRoot, ensure: true);

            var initialVersion = PatchVSIXManifest(projectPackages, out string serenityVersion);
            SetInitialVersionInSergenJson(initialVersion);
            SetTemplatesPackageVersion(initialVersion);

            if (StartProcess("dotnet", "restore " + Path.GetFileName(ProjectFile), ProjectFolder) != 0)
                ExitWithError("Error while restoring " + ProjectFile);

            UpdateSergen(ProjectFolder, serenityVersion);
            PatchPackageJsonCopy();

            if (!IsStartSharp)
            {
                CleanDirectory(TemplateZipFolder, ensure: true);
                CleanDirectory(TemplateZipWebFolder, ensure: true);
                PatchVsTemplateAndCopyFiles();

                File.Copy(Path.Combine(VSIXTemplateFolder, "SerenityLogo.ico"),
                    Path.Combine(TemplateZipFolder, "SerenityLogo.ico"));

                File.Copy(Path.Combine(VSIXTemplateFolder, ProjectId + ".vstemplate"),
                    Path.Combine(TemplateZipFolder, ProjectId + ".vstemplate"));

                File.Copy(PackageJsonCopy, Path.Combine(TemplateZipWebFolder, Path.GetFileName(PackageJsonCopy)), overwrite: true);

                var templateZip = Path.Combine(VSIXProjectTemplates, TemplateId + ".Template.zip");
                if (File.Exists(templateZip))
                    File.Delete(templateZip);
                Directory.CreateDirectory(VSIXProjectTemplates);
                ZipFile.CreateFromDirectory(TemplateZipFolder, templateZip);
            }
        }
    }
}
#endif