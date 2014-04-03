using Newtonsoft.Json;
using Serenity.Data;
using Serenity.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Serenity.Reflection
{
    public class CodeWriter
    {
        private StringBuilder sb;
        private string space;
        private string indent;

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

        public void Block(Action insideBlock)
        {
            IncreaseIndent();
            insideBlock();
            DecreaseIndent();
        }

        public void InBrace(Action insideBlock)
        {
            sb.Append(indent);
            sb.AppendLine("{");
            IncreaseIndent();
            insideBlock();
            DecreaseIndent();
            sb.Append(indent);
            sb.AppendLine("}");
        }

        public void Indent()
        {
            sb.Append(indent);
        }

        public void Indented(string s)
        {
            sb.Append(indent);
            sb.Append(s);
        }

        public void IndentedLine(string s)
        {
            sb.Append(indent);
            sb.AppendLine(s);
        }
    }
}