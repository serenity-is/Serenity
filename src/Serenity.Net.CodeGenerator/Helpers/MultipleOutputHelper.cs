using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Serenity.CodeGenerator
{
    public class MultipleOutputHelper
    {
        private static Encoding utf8 = new System.Text.UTF8Encoding(true);

        public void WriteFiles(string outDir, SortedDictionary<string, string> codeByFilename, params string[] deleteExtraPattern)
        {
            Directory.CreateDirectory(outDir);

            var generated = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var pair in codeByFilename)
            {
                generated.Add(pair.Key);

                var outFile = Path.Combine(outDir, pair.Key);
                bool exists = File.Exists(outFile);
                if (exists)
                {
                    var content = File.ReadAllText(outFile, utf8);
                    if (content.Trim().Replace("\r", "", StringComparison.Ordinal) ==
                        pair.Value.Trim().Replace("\r", "", StringComparison.Ordinal))
                        continue;
                }

                Console.ForegroundColor = exists ? ConsoleColor.Magenta : ConsoleColor.Green;
                Console.Write(exists ? "Overwriting: " : "New File: ");
                Console.ResetColor();
                Console.WriteLine(Path.GetFileName(outFile));

                File.WriteAllText(outFile, pair.Value, utf8);
            }

            if (deleteExtraPattern.Length == 0)
                return;

            var filesToDelete = deleteExtraPattern.SelectMany(
                    x => Directory.GetFiles(outDir, x))
                .Distinct();

            foreach (var file in filesToDelete)
                if (!generated.Contains(Path.GetFileName(file)))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Deleting: ");
                    Console.ResetColor();
                    Console.WriteLine(Path.GetFileName(file));

                    File.Delete(file);
                }
        }
    }
}