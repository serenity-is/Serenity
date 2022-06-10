namespace Serenity.Reflection
{
    /// <summary>
    /// Used to write formatted code to a string builder.
    /// </summary>
    public class CodeWriter
    {
        private readonly StringBuilder sb;
        private readonly string tab;
        private string indent;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeWriter"/> class.
        /// </summary>
        /// <param name="sb">The string builder.</param>
        /// <param name="tab">Tab string (e.g. \9 or '  ').</param>
        public CodeWriter(StringBuilder sb, string tab)
            : this(sb, 1)
        {
            this.tab = tab;
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
        /// Initializes a new instance of the <see cref="CodeWriter"/> class.
        /// </summary>
        /// <param name="sb">The sb.</param>
        /// <param name="tabSize">Number of spaces.</param>
        public CodeWriter(StringBuilder sb, int tabSize)
        {
            tab = new string(' ', tabSize);
            indent = "";
            this.sb = sb;
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
            StartBrace();
            insideBlock();
            EndBrace(endLine);
        }

        /// <summary>
        /// Adds a brace, increases indent.
        /// </summary>
        public void StartBrace()
        {
            sb.Append(indent);
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
                sb.Append(indent);
                sb.AppendLine(line);
            }
        }

        /// <summary>
        /// Gets tab string
        /// </summary>
        public string Tab => tab;

        /// <summary>
        /// Gets internal string builder
        /// </summary>
        public StringBuilder Builder => sb;

        /// <summary>
        /// Executes action by opening namespace if it is not null or empty
        /// </summary>
        /// <param name="ns">Namespace</param>
        /// <param name="action">Action</param>
        public void InNamespace(string ns, Action action)
        {
            if (!string.IsNullOrEmpty(ns))
            {
                var oldNamespace = CurrentNamespace;
                sb.Append("namespace ");
                sb.AppendLine(ns);
                CurrentNamespace = ns;
                InBrace(action);
                CurrentNamespace = oldNamespace;
            }
            else
            {
                var oldNamespace = CurrentNamespace;
                CurrentNamespace = ns;
                action();
                CurrentNamespace = oldNamespace;
            }
        }

        /// <summary>
        /// Gets / sets current namespace
        /// </summary>
        public string CurrentNamespace { get; set; }

        /// <summary>
        /// Gets / sets file comment
        /// </summary>
        public string FileComment { get; set; }

        /// <summary>
        /// Gets / sets global usings hash set
        /// </summary>
        public HashSet<string> GlobalUsings { get; set; }

        /// <summary>
        /// Gets / sets local usings hash set
        /// </summary>
        public HashSet<string> LocalUsings { get; set; } = new HashSet<string>();

        /// <summary>
        /// Gets sets function that determines if a namespace is allowed to be added to the local usings
        /// </summary>
        public Func<string, bool> AllowUsing { get; set; } = (ns) => true;

        /// <summary>
        /// Returns true if the namespace is in list of usings.
        /// </summary>
        /// <param name="ns"></param>
        /// <returns></returns>
        public bool IsUsing(string ns)
        {
            if (ns == CurrentNamespace)
                return true;

            if (GlobalUsings != null && GlobalUsings.Contains(ns))
                return true;

            return LocalUsings != null && LocalUsings.Contains(ns);
        }

        /// <summary>
        /// Returns true if the namespace is in list of usings.
        /// If AllowUsing callback is not null, this may add it to the list of local usings if function returns true.
        /// </summary>
        /// <param name="ns"></param>
        /// <returns></returns>
        public bool Using(string ns)
        {
            if (IsUsing(ns))
                return true;

            if (AllowUsing != null && AllowUsing(ns))
            {
                LocalUsings.Add(ns);
                return true;
            }

            return false;
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
        /// Appends a char to internal string builder
        /// </summary>
        /// <param name="c">Char</param>
        /// <returns>String builder</returns>
        public StringBuilder Append(char c)
        {
            return sb.Append(c);
        }

        /// <summary>
        /// Returns string representation of internal string builder, 
        /// including file comment and any local usings if any
        /// </summary>
        /// <returns>String representation of generated code</returns>
        public override string ToString()
        {
            var nsb = new StringBuilder();
            if (!string.IsNullOrEmpty(FileComment))
            {
                nsb.AppendLine(FileComment);
                nsb.AppendLine();
            }

            if (LocalUsings != null &&
                LocalUsings.Any())
            {
                nsb.AppendLine(string.Join(Environment.NewLine,
                    LocalUsings.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                        .Select(x => "using " + x + ";")));
                nsb.AppendLine();
            }

            nsb.Append(sb);

            return nsb.ToString().TrimEnd();
        }
    }
}