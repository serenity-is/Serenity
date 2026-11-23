using System.Globalization;

namespace Serenity.CodeGeneration;

/// <summary>
/// Writes the constants of the local text keys of a project (TextKeys.g.cs), so that a text key can be referred to
/// by a constant that the compiler checks, e.g. <c>TextKeys.Db.Northwind.Customer.Fax</c>, rather than by a string
/// that is hard to know and easy to mistype, see <see cref="LocalTextKeyRules"/> for the keys of an entity.
/// </summary>
/// <remarks>
/// The segments of the keys become the nested classes and the constants of the same names, in the shape of the keys,
/// e.g. the key "Db.Northwind.Customer.Fax_Placeholder" is the constant
/// <c>TextKeys.Db.Northwind.Customer.Fax_Placeholder</c>, whose value is the key itself. A segment that is not an
/// identifier gets the closest one (<see cref="LocalTextKeyRules.ToIdentifier"/>), and its first letter capitalized,
/// e.g. "fax" to "Fax"; the words of a segment that are separated by characters that an identifier can not have are
/// capitalized as well, e.g. "Filtered Lookup in Detail" to "FilteredLookupInDetail"; and the slashes of a key of the
/// navigation texts, which is the path of a page, are the separators of its segments, e.g.
/// "Navigation.Basic Samples/Editors/Filtered Lookup in Detail" is <c>TextKeys.Navigation.BasicSamples.Editors</c>.
/// A key whose name another key already has is left out, and the class notes it in a comment of its own, as a key that
/// is not written is better than one that is written wrong, and a comment tells why, see
/// <see cref="TextKeysResult.Diagnostics"/>.
/// </remarks>
public static class TextKeysWriter
{
    /// <summary>
    /// The name of the constant that carries the key of a class itself, e.g. the title of a page that has pages under
    /// it, whose own name would be the name of a class otherwise.
    /// </summary>
    public const string TextKeyName = "TextKey";

    /// <summary>
    /// Writes the constants of the specified keys.
    /// </summary>
    /// <param name="keys">The keys, e.g. "Db.Northwind.Customer.Fax".</param>
    /// <param name="rootNamespace">The namespace of the constants, or null for the global one.</param>
    /// <param name="className">The name of the outermost class of the constants.</param>
    /// <param name="texts">The invariant text of the keys, which the summary of their constants shows, if known.</param>
    /// <param name="fileScopedNamespaces">True to write the namespace of the file as a file scoped one, which is what
    /// the rest of the generated files of the project use, unless it says otherwise.</param>
    /// <param name="nullableDirective">The value of the nullable directive of the file, e.g. "enable", or null for none.</param>
    /// <param name="internalAccess">True to write the outermost class as an internal one, as the other classes that are
    /// generated for a project are, e.g. the one of MVC, when a project is not for others to use its types.</param>
    /// <param name="alias">True to make the class available as TextKeys in every file of the project, with a global
    /// using, as its namespace is not the one of the project but one under it, e.g. "MyApp.MVC", which a project would
    /// write out in each of its files otherwise.</param>
    /// <returns>The source of the constants, which is empty when there are no keys, and their diagnostics.</returns>
    public static TextKeysResult Write(IEnumerable<string> keys, string? rootNamespace = null,
        string className = "TextKeys", IReadOnlyDictionary<string, string?>? texts = null,
        bool fileScopedNamespaces = false, string? nullableDirective = null, bool internalAccess = false,
        bool alias = true)
    {
        ArgumentNullException.ThrowIfNull(keys);
        if (string.IsNullOrEmpty(className))
            throw new ArgumentException("The class name can not be empty.", nameof(className));

        var diagnostics = new List<TextKeyDiagnostic>();
        var root = new Node();
        var seenKeys = new HashSet<string>(StringComparer.Ordinal);
        var count = 0;

        foreach (var key in keys)
        {
            if (string.IsNullOrEmpty(key) || !seenKeys.Add(key))
                continue;

            var segments = LocalTextKeyRules.SplitKey(key);
            var node = root;
            for (var i = 0; i < segments.Length; i++)
            {
                if (LocalTextKeyRules.ToIdentifier(segments[i], out _) is null)
                {
                    root.Skipped.Add((key!, "no identifier for \"" + segments[i] + "\""));
                    diagnostics.Add(new TextKeyDiagnostic(TextKeyDiagnostic.UnmapableSegment,
                        $"The key \"{key}\" has no constant of its own, as its segment \"{segments[i]}\" " +
                        "has no characters that an identifier can have."));
                    break;
                }

                if (i == segments.Length - 1)
                {
                    node.Members.Add(new Member(segments[i], key!));
                    count++;
                }
                else
                {
                    if (!node.Children.TryGetValue(segments[i], out var child))
                        node.Children[segments[i]] = child = new Node();

                    node = child;
                }
            }
        }

        if (count == 0)
            return new TextKeysResult("", diagnostics, 0);

        ResolveNames(root, "", className, diagnostics);

        var cw = new CodeWriter(new StringBuilder(4096), 4)
        {
            AutoGeneratedComment = true,
            FileScopedNamespaces = fileScopedNamespaces,
            IsCSharp = true,
            NullableDirective = nullableDirective
        };
        // the class is in a namespace under the one of the project, so a file of it writes the alias to reach it
        if (alias && !string.IsNullOrEmpty(rootNamespace))
        {
            cw.AppendLine($"global using {className} = {rootNamespace}.{className};");
            cw.AppendLine();
        }

        cw.InNamespace(rootNamespace, () => WriteClass(cw, root, className, null, texts, internalAccess));
        return new TextKeysResult(cw.ToString(), diagnostics, Written(root));
    }

