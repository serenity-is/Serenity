using Serenity.CodeGeneration;

namespace Serenity.CodeGenerator
{
    public class MultipleOutputHelper
    {
        private static readonly Encoding utf8 = new UTF8Encoding(true);

        public static void WriteFiles(IGeneratorFileSystem fileSystem,
            string outDir, IEnumerable<(string Path, string Text)> filesToWrite,
            string[] deleteExtraPattern,
            string endOfLine)
        {
            if (fileSystem is null)
                throw new ArgumentNullException(nameof(fileSystem));

            fileSystem.CreateDirectory(outDir);

            var generated = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var file in filesToWrite)
            {
                generated.Add(file.Path);

                var outFile = fileSystem.Combine(outDir, file.Path);
                bool exists = fileSystem.FileExists(outFile);
                if (exists)
                {
                    var content = fileSystem.ReadAllText(outFile, utf8);
                    if (content.Trim().Replace("\r", "", StringComparison.Ordinal) ==
                        (file.Text ?? "").Trim().Replace("\r", "", StringComparison.Ordinal))
                        continue;
                }

#if !ISSOURCEGENERATOR
                Console.ForegroundColor = exists ? ConsoleColor.Magenta : ConsoleColor.Green;
                Console.Write(exists ? "Overwriting: " : "New File: ");
                Console.ResetColor();
                Console.WriteLine(fileSystem.GetFileName(outFile));
#endif

                string text = file.Text ?? "";
                if (string.Equals(endOfLine, "lf", StringComparison.OrdinalIgnoreCase))
                    text = text.Replace("\r", "");
                else if (string.Equals(endOfLine, "crlf", StringComparison.OrdinalIgnoreCase))
                    text = text.Replace("\r", "").Replace("\n", "\r\n");

                fileSystem.WriteAllText(outFile, text, utf8);
            }

            if (deleteExtraPattern?.Length is null or 0)
                return;

            var filesToDelete = deleteExtraPattern.SelectMany(
                    x => fileSystem.GetFiles(outDir, x))
                .Distinct();

            foreach (var file in filesToDelete)
                if (!generated.Contains(fileSystem.GetFileName(file)))
                {
#if !ISSOURCEGENERATOR
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Deleting: ");
                    Console.ResetColor();
                    Console.WriteLine(fileSystem.GetFileName(file));
#endif
                    fileSystem.DeleteFile(file);
                }
        }
    }
}