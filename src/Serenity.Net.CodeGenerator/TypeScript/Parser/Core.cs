using Serenity.TypeScript.TsTypes;

namespace Serenity.TypeScript.TsParser;

public static class Core
{

    public static ScriptKind EnsureScriptKind(string fileName, ScriptKind scriptKind)
    {
        // Using scriptKind as a condition handles both:
        // - 'scriptKind' is unspecified and thus it is `null`
        // - 'scriptKind' is set and it is `Unknown` (0)
        // If the 'scriptKind' is 'null' or 'Unknown' then we attempt
        // to get the ScriptKind from the file name. If it cannot be resolved
        // from the file name then the default 'TS' script kind is returned.
        var sk = scriptKind != ScriptKind.Unknown ? scriptKind : GetScriptKindFromFileName(fileName);
        return sk != ScriptKind.Unknown ? sk : ScriptKind.Ts;
    }
    public static ScriptKind GetScriptKindFromFileName(string fileName)
    {
        //var ext = fileName.substr(fileName.LastIndexOf("."));
        var ext = System.IO.Path.GetExtension(fileName);
        return (ext?.ToLower()) switch
        {
            ".js" => ScriptKind.Js,
            ".jsx" => ScriptKind.Jsx,
            ".ts" => ScriptKind.Ts,
            ".tsx" => ScriptKind.Tsx,
            _ => ScriptKind.Unknown,
        };
    }

    public static string NormalizePath(string path)
    {
        path = path.Replace('\\', '/');
        var rootLength = GetRootLength(path);
        var root = path[..rootLength];
        var normalized = GetNormalizedParts(path, rootLength);
        if (normalized.Count != 0)
        {
#if ISSOURCEGENERATOR
            var joinedParts = root + string.Join("/", normalized);
#else
            var joinedParts = root + string.Join('/', normalized);
#endif
            return path[^1] == '/' ? joinedParts + '/' : joinedParts;
        }
        else
        {
            return root;
        }
    }

    public static int GetRootLength(string path)
    {
        if (path[0] == '/')
        {
            if (path.Length < 2 || path[1] != '/')
                return 1;
            var p1 = path.IndexOf('/', 2);
            if (p1 < 0)
                return 2;
            var p2 = path.IndexOf('/', p1 + 1);
            if (p2 < 0)
                return p1 + 1;
            return p2 + 1;
        }

        if (path.Length > 1 && path[1] == ':')
        {
            if (path.Length > 2 && path[2] == '/')
                return 3;
            return 2;
        }

        if (path.LastIndexOf("file:///", 0, StringComparison.Ordinal) == 0)
            return "file:///".Length;
        var idx = path.IndexOf("://", StringComparison.Ordinal);
        if (idx != -1)
            return idx + "://".Length;
        return 0;
    }

    private static List<string> GetNormalizedParts(string normalizedSlashedPath, int rootLength)
    {
        var parts = normalizedSlashedPath[rootLength..].Split('/');
        List<string> normalized = [];
        foreach (var part in parts)
        {
            if (part == ".")
                continue;
            if (part == ".." && normalized.Count > 0 && normalized.LastOrDefault() != "..")
                normalized.RemoveAt(normalized.Count - 1);
            else if (!string.IsNullOrEmpty(part))
                normalized.Add(part);
        }
        return normalized;
    }

    public static bool FileExtensionIs(string path, string extension)
    {
        return path.EndsWith(extension, StringComparison.Ordinal);
    }

    public static Diagnostic CreateFileDiagnostic(SourceFile file, int start, int length, DiagnosticMessage message, object argument)
    {
        return new Diagnostic
        {
            File = file,
            Start = start,
            Length = length,
            Message = message,
            Argument = argument
        };
    }
}
