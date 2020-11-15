using System;
using System.Text;

namespace Serenity.Reflection
{
    /// <summary>
    /// Used to write formatted code to a string builder.
    /// </summary>
    public class CodeWriter
    {
        private readonly StringBuilder sb;
        private readonly string space;
        private string indent;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeWriter"/> class.
        /// </summary>
        /// <param name="sb">The sb.</param>
        /// <param name="indentSize">Size of the indent.</param>
        public CodeWriter(StringBuilder sb, int indentSize)
        {
            this.space = new String(' ', indentSize);
            this.indent = "";
            this.sb = sb;
        }

        private void IncreaseIndent()
        {
            indent += space;
        }

        private void DecreaseIndent()
        {
            if (indent.Length >= space.Length)
                indent = indent.Substring(0, indent.Length - space.Length);
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
        public void InBrace(Action insideBlock)
        {
            StartBrace();
            insideBlock();
            EndBrace();
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
        public void EndBrace()
        {
            DecreaseIndent();
            sb.Append(indent);
            sb.AppendLine("}");
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
        /// Appends current indent for all lines of the specified string by splitting it 
        /// by end of line characters, and a new line.
        /// </summary>
        public void IndentedMultiLine(string s)
        {
            var lines = s.Split(new char[] { '\n' }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                var x = line.Replace("\r", "");
                IndentedLine(x);
            }
        }

        /// <summary>
        /// Whether to put opening brace on the same line.
        /// </summary>
        /// <value>
        ///   <c>true</c> if brace on same line; otherwise, <c>false</c>.
        /// </value>
        public bool BraceOnSameLine { get; set; }
    }
}