using System.Diagnostics;

namespace Serenity.CodeGenerator;

public class CodeFileHelper : ICodeFileHelper
{
    private static readonly Encoding utf8 = new UTF8Encoding(true);
    private readonly IGeneratorFileSystem fileSystem;

    public string Kdiff3Path { get; set; }
    public string TSCPath { get; set; }
    public bool NoUserInteraction { get; set; }

    private bool? overwriteAll;

    public static byte[] ToUTF8BOM(string s)
    {
        return Encoding.UTF8.GetPreamble().Concat(utf8.GetBytes(s)).ToArray();
    }

    public CodeFileHelper(IGeneratorFileSystem fileSystem)
    {
        this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }

    public void CheckoutAndWrite(string file, string contents)
    {
        CheckoutAndWrite(file, ToUTF8BOM(contents));
    }

    public void CheckoutAndWrite(string file, byte[] contents)
    {
        fileSystem.WriteAllBytes(file, contents);
    }

    public bool FileContentsEqual(string file1, string file2)
    {
        var content1 = fileSystem.ReadAllText(file1, utf8);
        var content2 = fileSystem.ReadAllText(file2, utf8);
        return content1.Trim().Replace("\r", "", StringComparison.Ordinal) ==
            content2.Trim().Replace("\r", "", StringComparison.Ordinal);
    }

    public void MergeChanges(string backup, string file)
    {
        if (backup == null || !fileSystem.FileExists(backup) || !fileSystem.FileExists(file))
            return;

        bool isEqual = FileContentsEqual(backup, file);

        if (isEqual || NoUserInteraction)
        {
            CheckoutAndWrite(file, fileSystem.ReadAllBytes(backup));
            fileSystem.DeleteFile(backup);
            return;
        }

        if (!Kdiff3Path.IsEmptyOrNull() &&
            !fileSystem.FileExists(Kdiff3Path))
        {
            if (Kdiff3Path.IsNullOrEmpty())
                throw new InvalidOperationException(
                    "Couldn't locate KDiff3 utility which is required to merge changes. " +
                    "Please install it, or if it is not installed to default location, " +
                    "set its path in CodeGenerator.config file!");

            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                "Couldn't locate KDiff3 utility at '{0}' which is required to merge changes. " +
                "Please install it, or if it is not installed to default location, " +
                "set its path in CodeGenerator.config file!", Kdiff3Path));
        }
        else if (!Kdiff3Path.IsEmptyOrNull())
        {
            var generated = fileSystem.ChangeExtension(file, fileSystem.GetExtension(file) + ".gen.bak");
            CheckoutAndWrite(generated, fileSystem.ReadAllBytes(file));
            CheckoutAndWrite(file, fileSystem.ReadAllBytes(backup));
            Process.Start(Kdiff3Path, "--auto \"" + file + "\" \"" + generated + "\" -o \"" + file + "\"");
        }
        else
        {
            string answer;
            if (overwriteAll == true)
                answer = "y";
            else if (overwriteAll == false)
                answer = "n";
            else
            {

                while (true)
                {
                    Console.Write("Overwrite " + fileSystem.GetFileName(file) + "? ([Y]es, [N]o, Yes to [A]ll, [S]kip All): ");
                    answer = Console.ReadLine();

                    if (answer != null)
                    {
                        answer = answer.Length > 0 ? answer.ToLowerInvariant()[0].ToString() : " ";
                        if (answer == "a")
                        {
                            overwriteAll = true;
                            break;
                        }
                        else if (answer == "s")
                        {
                            overwriteAll = false;
                            break;
                        }
                        else if (answer == "y" || answer == "n")
                            break;
                    }
                }
            }

            if (answer == "y" || answer == "a")
            {
                fileSystem.DeleteFile(backup);
            }
            else
            {
                CheckoutAndWrite(file, fileSystem.ReadAllBytes(backup));
                fileSystem.DeleteFile(backup);
            }
        }
    }

    public void ExecuteTSC(string workingDirectory, string arguments)
    {
        if (NoUserInteraction)
            return;

        if (TSCPath.IsNullOrEmpty() ||
            !fileSystem.FileExists(TSCPath))
        {
            if (TSCPath.IsNullOrEmpty())
                throw new InvalidOperationException(
                    "Couldn't locate TSC.EXE file which is required for TypeScript compilation. " +
                    "Please install it, or if it is not installed to default location, " +
                    "set its path in CodeGenerator.config file (TSCPath setting)!");

            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                "Couldn't locate TSC.EXE file at '{0}' which is required for TypeScript compilation. " +
                "Please install it, or if it is not installed to default location, " +
                "set its path in CodeGenerator.config file! (TSCPath setting)", TSCPath));
        }

        var psi = new ProcessStartInfo(TSCPath, arguments)
        {
            WorkingDirectory = workingDirectory
        };
        Process.Start(psi).WaitForExit(10000);
    }
}