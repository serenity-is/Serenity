#if IsTemplateBuild || IsProjectWizard

using System;
using System.Collections.Generic;
using System.Linq;

namespace Build;

public static partial class Shared
{
    public static readonly (string prefix, string suffix, string eelse, string ending)[] ConditionalPatterns = new[]
    {
        ("//<if:", "//>", "//<else>", "//</if:{0}>"),
        ("//#if(", ")", "//#else", "//#endif"),
        ("//#if (", ")", "//#else", "//#endif"),
        ("#if(", ")", "#else", "#endif"),
        ("#if (", ")", "#else", "#endif"),
        ("<!--<if:", ">-->", "<!--<else>-->", "<!--<endif:{0}>-->"),
        ("<!--#if(", ")-->", "<!--#else-->", "<!--#endif-->"),
        ("<!--#if (", ")-->", "<!--#else-->", "<!--#endif-->")
    };

    public static bool HasConditionals(string content)
    {
        var lines = (content ?? "").Replace("\r", "").Split('\n').ToList();
        return lines.Any(line =>
        {
            line = line.TrimStart();
            return ConditionalPatterns.Any(pattern => line.StartsWith(pattern.prefix, StringComparison.Ordinal));
        });
    }

    public static string PreprocessConditionals(string content, ISet<string> enabledFeatures)
    {
        var lineEnding = DetectLineEnding(content) ?? "\n";
        var lines = content.Replace("\r", "").Split('\n').ToList();

        int lastStart = 0;
        while (true)
        {
            var found = false;
            foreach (var pattern in ConditionalPatterns)
            {
                var start = lines.FindIndex(lastStart,
                    l => l.TrimStart().StartsWith(pattern.prefix));

                if (start < 0)
                    continue;

                found = true;
                lastStart = start;

                var line = lines[start];
                lines.RemoveAt(start);

                string feature;
                int end;
                int eelse;
                line = line.TrimStart()[pattern.prefix.Length..];

                var endidx = line.LastIndexOf(pattern.suffix);
                if (endidx < 0)
                    break;

                feature = line[..endidx].Trim();
                end = lines.FindIndex(start, x => x.Trim().StartsWith(string.Format(pattern.ending, feature)));
                if (end < 0)
                    break;

                eelse = lines.FindIndex(start, x => x.Trim() == pattern.eelse);
                if (eelse > end)
                    eelse = -1;

                lines.RemoveAt(end);

                if (enabledFeatures.Contains(feature))
                {
                    if (eelse >= 0)
                    {
                        for (var l = eelse; l < end; l++)
                            lines.RemoveAt(eelse);
                    }

                    continue;
                }

                var z = end;
                if (eelse >= 0)
                {
                    bool commentedElse = pattern.ending.Contains("{0}", StringComparison.CurrentCulture);

                    lines.RemoveAt(eelse);
                    z = eelse;
                    for (var l = eelse; l < end - 1; l++)
                    {
                        var e = lines[l];
                        if (pattern.prefix.StartsWith('<'))
                        {
                            var cidx = e.IndexOf("<!--");
                            if (commentedElse && cidx >= 0)
                            {
                                e = e[..cidx] + e[(cidx + 4)..];
                                cidx = e.LastIndexOf("-->");
                                if (cidx >= 0)
                                    e = e[..cidx];
                                lines[l] = e;
                            }
                        }
                        else
                        {
                            var cidx = e.IndexOf("//");
                            if (commentedElse && cidx >= 0)
                                lines[l] = e[..cidx] + e[(cidx + 2)..];
                        }
                    }
                }

                for (var l = start; l < z; l++)
                    lines.RemoveAt(start);
            }

            if (!found)
                break;
        }

        return string.Join(lineEnding, lines);
    }
}
#endif