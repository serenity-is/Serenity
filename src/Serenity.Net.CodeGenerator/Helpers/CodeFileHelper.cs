using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Serenity.CodeGenerator
{
    public class CodeFileHelper
    {
        private static Encoding utf8 = new UTF8Encoding(true);
        public static string Kdiff3Path;
        public static string TSCPath;
        public static bool TFSIntegration;
        public static bool? Overwrite;

        public static byte[] ToUTF8BOM(string s)
        {
            return Encoding.UTF8.GetPreamble().Concat(utf8.GetBytes(s)).ToArray();
        }

        private bool InsertDefinition(string file, string type, string key, string code)
        {
            int insertAfter = -1;
            int lastPermission = -1;
            bool alreadyAdded = false;

            List<string> lines = new List<string>();
            var spaceRegex = new Regex("\\s+");
            using (var sr = new StreamReader(File.OpenRead(file), utf8))
            {
                int lineNum = 0;
                while (true)
                {
                    string line = sr.ReadLine();
                    if (line == null)
                        break;
                    lines.Add(line);
                    line = spaceRegex.Replace(line.TrimToEmpty(), " ");
                    string s = " " + type + " ";
                    var idx = line.IndexOf(s, StringComparison.Ordinal);
                    if (idx >= 0)
                    {
                        var idx2 = line.IndexOf("=", StringComparison.Ordinal);
                        if (idx2 >= 0)
                        {
                            var ar = line.Substring(idx + s.Length, idx2 - idx - s.Length).TrimToNull();
                            if (ar != null)
                            {
                                int comp = string.CompareOrdinal(key, ar);
                                if (comp > 0)
                                    insertAfter = lineNum;
                                else if (comp == 0)
                                    alreadyAdded = true;
                            }
                        }
                    }

                    lineNum++;
                }
            }

            if (alreadyAdded)
                return true;

            if (insertAfter == -1)
                insertAfter = lastPermission;

            if (insertAfter != -1)
            {
                lines.Insert(insertAfter + 1, code.TrimEnd());

                using (var sw = new StreamWriter(File.Create(file), utf8))
                {
                    sw.Write(string.Join(Environment.NewLine, lines));
                }
                return true;
            }

            return false;
        }

        public static void ExecuteTFCommand(string file, string command)
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
                string answer = null;
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

            var psi = new ProcessStartInfo(TSCPath, arguments);
            psi.WorkingDirectory = workingDirectory;
            Process.Start(psi).WaitForExit(10000);
        }
    }
}