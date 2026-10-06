using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;

namespace Serenity.Services.SqlErrors;

/// <summary>
/// Disabled test utility that regenerates
/// <c>DefaultSqlErrorExtractor.SqlServerMessages.cs</c> from a live SQL Server's
/// <c>sys.messages</c>. Enable it (uncomment <c>[Fact]</c>) and run against a SQL
/// Server that has the localized messages installed, then restore the attribute.
/// </summary>
/// <remarks>
/// SQL Server stores the message format template per <c>message_id</c> and
/// <c>language_id</c>. The placeholder numbers are stable across languages
/// (e.g. 547 always has %5 = table, %3 = constraint); only the surrounding literal
/// text differs. This utility turns each template into an anchored regex where the
/// literals are escaped and the placeholders become capture groups named after the
/// field they represent, so table/column/constraint names can be extracted from
/// messages in any language.
/// </remarks>
public class SqlServerMessageRegexGeneratorTests
{
    // [Fact]
    public void SqlServerMessageRegexGenerator()
    {
        var path = Environment.CurrentDirectory.Replace('\\', '/');
        Assert.Contains("/Serenity/", path);
        var serenityRoot = path[0..(path.IndexOf("/Serenity/", StringComparison.Ordinal) + 10)];

        using var connection = SqlIntegrationConnections.CreateConnection("SqlServer");
        connection.Open();

        var rows = ReadMessages(connection);

        var content = Generate(rows);

        var target = System.IO.Path.Combine(serenityRoot, "src", "services", "RequestHandlers",
            "IntegratedFeatures", "SqlErrors", "DefaultSqlErrorExtractor.SqlServerMessages.cs");
        System.IO.File.WriteAllText(target, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
    }

    private static List<(int MessageId, string Language, string Text)> ReadMessages(DbConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT m.message_id, l.name AS language, m.text
            FROM sys.messages m
            JOIN sys.syslanguages l ON l.msglangid = m.language_id
            WHERE m.message_id IN (515, 547, 2601, 2627)
            ORDER BY m.message_id, l.name
            """;

        var rows = new List<(int, string, string)>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
            rows.Add((reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));

        return rows;
    }

    // semantic meaning of each placeholder number per message id
    private static readonly Dictionary<int, Dictionary<int, string>> placeholderMaps = new()
    {
        [515] = new() { [1] = "column", [2] = "table", [3] = "statement" },
        [547] = new()
        {
            [1] = "statement", [2] = "kind", [3] = "constraint", [4] = "database",
            [5] = "table", [6] = "tail1", [7] = "tail2", [8] = "tail3"
        },
        [2601] = new() { [1] = "table", [2] = "constraint", [3] = "value" },
        [2627] = new() { [1] = "kind", [2] = "constraint", [3] = "table", [4] = "value" }
    };

    private static readonly HashSet<string> captureNames = ["table", "column", "constraint", "value"];

    private static readonly Regex placeholderRegex = new("%(?:\\d+!|\\.?\\*?[a-zA-Z]+)");
    private static readonly Regex numberedPlaceholderRegex = new("^%(\\d+)!$");

    private static string Generate(IEnumerable<(int MessageId, string Language, string Text)> rows)
    {
        var sb = new StringBuilder();
        sb.AppendLine("namespace Serenity.Services.SqlErrors;");
        sb.AppendLine();
        sb.AppendLine("public partial class DefaultSqlErrorExtractor");
        sb.AppendLine("{");
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Regexes generated from SQL Server sys.messages for the handled constraint");
        sb.AppendLine("    /// violation message ids (515, 547, 2601, 2627), one per language. Named groups:");
        sb.AppendLine("    /// <c>table</c>, <c>column</c>, <c>constraint</c>, <c>value</c>.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    protected static readonly Regex[] SqlServerMessageRegexes =");
        sb.AppendLine("    [");

        foreach (var (messageId, language, _) in rows)
        {
            sb.Append("        ");
            sb.Append(MethodName(messageId, language));
            sb.Append("(), // ");
            sb.Append(messageId);
            sb.Append(' ');
            sb.Append(SafeComment(language));
            sb.AppendLine();
        }

        sb.AppendLine("    ];");
        sb.AppendLine();

        foreach (var (messageId, language, text) in rows)
        {
            var pattern = BuildPattern(messageId, text);
            sb.Append("    // ");
            sb.Append(messageId);
            sb.Append(' ');
            sb.Append(SafeComment(language));
            sb.AppendLine();
            sb.Append("    [GeneratedRegex(");
            sb.Append(ToCSharpStringLiteral(pattern));
            sb.AppendLine(", RegexOptions.CultureInvariant)]");
            sb.Append("    private static partial Regex ");
            sb.Append(MethodName(messageId, language));
            sb.AppendLine("();");
            sb.AppendLine();
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string MethodName(int messageId, string language) =>
        "SqlServerMessageRegex_" + messageId + "_" + GetLanguageIdentifier(language);

    // maps SQL Server language names to stable English identifiers for method names
    private static readonly Dictionary<string, string> languageIdentifiers = new(StringComparer.OrdinalIgnoreCase)
    {
        ["British"] = "English",
        ["us_english"] = "UsEnglish",
        ["čeština"] = "Czech",
        ["Dansk"] = "Danish",
        ["Deutsch"] = "German",
        ["Español"] = "Spanish",
        ["Français"] = "French",
        ["Italiano"] = "Italian",
        ["magyar"] = "Hungarian",
        ["Nederlands"] = "Dutch",
        ["norsk (bokmål)"] = "Norwegian",
        ["polski"] = "Polish",
        ["Português"] = "Portuguese",
        ["Português (Brasil)"] = "PortugueseBrazil",
        ["Suomi"] = "Finnish",
        ["Svenska"] = "Swedish",
        ["Türkçe"] = "Turkish",
        ["ελληνικά"] = "Greek",
        ["русский"] = "Russian",
        ["한국어"] = "Korean",
        ["日本語"] = "Japanese",
        ["简体中文"] = "ChineseSimplified",
        ["繁體中文"] = "ChineseTraditional"
    };

    private static string GetLanguageIdentifier(string language)
    {
        if (languageIdentifiers.TryGetValue(language, out var identifier))
            return identifier;

        var chars = language
            .Select(ch => char.IsAsciiLetterOrDigit(ch) ? ch : '_')
            .ToArray();
        var sanitized = new string(chars).Trim('_');
        while (sanitized.Contains("__", StringComparison.Ordinal))
            sanitized = sanitized.Replace("__", "_", StringComparison.Ordinal);

        return sanitized.Length > 0 ? sanitized : "Language";
    }

    private static string SafeComment(string value) => value
        .Replace('\\', ' ')
        .Replace('"', '\'')
        .Replace('\r', ' ')
        .Replace('\n', ' ');

    private static string BuildPattern(int messageId, string text)
    {
        var map = placeholderMaps[messageId];
        var pattern = new StringBuilder("^");
        var pos = 0;
        var positional = 0;

        foreach (Match match in placeholderRegex.Matches(text))
        {
            pattern.Append(Regex.Escape(text[pos..match.Index]));

            string? semantic;
            if (numberedPlaceholderRegex.Match(match.Value) is { Success: true } numbered)
                map.TryGetValue(int.Parse(numbered.Groups[1].Value), out semantic);
            else
                map.TryGetValue(++positional, out semantic);

            pattern.Append(semantic is not null && captureNames.Contains(semantic)
                ? $"(?<{semantic}>.*?)"
                : ".*?");

            pos = match.Index + match.Length;
        }

        pattern.Append(Regex.Escape(text[pos..]));
        pattern.Append('$');

        // Regex.Escape escapes literal spaces as "\ "; make the output more readable
        return pattern.ToString().Replace("\\ ", " ");
    }

    private static string ToCSharpStringLiteral(string value) =>
        "\"" + value.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
}
