#nullable enable
namespace Serenity.Reflection;

/// <summary>
/// Used to write formatted code to a string builder.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CodeWriter"/> class.
/// </remarks>
/// <param name="sb">The sb.</param>
/// <param name="tabSize">Number of spaces.</param>
public class CodeWriter(StringBuilder sb, int tabSize)
{
    private readonly StringBuilder sb = sb ?? throw new ArgumentNullException(nameof(sb));
    private readonly string tab = new(' ', tabSize);
    private string indent = "";

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeWriter"/> class.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="tab">Tab string (e.g. \9 or '  ').</param>
    public CodeWriter(StringBuilder sb, string tab)
        : this(sb, 1)
    {
        this.tab = tab ?? throw new ArgumentNullException(nameof(tab));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeWriter"/> class.
    /// </summary>
    /// <param name="tabSize">Number of spaces.</param>
    public CodeWriter(int tabSize = 4)
        : this(new StringBuilder(), tabSize)
    {
    }

    /// <summary>
    /// Increases indenting using tab string
    /// </summary>
    public void IncreaseIndent()
    {
        indent += tab;
    }

    /// <summary>
    /// Decreases indenting by tab size
    /// </summary>
    public void DecreaseIndent()
    {
        if (indent.Length >= tab.Length)
            indent = indent[..^tab.Length];
    }

    /// <summary>
    /// Increases indent, runs the insideBlock and decreases indent back.
    /// </summary>
    /// <param name="insideBlock">The inside block.</param>
    public void Block(Action insideBlock)
    {
        if (insideBlock is null)
            throw new ArgumentNullException(nameof(insideBlock));

        IncreaseIndent();
        insideBlock();
        DecreaseIndent();
    }

    /// <summary>
    /// Adds a brace, increases indent, runs the inside block, decreases indent back, closes the brace.
    /// </summary>
    /// <param name="insideBlock">The inside block.</param>
    /// <param name="endLine">If should end the line</param>
    public void InBrace(Action insideBlock, bool endLine = true)
    {
        if (insideBlock is null)
            throw new ArgumentNullException(nameof(insideBlock));

        StartBrace();
        insideBlock();
        EndBrace(endLine);
    }

    /// <summary>
    /// Adds a brace, increases indent.
    /// </summary>
    public void StartBrace()
    {
        if (!BraceOnSameLine)
            sb.Append(indent);
        else
            sb.Append(" ");
        sb.AppendLine("{");
        IncreaseIndent();
    }

    /// <summary>
    /// Decreases indent and closes the brace
    /// </summary>
    public void EndBrace(bool endLine = true)
    {
        DecreaseIndent();
        sb.Append(indent);
        if (endLine)
            sb.AppendLine("}");
        else
            sb.Append('}');
    }

    /// <summary>
    /// Appends current indent
    /// </summary>
    public void Indent()
    {
        sb.Append(indent);
    }

    /// <summary>
    /// Appends current indent and the specified string
    /// </summary>
    /// <param name="s">The string.</param>
    public void Indented(string s)
    {
        sb.Append(indent);
        sb.Append(s);
    }

    /// <summary>
    /// Appends current indent, the specified string, and a new line.
    /// </summary>
    /// <param name="s">The s.</param>
    public void IndentedLine(string s)
    {
        sb.Append(indent);
        sb.AppendLine(s);
    }

    /// <summary>
    /// Appends each of the text's lines with current indent, and a new line.
    /// </summary>
    /// <param name="code">The code</param>
    public void IndentedMultiLine(string code)
    {
        if (string.IsNullOrEmpty(code))
            return;

        foreach (var line in code.Replace("\r", "")
            .Split('\n'))
        {
            if (line.Length > 0)
                sb.Append(indent);

            sb.AppendLine(line);
        }
    }

    /// <summary>
    /// Executes action by opening namespace if it is not null or empty
    /// </summary>
    /// <param name="ns">Namespace</param>
    /// <param name="action">Action</param>
    public void InNamespace(string ns, Action action)
    {
        var oldNamespace = CurrentNamespace;
        CurrentNamespace = ns;
        if (!string.IsNullOrEmpty(ns))
        {
            Indented("namespace ");
            if (FileScopedNamespaces && IsCSharp)
            {
                sb.Append(ns);
                sb.AppendLine(";");
                sb.AppendLine();
                action();
            }
            else
            {
                sb.AppendLine(ns);
                InBrace(action);
            }
        }
        else
        {
            action();
        }
        CurrentNamespace = oldNamespace;
    }

    /// <summary>
    /// Gets sets function that determines if a namespace is allowed to be added to the local usings
    /// </summary>
    public Func<string, bool> AllowUsing { get; set; } = (ns) => true;

    /// <summary>
    /// Whether to put opening brace on the same line.
    /// </summary>
    /// <value>
    ///   <c>true</c> if brace on same line; otherwise, <c>false</c>.
    /// </value>
    public bool BraceOnSameLine { get; set; }

    /// <summary>
    /// Use a file scoped namespace instead. Can only
    /// be used with one namespace per file.
    /// </summary>
    public bool FileScopedNamespaces { get; set; }

    /// <summary>
    /// Gets internal string builder
    /// </summary>
    public StringBuilder Builder => sb;

    /// <summary>
    /// Gets / sets current namespace
    /// </summary>
    public string? CurrentNamespace { get; set; }

    /// <summary>
    /// Gets / sets file comment
    /// </summary>
    public string? FileComment { get; set; }

    /// <summary>
    /// Gets / sets global usings hash set
    /// </summary>
    public HashSet<string>? GlobalUsings { get; set; }

    /// <summary>
    /// Gets / sets if the code writer is used for generating C# code.
    /// </summary>
    public bool IsCSharp { get; set; }

    /// <summary>
    /// Gets / sets local usings hash set
    /// </summary>
    public HashSet<string>? LocalUsings { get; private set; }

    /// <summary>
    /// Gets tab string
    /// </summary>
    public string Tab => tab;

    /// <summary>
    /// Gets current indentation string
    /// </summary>
    public string Indentation => indent;

    /// <summary>
    /// Returns true if the namespace is in list of usings.
    /// </summary>
    /// <param name="ns">The namespace to check.</param>
    /// <returns>True if the namespace is in the list of usings; otherwise, false.</returns>
    public bool IsUsing(string ns)
    {
        if (ns == CurrentNamespace)
            return true;

        if (GlobalUsings != null && GlobalUsings.Contains(ns))
            return true;

        return LocalUsings != null && LocalUsings.Contains(ns);
    }

    /// <summary>
    /// Returns true if the namespace is in the list of usings.
    /// If the AllowUsing callback is null or returns true, or force is true,
    /// this may add it to the list of local usings.
    /// </summary>
    /// <param name="ns">The namespace to check.</param>
    /// <param name="force"><c>true</c> to add the namespace to the local usings regardless of the AllowUsing callback.</param>
    /// <returns><c>true</c> if the namespace is in the list of usings; otherwise, <c>false</c>.</returns>
    public bool Using(string ns, bool force)
    {
        if (IsUsing(ns))
            return true;

        if (force || AllowUsing is null || AllowUsing(ns))
        {
            LocalUsings ??= [];
            LocalUsings.Add(ns);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Returns true if the namespace is in the list of usings.
    /// If the AllowUsing callback is null or returns true,
    /// this may add it to the list of local usings.
    /// </summary>
    /// <param name="ns">The namespace to check.</param>
    /// <returns><c>true</c> if the namespace is in the list of usings; otherwise, <c>false</c>.</returns>
    public bool Using(string ns)
    {
        return Using(ns, false);
    }

    /// <summary>
    /// Appends a line to internal string builder
    /// </summary>
    /// <returns>String builder</returns>
    public StringBuilder AppendLine()
    {
        return sb.AppendLine();
    }

    /// <summary>
    /// Appends a line to internal string builder
    /// </summary>
    /// <param name="text">Text</param>
    /// <returns>String builder</returns>
    public StringBuilder AppendLine(string text)
    {
        return sb.AppendLine(text);
    }

    /// <summary>
    /// Appends text to internal string builder
    /// </summary>
    /// <param name="text">Text</param>
    /// <returns>String builder</returns>
    public StringBuilder Append(string text)
    {
        return sb.Append(text);
    }

    /// <summary>
    /// Inserts string to internal string builder
    /// </summary>
    /// <param name="text">The text to insert.</param>
    /// <param name="index">The zero-based index at which to insert the text.</param>
    public StringBuilder Insert(int index, string text)
    {
        return sb.Insert(index, text);
    }

    /// <summary>
    /// Returns the short type name for the given namespace and type name, adding the namespace
    /// to the local usings if possible.
    /// </summary>
    /// <param name="ns">The namespace.</param>
    /// <param name="typeName">The type name.</param>
    /// <returns>The type name alone if the namespace can be used, otherwise the fully qualified name.</returns>
    public string ShortTypeName(string ns, string typeName)
    {
        if (string.IsNullOrEmpty(typeName))
            return string.Empty;

        if (string.IsNullOrEmpty(ns))
            return typeName;

        if (Using(ns))
            return typeName;
        else if (CurrentNamespace != null)
        {
            var idx = CurrentNamespace.IndexOf('.', StringComparison.Ordinal);
            if (idx >= 0 && ns.StartsWith(CurrentNamespace[..(idx + 1)], StringComparison.Ordinal))
                ns = ns[(idx + 1)..];
        }
        return ns + "." + typeName;
    }

    /// <summary>
    /// Returns the short type name for the given fully qualified name, adding the namespace
    /// to the local usings if possible.
    /// </summary>
    /// <param name="fullName">The fully qualified type name.</param>
    /// <returns>The type name alone if the namespace can be used, otherwise the fully qualified name.</returns>
    public string ShortTypeName(string fullName)
    {
        var idx = fullName.LastIndexOf('.');
        if (idx < 0)
            return fullName;

        return ShortTypeName(fullName[..idx], fullName[(idx + 1)..]);
    }

    /// <summary>
    /// Appends a char to internal string builder
    /// </summary>
    /// <param name="c">Char</param>
    /// <returns>String builder</returns>
    public StringBuilder Append(char c)
    {
        return sb.Append(c);
    }

    /// <summary>
    /// Determines whether the given type name is a C# primitive keyword.
    /// </summary>
    /// <param name="dataType">The type name to check.</param>
    /// <returns><c>true</c> if the type name is a C# primitive keyword; otherwise, <c>false</c>.</returns>
    public static bool IsCSKeyword(string dataType)
    {
        return dataType switch
        {
            "string" => true,
            "bool" => true,
            "byte" => true,
            "char" => true,
            "decimal" => true,
            "double" => true,
            "short" => true,
            "int" => true,
            "long" => true,
            "object" => true,
            "sbyte" => true,
            "float" => true,
            "ushort" => true,
            "uint" => true,
            "ulong" => true,
            _ => false
        };
    }

    /// <summary>
    /// Returns <c>true</c> if the identifier is a reserved JavaScript keyword.
    /// </summary>
    /// <param name="identifier">The identifier to check.</param>
    /// <returns><c>true</c> if the identifier is a reserved JavaScript keyword; otherwise, <c>false</c>.</returns>
    public static bool IsJSKeyword(string identifier)
    {
        return JsReserved.Contains(identifier);
    }

    private static readonly HashSet<string> JsReserved = [
        "abstract",
        "arguments",
        "as",
        "async",
        "await",
        "boolean",
        "break",
        "case",
        "catch",
        "class",
        "const",
        "continue",
        "debugger",
        "default",
        "delete",
        "do",
        "double",
        "else",
        "enum",
        "eval",
        "export",
        "extends",
        "false",
        "final",
        "finally",
        "float",
        "for",
        "function",
        "get",
        "goto",
        "if",
        "implements",
        "import",
        "in",
        "instanceof",
        "int",
        "interface",
        "let",
        "long",
        "native",
        "new",
        "null",
        "of",
        "package",
        "private",
        "protected",
        "public",
        "return",
        "set",
        "short",
        "static",
        "super",
        "switch",
        "synchronized",
        "this",
        "throw",
        "throws",
        "transient",
        "true",
        "try",
        "typeof",
        "var",
        "void",
        "volatile",
        "while",
        "with",
        "yield"
    ];


    /// <summary>
    /// Converts a primitive class name to its C# keyword. If the given class is not a primitive class, returns <c>null</c>.
    /// </summary>
    /// <param name="dataType">The class name to convert.</param>
    /// <returns>The corresponding C# keyword, or <c>null</c> if the class is not a primitive.</returns>
    public static string? ToCSKeyword(string dataType)
    {
        return dataType switch
        {
            "String" => "string",
            "Boolean" => "bool",
            "Byte" => "byte",
            "Char" => "char",
            "Decimal" => "decimal",
            "Double" => "double",
            "Int16" => "short",
            "Int32" => "int",
            "Int64" => "long",
            "Object" => "object",
            "SByte" => "sbyte",
            "Single" => "float",
            "UInt16" => "ushort",
            "UInt32" => "uint",
            "UInt64" => "ulong",
            _ => null
        };
    }

    /// <summary>
    /// Converts a data type with a namespace to a data type without the namespace if its namespace
    /// is in the allowed usings, otherwise returns the full name.
    /// This can handle nullables, C# keywords, and generics to some extent.
    /// See <see cref="IsCSharp"/> if you are using this for C#.
    /// </summary>
    /// <param name="fullName">The full name of the class.</param>
    /// <returns>The short type reference, or the full name if the namespace cannot be used.</returns>
    public string ShortTypeRef(string fullName)
    {
        fullName = fullName.Trim();

        if (string.IsNullOrEmpty(fullName))
            return string.Empty;

        var nullableText = "";
        if (fullName.EndsWith("?"))
        {
            fullName = fullName[..^1];
            nullableText = "?";
        }

        if (!IsCSharp)
            return ShortTypeName(fullName) + nullableText;

        if (IsCSKeyword(fullName))
            return fullName + nullableText;

        if (fullName.IndexOf('.', StringComparison.OrdinalIgnoreCase) < 0)
        {
            if (fullName == "Stream")
                fullName = "System.IO.Stream";
            else
            {
                var type = Type.GetType("System." + fullName);

                if (type != null)
                {
                    fullName = type.FullName;
                }
                else
                    return fullName + nullableText;
            }
        }

        if (fullName.EndsWith(">"))
        {
            var idx = fullName.IndexOf('<', StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
                return ShortTypeName(fullName[..idx]) + '<' + ShortTypeRef(fullName[(idx + 1)..^1]) + '>' + nullableText;
        }

        return ShortTypeName(fullName) + nullableText;
    }

    /// <summary>
    /// Returns string representation of internal string builder, 
    /// including file comment and any local usings if any
    /// </summary>
    /// <returns>String representation of generated code</returns>
    public override string ToString()
    {
        var nsb = new StringBuilder();

        if (AutoGeneratedComment)
        {
            nsb.AppendLine(AutoGeneratedCommentText);
            nsb.AppendLine();
        }

        if (!string.IsNullOrEmpty(FileComment))
        {
            nsb.AppendLine(FileComment);
            nsb.AppendLine();
        }

        if (PragmaSuppressions.Any())
        {
            nsb.AppendLine("#pragma warning disable " + string.Join(", ",
                PragmaSuppressions.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).Select(x => x)));
            
            nsb.AppendLine();
        }

        if (LocalUsings != null &&
            LocalUsings.Any())
        {
            nsb.AppendLine(string.Join(Environment.NewLine,
                LocalUsings
                    .OrderBy(x => x.Contains("="))
                    .ThenBy(x => x, StringComparer.OrdinalIgnoreCase)
                    .Select(x => "using " + x + ";")));
            
            nsb.AppendLine();
        }

        nsb.Append(sb);

        if (PragmaSuppressions.Any())
        {
            if (nsb[^1] != '\n')
                nsb.AppendLine();

            nsb.AppendLine();
            nsb.AppendLine("#pragma warning restore " + string.Join(", ",
                PragmaSuppressions.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).Select(x => x)));
        }

        return nsb.ToString().TrimEnd();
    }

    /// <summary>
    /// Gets or sets a value indicating whether to add an auto-generated comment at the top of the generated code.
    /// </summary>
    public bool AutoGeneratedComment { get; set; }
    
    /// <summary>
    /// Gets or sets the text of the auto-generated comment to be added at the top of the generated code. The default value is "// <auto-generated />".
    /// </summary>
    public string AutoGeneratedCommentText { get; set; } = "// <auto-generated />";

    /// <summary>
    /// Gets the set of pragma warning suppressions to be added at the top of the generated code.
    /// </summary>
    public ISet<string> PragmaSuppressions { get; } = new HashSet<string>();

    /// <summary>
    /// Gets or sets a value indicating whether to suppress the CS1591 warning for missing XML comments in the generated code. When set to true, the CS1591 warning will be added to the set of pragma warning suppressions; when set to false, it will be removed from the set of pragma warning suppressions.
    /// </summary>
    public bool SuppressMissingXmlCommentWarning
    {
        get => PragmaSuppressions.Contains("CS1591");
        set
        {
            if (value)
                PragmaSuppressions.Add("CS1591");
            else
                PragmaSuppressions.Remove("CS1591");
        }
    }

    /// <summary>
    /// List of usings that can be safely used during code generation
    /// without causing type name clashes
    /// </summary>
    public static readonly HashSet<string> SafeSetOfUsings =
    [
        "Serenity",
        "Serenity.Abstractions",
        "Serenity.ComponentModel",
        "Serenity.Data",
        "Serenity.Data.Mapping",
        "Serenity.Extensions",
        "Serenity.Localization",
        "Serenity.Reflection",
        "Serenity.Services",
        "Serenity.Web",
        "Microsoft.AspNetCore.Mvc",
        "System.Globalization",
        "System.Data",
        "System",
        "System.IO",
        "System.ComponentModel",
        "System.Collections.Generic"
    ];
}