    private sealed class Node
    {
        public readonly Dictionary<string, Node> Children = new(StringComparer.Ordinal);
        public readonly List<Member> Members = [];
        public readonly Dictionary<string, string> Names = new(StringComparer.Ordinal);
        /// <summary>The keys under this class that are not written, with the key or the segment that they lost to.</summary>
        public readonly List<(string Key, string Reason)> Skipped = [];
        /// <summary>The keys of this class itself, e.g. the title of a page that has pages under it, which are written
        /// as TextKey, or with an underscore at their segment's end when that one is taken, see ResolveNames.</summary>
        public readonly List<Member> NodeTexts = [];
    }

    private sealed class Member(string segment, string key)
    {
        public string Segment { get; } = segment;
        public string Key { get; } = key;
        public string? Name { get; set; }
    }

    /// <summary>
    /// Counts the constants that the classes of a node carry, which are the ones of the keys that it kept, as a key that
    /// another one took the name of has none, and the ones under a class that has no name of its own are not written.
    /// </summary>
    private static int Written(Node node)
    {
        var count = node.Members.Count(x => x.Name is not null) + node.NodeTexts.Count(x => x.Name is not null);
        foreach (var child in node.Children)
        {
            // a class that lost its name is not written, and neither are the keys under it
            if (node.Names.ContainsKey(child.Key))
                count += Written(child.Value);
        }

        return count;
    }

    /// <summary>
    /// Finds the segment of the keys of a class of a node by the name that it was given, which can differ from the
    /// segment in case, e.g. "Fax" for the keys of "fax", as the names are case sensitive and the first one wins.
    /// </summary>
    private static string? SegmentOfName(Node node, string name)
    {
        foreach (var child in node.Children)
        {
            if (node.Names.TryGetValue(child.Key, out var childName) &&
                string.Equals(childName, name, StringComparison.Ordinal))
                return child.Key;
        }

        return null;
    }

    private static string Identifier(string segment) => LocalTextKeyRules.ToIdentifier(segment, out _)!;

    private static string Name(string segment) => LocalTextKeyRules.Pascalize(Identifier(segment));

    /// <summary>
    /// An identifier that is already the preferred one goes first, e.g. "Fax" before "fax", then the ones that only
    /// need a capital letter, and the mangled ones last, so that the plain key keeps the plain name.
    /// </summary>
    private static int Priority(string segment)
    {
        var identifier = Identifier(segment);
        if (identifier != segment)
            return 2;

        return LocalTextKeyRules.Pascalize(identifier) == identifier ? 0 : 1;
    }

    private static IEnumerable<string> InOrder(IEnumerable<string> segments)
    {
        return segments.Distinct(StringComparer.Ordinal)
            .OrderBy(Priority)
            .ThenBy(segment => segment, StringComparer.Ordinal);
    }

