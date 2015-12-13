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
            File.Copy(file, generated, true);
            File.Copy(backup, file, true);

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
    }
}