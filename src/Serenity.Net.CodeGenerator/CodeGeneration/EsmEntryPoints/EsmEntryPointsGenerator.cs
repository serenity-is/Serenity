using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace Serenity.CodeGeneration;

public class EsmEntryPointsGenerator()
{
    public List<string> EntryPoints { get; } = [
        "Modules/**/*Page.ts",
        "Modules/**/*Page.tsx",
        "Modules/**/ScriptInit.ts"
    ];

    public string EsmAssetBasePath { get; set; } = "/esm";

    public string Generate(IFileSystem fileSystem, string projectDir, 
        string rootNamespace, bool fileScopedNamespace = false, bool internalAccess = false)
    {
        ArgumentExceptionHelper.ThrowIfNull(fileSystem);
        ArgumentExceptionHelper.ThrowIfNull(rootNamespace);

        Matcher matcher = new();
        matcher.AddExclude(".git/**");
        matcher.AddExclude("App_Data/**");
        matcher.AddExclude("bin/**");
        matcher.AddExclude("obj/**");
        matcher.AddExclude("node_modules/**");
        matcher.AddExclude("**/node_modules/**");

        foreach (var pattern in EntryPoints)
        {
            if (string.IsNullOrEmpty(pattern))
                continue;

            if (pattern.StartsWith('!'))
                matcher.AddExclude(pattern[1..]);
            else
                matcher.AddInclude(pattern);
        }

        IEnumerable<string> files;
        if (fileSystem is PhysicalFileSystem)
        {
            var matches = matcher.Execute(new DirectoryInfoWrapper(new System.IO.DirectoryInfo(projectDir))).Files;
            files = matches.Select(match => match.Path).ToArray();
        }
        else
        {
            var matches = matcher.Execute(new FileSystemDirectoryWrapper(fileSystem, projectDir)).Files;
            files = matches.Select(match => match.Path).ToArray();
        }

        string getStrippedName(string s)
        {
            return fileSystem.ChangeExtension(s, null).Replace('\\', '/');
        }

        string normalizeParts(string s)
        {
            foreach (var character in s)
            {
                if (!char.IsLetterOrDigit(character) && character != '_')
                    s = s.Replace(character, '_');
            }

            if (!char.IsLetter(s[0]) && s[0] != '_')
                s = $"_{s}";

            return s;
        }

        files = [.. files.OrderBy(getStrippedName)];

        var cw = new CodeWriter
        {
            IsCSharp = true
        };

        void action()
        {
            cw.IndentedLine($"{(internalAccess ? "internal" : "public")} static partial class ESM");
            cw.InBrace(() =>
            {
                var firstParts = new HashSet<string>(files.Select(x => normalizeParts(getStrippedName(x).Split('/').First())));
                var byShortName = files.ToLookup(x => normalizeParts(getStrippedName(x).Split('/').Last()))
                    .OrderBy(x => x.Key)
                    .Where(x => !firstParts.Contains(x.Key) &&
                        x.Count() == 1);

                foreach (var shortName in byShortName)
                    cw.IndentedLine("public const string " + shortName.Key + " = \"~" + EsmAssetBasePath + "/" +
                        System.IO.Path.ChangeExtension(shortName.First(), ".js").Replace('\\', '/') + "\";");

                if (byShortName.Any())
                    cw.AppendLine();

                var last = Array.Empty<string>();
                var processed = new HashSet<string>();

                bool constLine = false;
                foreach (var file in files)
                {
                    var name = getStrippedName(file);
                    if (processed.Contains(name))
                        continue;

                    processed.Add(name);
                    var parts = name.Split(['/']);

                    if (parts.Length == 0)
                        continue;

                    for (var i = 0; i < parts.Length; i++)
                    {
                        parts[i] = normalizeParts(parts[i]);
                        if (i > 0 && parts[i - 1] == parts[i])
                            parts[i] += "_";
                    }

                    bool endOfBrace = false;
                    for (var i = last.Length; i > parts.Length; i--)
                    {
                        cw.EndBrace();
                        endOfBrace = true;
                        constLine = false;
                    }

                    var x = Math.Min(last.Length, parts.Length) - 2;

                    for (var i = 0; i <= x; i++)
                    {
                        if (last[i] == parts[i])
                            continue;

                        while (x >= i)
                        {
                            cw.EndBrace();
                            x--;
                            endOfBrace = true;
                            constLine = false;
                        }

                        break;
                    }

                    for (var i = Math.Max(x + 1, 0); i < parts.Length - 1; i++)
                    {
                        if (endOfBrace)
                        {
                            cw.AppendLine();
                            endOfBrace = false;
                        }

                        var u = parts[i];

                        if (constLine)
                        {
                            cw.AppendLine();
                            constLine = false;
                        }
                        cw.IndentedLine("public static partial class " + u);
                        cw.StartBrace();
                    }

                    var n = parts[^1];

                    if (endOfBrace)
                    {
                        cw.AppendLine();
                        endOfBrace = false;
                    }

                    cw.IndentedLine("public const string " + n + " = \"~" + EsmAssetBasePath + "/" +
                        System.IO.Path.ChangeExtension(file, ".js").Replace('\\', '/') + "\";");
                    constLine = true;
                    last = parts;
                }

                for (var i = last.Length - 1; i > 0; i--)
                    cw.EndBrace();
            });
        }

        if (fileScopedNamespace)
        {
            cw.IndentedLine("namespace " + rootNamespace + ";");
            cw.AppendLine();
            action();
        }
        else
        {
            cw.InNamespace(rootNamespace, action);
        }

        return cw.ToString().TrimEnd();
    }
}