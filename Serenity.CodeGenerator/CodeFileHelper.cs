using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Serenity.CodeGenerator
{
    public class CodeFileHelper
    {
        private static Encoding utf8 = new UTF8Encoding(true);
        public static string Kdiff3Path;
        public static bool TFSIntegration;
        public static string TFPath;

        private bool InsertDefinition(string file, string type, string key, string code)
        {
            int insertAfter = -1;
            int lastPermission = -1;
            bool alreadyAdded = false;

            List<string> lines = new List<string>();
            var spaceRegex = new Regex("\\s+");
            using (var sr = new StreamReader(file, utf8))
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
                    var idx = line.IndexOf(s);
                    if (idx >= 0)
                    {
                        var idx2 = line.IndexOf("=");
                        if (idx2 >= 0)
                        {
                            var ar = line.Substring(idx + s.Length, idx2 - idx - s.Length).TrimToNull();
                            if (ar != null)
                            {
                                int comp = String.CompareOrdinal(key, ar);
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

                using (var sw = new StreamWriter(file, false, utf8))
                {
                    sw.Write(String.Join("\r\n", lines));
                }
                return true;
            }

            return false;
        }

        public static bool StreamsContentsAreEqual(Stream stream1, Stream stream2)
        {
            const int bufferSize = 2048 * 2;
            var buffer1 = new byte[bufferSize];
            var buffer2 = new byte[bufferSize];

            while (true)
            {
                int count1 = stream1.Read(buffer1, 0, bufferSize);
                int count2 = stream2.Read(buffer2, 0, bufferSize);

                if (count1 != count2)
                {
                    return false;
                }

                if (count1 == 0)
                {
                    return true;
                }

                int iterations = (int)Math.Ceiling((double)count1 / sizeof(Int64));
                for (int i = 0; i < iterations; i++)
                {
                    if (BitConverter.ToInt64(buffer1, i * sizeof(Int64)) != BitConverter.ToInt64(buffer2, i * sizeof(Int64)))
                    {
                        return false;
                    }
                }
            }
        }

        public static void ExecuteTFCommand(string file, string command)
        {
            if (TFPath.IsNullOrEmpty() ||
                !File.Exists(TFPath))
            {
                if (TFPath.IsNullOrEmpty())
                    throw new Exception(
                        "Couldn't locate TF.EXE utility which is required for TFS integration. " +
                        "Please install it, or if it is not installed to default location, " +
                        "set its path in CodeGenerator.config file!");

                throw new Exception(String.Format(
                    "Couldn't locate TF.EXE utility at '{0}' which is required for TFS integration. " +
                    "Please install it, or if it is not installed to default location, " +
                    "set its path in CodeGenerator.config file!", TFPath));
            }

            Process.Start(TFPath, command + " " + file).WaitForExit(10000);
        }

        public static bool CheckoutAndWrite(string file, byte[] contents, bool addToSourceControl)
        {
            if (!File.Exists(file))
            {
                File.WriteAllBytes(file, contents);
                if (addToSourceControl && TFSIntegration)
                    ExecuteTFCommand(file, "add");
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
            return true;
        }

        public static void MergeChanges(string backup, string file)
        {
            if (backup == null || !File.Exists(backup) || !File.Exists(file))
                return;

            bool isEqual;
            using (var fs1 = new FileStream(backup, FileMode.Open))
            using (var fs2 = new FileStream(file, FileMode.Open))
                isEqual = CodeFileHelper.StreamsContentsAreEqual(fs1, fs2);

            if (isEqual)
            {
                File.Delete(backup);
                return;
            }

            var generated = Path.ChangeExtension(file, Path.GetExtension(file) + ".gen.bak");
            CheckoutAndWrite(generated, File.ReadAllBytes(file), false);
            CheckoutAndWrite(file, File.ReadAllBytes(backup), true);

            if (Kdiff3Path.IsNullOrEmpty() ||
                !File.Exists(Kdiff3Path))
            {
                if (Kdiff3Path.IsNullOrEmpty())
                    throw new Exception(
                        "Couldn't locate KDiff3 utility which is required to merge changes. " +
                        "Please install it, or if it is not installed to default location, " + 
                        "set its path in CodeGenerator.config file!");

                throw new Exception(String.Format(
                    "Couldn't locate KDiff3 utility at '{0}' which is required to merge changes. " +
                    "Please install it, or if it is not installed to default location, " +
                    "set its path in CodeGenerator.config file!", Kdiff3Path));
            }

            Process.Start(Kdiff3Path, "--auto " + file + " " + generated + " -o " + file);
        }

        public static void SetupTFSIntegration(string tfPath)
        {
            TFSIntegration = true;
            var tfPaths = new List<string>();
            if (!string.IsNullOrEmpty(tfPath) && File.Exists(tfPath))
                TFPath = tfPath;
            else
            {
                var pf86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                var pf64 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

                for (var i = 20; i > 10; i--)
                {
                    var folder = @"Microsoft Visual Studio " + i + @".0\Common7\IDE\TF.exe";
                    var f86 = Path.Combine(pf86, folder);
                    if (File.Exists(f86))
                    {
                        TFPath = f86;
                        break;
                    }
                    var f64 = Path.Combine(pf64, folder);
                    if (File.Exists(f64))
                    {
                        TFPath = f64;
                        return;
                    }
                }

                // to give meaningfull error
                TFPath = tfPath.TrimToNull();
            }
        }
    }
}
