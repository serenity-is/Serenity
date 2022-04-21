using System.Diagnostics;
using System.IO;

namespace Serenity.CodeGenerator
{
    public class CodeFileHelper
    {
        private static readonly Encoding utf8 = new UTF8Encoding(true);
        public static string Kdiff3Path { get; set; }
        public static string TSCPath { get; set; }
        public static bool TFSIntegration { get; set; }
        public static bool? Overwrite { get; set; }

        public static byte[] ToUTF8BOM(string s)
        {
            return Encoding.UTF8.GetPreamble().Concat(utf8.GetBytes(s)).ToArray();
        }

#pragma warning disable IDE0060 // Remove unused parameter
        public static void ExecuteTFCommand(string file, string command)
#pragma warning restore IDE0060 // Remove unused parameter
        {
        }

        public static void CheckoutAndWrite(string file, string contents, bool addToSourceControl)
        {
            CheckoutAndWrite(file, ToUTF8BOM(contents), addToSourceControl);
        }

        public static void CheckoutAndWrite(string file, byte[] contents, bool addToSourceControl)
        {
            if (!File.Exists(file))
            {
                File.WriteAllBytes(file, contents);
                if (addToSourceControl && TFSIntegration)
                    ExecuteTFCommand(file, "add");
                return;
            }

            var attr = File.GetAttributes(file);
            if (attr.HasFlag(FileAttributes.ReadOnly) && TFSIntegration)
            {
                ExecuteTFCommand(file, "checkout");
                attr = File.GetAttributes(file);
            }

            if (attr.HasFlag(FileAttributes.ReadOnly))
            {
                attr -= FileAttributes.ReadOnly;
                File.SetAttributes(file, attr);
            }

            File.WriteAllBytes(file, contents);
        }

        public static bool FileContentsEqual(string file1, string file2)
        {
            var content1 = File.ReadAllText(file1, utf8);
            var content2 = File.ReadAllText(file2, utf8);
            return content1.Trim().Replace("\r", "", StringComparison.Ordinal) == content2.Trim().Replace("\r", "", StringComparison.Ordinal);
        }

        public static void MergeChanges(string backup, string file)
        {
            if (backup == null || !File.Exists(backup) || !File.Exists(file))
                return;

            bool isEqual = FileContentsEqual(backup, file);

            if (isEqual)
            {
                CheckoutAndWrite(file, File.ReadAllBytes(backup), true);
                File.Delete(backup);
                return;
            }

            if (!Kdiff3Path.IsEmptyOrNull() &&
                !File.Exists(Kdiff3Path))
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
                var generated = Path.ChangeExtension(file, Path.GetExtension(file) + ".gen.bak");
                CheckoutAndWrite(generated, File.ReadAllBytes(file), false);
                CheckoutAndWrite(file, File.ReadAllBytes(backup), true);
                Process.Start(Kdiff3Path, "--auto \"" + file + "\" \"" + generated + "\" -o \"" + file + "\"");
            }
            else
            {
                string answer;
                if (Overwrite == true)
                    answer = "y";
                else if (Overwrite == false)
                    answer = "n";
                else
                {
                    
                    while (true)
                    {
                        Console.Write("Overwrite " + Path.GetFileName(file) + "? ([Y]es, [N]o, Yes to [A]ll, [S]kip All): ");
                        answer = Console.ReadLine();

                        if (answer != null)
                        {
                            answer = answer.Length > 0 ? answer.ToLowerInvariant()[0].ToString() : " ";
                            if (answer == "a")
                            {
                                Overwrite = true;
                                break;
                            }
                            else if (answer == "s")
                            {
                                Overwrite = false;
                                break;
                            }
                            else if (answer == "y" || answer == "n")
                                break;
                        }
                    }
                }
                    
                if (answer == "y" || answer == "a")
                {
                    File.Delete(backup);
                }
                else
                {
                    CheckoutAndWrite(file, File.ReadAllBytes(backup), true);
                    File.Delete(backup);
                }
                
            }
        }

        public static void ExecuteTSC(string workingDirectory, string arguments)
        {
            if (TSCPath.IsNullOrEmpty() ||
                !File.Exists(TSCPath))
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
}