    private static void ResolveNames(Node node, string keyPrefix, string? className,
        List<TextKeyDiagnostic> diagnostics)
    {
        var taken = new HashSet<string>(StringComparer.Ordinal);
        var classNames = new HashSet<string>(StringComparer.Ordinal);
        var owners = new Dictionary<string, string>(StringComparer.Ordinal);
        var skippedChildren = new HashSet<string>(StringComparer.Ordinal);

        // the classes of the keys that go on are named first, as the keys under a key need the class that it becomes
        foreach (var segment in InOrder(node.Children.Keys))
        {
            var name = Name(segment);

            // a class can not have the name of the class that it is in, e.g. the keys "Db.Pro.DataAuditLog.DataAuditLog"
            // of a row that is named after its own module, nor the name of a class next to it, and an underscore at its
            // end is what tells them apart, while it is free, as it is for the views of MVC
            if (name == className || taken.Contains(name))
            {
                var underscored = name + "_";
                if (taken.Contains(underscored) || underscored == className)
                {
                    skippedChildren.Add(segment);
                    node.Skipped.Add((keyPrefix + segment + ".*", owners[name]));
                    diagnostics.Add(new TextKeyDiagnostic(TextKeyDiagnostic.NameTaken,
                        $"The keys \"{keyPrefix}{segment}.*\" have no constants, as the name \"{name}\" " +
                        $"is the name of \"{owners[name]}\"."));
                    continue;
                }

                name = underscored;
            }

            taken.Add(name);
            owners[name] = keyPrefix + segment + ".*";
            classNames.Add(name);
            node.Names[segment] = name;
        }

        // the keys that become the text of a class of their own are written there, not where they are read from
        var moved = new List<Member>();

        foreach (var member in node.Members
            .OrderBy(x => Priority(x.Segment))
            .ThenBy(x => x.Segment, StringComparer.Ordinal)
            .ThenBy(x => x.Key, StringComparer.Ordinal))
        {
            var name = Name(member.Segment);
            var identifier = Identifier(member.Segment);

            // the keys are case sensitive, so a key that differs only in case from a class of the same class keeps its
            // own name, e.g. "Db.fax" next to the keys "Db.Fax.Y", which is no clash at all
            if (name != identifier && !taken.Contains(identifier) && !classNames.Contains(identifier) &&
                (name == className || classNames.Contains(name)))
                name = identifier;

            // a constant can not have the name of a class of the same class, e.g. the key "Db.Fax" next to the keys
            // "Db.Fax.Y", nor the name of the class that it is in, e.g. "Controls.Dialog.Dialog", so it becomes the
            // text of that class, which is what the key is: its TextKey, see below
            if (name == className || classNames.Contains(name))
            {
                if (name == className)
                {
                    node.NodeTexts.Add(member);
                }
                else
                {
                    // the class is the one that the name was given to, which is the segment of its keys, not the
                    // segment of this key, as the two can differ in case, e.g. "fax" and "Fax"
                    var segment = SegmentOfName(node, name);
                    if (segment is null)
                    {
                        node.Skipped.Add((member.Key, owners[name]));
                        diagnostics.Add(new TextKeyDiagnostic(TextKeyDiagnostic.NameTaken,
                            $"The key \"{member.Key}\" has no constant, as its name \"{name}\" " +
                            $"is the name of \"{owners[name]}\"."));
                        continue;
                    }

                    node.Children[segment].NodeTexts.Add(member);
                    moved.Add(member);
                }

                continue;
            }

            if (taken.Contains(name))
            {
                // the keys are case sensitive, so a key that differs only in case from the one that has the name keeps
                // the name of its own segment, e.g. "fax" next to "Fax", which is no clash at all
                if (!taken.Contains(Identifier(member.Segment)))
                {
                    name = Identifier(member.Segment);
                }
                else
                {
                    // what is left can not be resolved, e.g. "x y" and "x-y", which are both "XY", next to an "XY"
                    node.Skipped.Add((member.Key, owners[name]));
                    diagnostics.Add(new TextKeyDiagnostic(TextKeyDiagnostic.NameTaken,
                        $"The key \"{member.Key}\" has no constant, as its name \"{name}\" " +
                        $"is the name of \"{owners[name]}\"."));
                    continue;
                }
            }

            taken.Add(name);
            owners[name] = member.Key;
            member.Name = name;
        }

        foreach (var member in moved)
            node.Members.Remove(member);

        // the keys of the class itself, e.g. the title of a page that has pages under it, which is written as TextKey
        // while that one is free, and as its own name with an underscore at its end while that one is free, e.g. the
        // key "Controls.Dialogs.Dialogs" next to "Controls.Dialogs", both of which are the text of the Dialogs class
        foreach (var text in node.NodeTexts.OrderBy(x => x.Key, StringComparer.Ordinal))
        {
            var name = TextKeyName;
            if (taken.Contains(name) || classNames.Contains(name))
                name = Name(text.Segment) + "_";

            if (taken.Contains(name) || classNames.Contains(name) || name == className)
            {
                var owner = owners.TryGetValue(name, out var found) ? found : keyPrefix + name + ".*";
                node.Skipped.Add((text.Key, owner));
                diagnostics.Add(new TextKeyDiagnostic(TextKeyDiagnostic.NameTaken,
                    $"The key \"{text.Key}\" has no constant, as the name \"{name}\" " +
                    $"is the name of \"{owner}\"."));
                continue;
            }

            taken.Add(name);
            owners[name] = text.Key;
            text.Name = name;
        }

        foreach (var child in node.Children)
            if (!skippedChildren.Contains(child.Key))
                ResolveNames(child.Value, keyPrefix + child.Key + ".", node.Names[child.Key], diagnostics);
    }

    private static void WriteClass(CodeWriter cw, Node node, string className, string? keyPath,
        IReadOnlyDictionary<string, string?>? texts, bool internalAccess)
    {
        // the summary names the keys that the class holds, not the names of the classes that they are in, which can
        // differ from them, e.g. "DataAuditLog_" for the keys of "Db.Pro.DataAuditLog.DataAuditLog"
        cw.IndentedLine(keyPath is null ?
            "/// <summary>The local text keys of the project, a constant for each of their segments.</summary>" :
            $"/// <summary>The text keys under \"{EscapeXml(keyPath)}\".</summary>");
        // the classes under the outermost one are public, as the one that holds them is the one that a project hides
        cw.IndentedLine($"{(keyPath is null && internalAccess ? "internal" : "public")} static partial class {className}");
        cw.InBrace(() =>
        {
            var children = node.Children
                .Where(x => node.Names.ContainsKey(x.Key) && HasContent(x.Value))
                .Select(x => (Raw: x.Key, Node: x.Value, Name: node.Names[x.Key]))
                .OrderBy(x => x.Name, StringComparer.Ordinal)
                .ToList();
            // the keys of the class itself are written before the rest, see below
            var members = node.Members
                .Where(x => x.Name is not null && !node.NodeTexts.Contains(x))
                .OrderBy(x => x.Name, StringComparer.Ordinal)
                .ToList();

            var nodeTexts = node.NodeTexts
                .Where(x => x.Name is not null)
                .OrderBy(x => x.Name, StringComparer.Ordinal)
                .ToList();
            foreach (var text in nodeTexts)
            {
                cw.IndentedLine($"/// <summary>{Summary(text.Key, texts)}</summary>");
                cw.IndentedLine($"public const string {text.Name} = \"{EscapeLiteral(text.Key)}\";");
            }

            if (nodeTexts.Count > 0 && (children.Count > 0 || members.Count > 0))
                cw.AppendLine();

            for (var i = 0; i < children.Count; i++)
            {
                if (i > 0)
                    cw.AppendLine();

                var child = children[i];
                var childPath = keyPath is null ? Name(child.Raw) : keyPath + "." + Name(child.Raw);
                WriteClass(cw, child.Node, child.Name, childPath, texts, internalAccess);
            }

            if (children.Count > 0 && (members.Count > 0 || node.Skipped.Count > 0))
                cw.AppendLine();

            foreach (var member in members)
            {
                cw.IndentedLine($"/// <summary>{Summary(member.Key, texts)}</summary>");
                cw.IndentedLine($"public const string {member.Name} = \"{EscapeLiteral(member.Key)}\";");
            }

            if (node.Skipped.Count > 0)
            {
                if (members.Count > 0)
                    cw.AppendLine();

                cw.IndentedLine("// Not written, as they could not keep a name of their own:");
                foreach (var (key, reason) in node.Skipped)
                    cw.IndentedLine($"//   {key} -> {reason}");
            }
        });
    }

    /// <summary>
    /// Whether a class holds a constant or a note to write, as one that holds neither is not written at all.
    /// </summary>
    private static bool HasContent(Node node)
    {
        return node.Skipped.Count > 0 || node.Members.Any(x => x.Name is not null) ||
            node.NodeTexts.Any(x => x.Name is not null) ||
            node.Children.Any(x => node.Names.ContainsKey(x.Key) && HasContent(x.Value));
    }

    /// <summary>
    /// The summary of the constant of a key, which is the invariant text of the key, for the hint that the editor
    /// shows, as the name of the constant does not need it to know which key it is. A key that has no text, e.g. one
    /// that the template of the text resources of the project is missing, or that it leaves for a translation, has ""
    /// as its summary, which is a summary that is not empty, as an empty one is a documentation error.
    /// </summary>
    private static string Summary(string key, IReadOnlyDictionary<string, string?>? texts)
    {
        var text = texts is not null && texts.TryGetValue(key, out var value) ? value : null;
        if (string.IsNullOrWhiteSpace(text))
            return "\"\"";

        return EscapeXml(OneLine(text));
    }

    /// <summary>
    /// The text of a doc comment on a single line, as the lines of a summary that are in one line of source would break
    /// the comment that they are in.
    /// </summary>
    private static string OneLine(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return "";

        return string.Join(" ", text!.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim()).Where(x => x.Length > 0));
    }

    private static string EscapeXml(string text)
    {
        return text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
    }

    private static string EscapeLiteral(string text)
    {
        var sb = new StringBuilder(text.Length);
        foreach (var c in text)
        {
            switch (c)
            {
                case '\\': sb.Append("\\\\"); break;
                case '"': sb.Append("\\\""); break;
                case '\r': sb.Append("\\r"); break;
                case '\n': sb.Append("\\n"); break;
                case '\t': sb.Append("\\t"); break;
                default: sb.Append(c); break;
            }
        }

        return sb.ToString();
    }
}
