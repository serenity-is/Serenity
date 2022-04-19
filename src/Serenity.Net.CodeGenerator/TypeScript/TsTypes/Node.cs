using Serenity.TypeScript.TsParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Serenity.TypeScript.TsTypes
{
    public class Node : TextRange, INode
    {
        public List<Node> Children { get; set; }
        public ITypeScriptAST Ast { get; set; }

        public string SourceStr
        {
            get => Ast.SourceStr;
            set => Ast.SourceStr = value;
        }

        public string IdentifierStr => Kind == SyntaxKind.Identifier
            ? GetText()
            : Children.FirstOrDefault(v => v.Kind == SyntaxKind.Identifier)?.GetText().Trim();

        public int ParentId { get; set; }
        public int NodeStart { get; set; } = -1;
        public SyntaxKind Kind { get; set; }
        public NodeFlags Flags { get; set; }
        public ModifierFlags ModifierFlagsCache { get; set; }
        public NodeArray<Decorator> Decorators { get; set; }
        public /*ModifiersArray*/NodeArray<Modifier> Modifiers { get; set; }
        public INode Parent { get; set; }
        public List<JsDoc> JsDoc { get; set; }

        public void MakeChildren(TypeScriptAST ast)
        {
            Children = new List<Node>();
            Ts.ForEachChild(this, node =>
            {
                if (node == null)
                    return null;
                var n = (Node)node;
                n.Ast = ast;
                if (n.Pos != null) n.NodeStart = Scanner.SkipTriviaM(SourceStr, (int)n.Pos);
                Children.Add(n);
                n.MakeChildren(ast);
                return null;
            });
        }

        public void MakeChildrenOptimized(TypeScriptAST ast)
        {
            Ts.ForEachChildOptimized(this, node =>
            {
                if (node == null)
                    return;

                var n = (Node)node;
                n.Ast = ast;
                n.Parent = this;
                if (n.Pos != null) 
                    n.NodeStart = Scanner.SkipTriviaM(SourceStr, (int)n.Pos);
                n.MakeChildrenOptimized(ast);
            });
        }

        public string GetText(string source = null)
        {
            if (source == null) source = SourceStr;
            if (NodeStart == -1)
                if (Pos != null && End != null) return source.Substring((int)Pos, (int)End - (int)Pos);
                else return null;

            if (End != null) return source.Substring(NodeStart, (int)End - NodeStart);
            return null;
        }

        public string GetTextWithComments(string source = null)
        {
            if (source == null) source = SourceStr;
            if (Pos != null && End != null)
                return source.Substring((int)Pos, (int)End - (int)Pos);
            return null;
        }

        public override string ToString()
        {
            var posStr = $" [{Pos}, {End}]";

            return $"{Enum.GetName(typeof(SyntaxKind), Kind)}  {posStr} {IdentifierStr}";
        }

        public string ToString(bool withPos)
        {
            if (withPos)
            {
                var posStr = $" [{Pos}, {End}]";

                return $"{Enum.GetName(typeof(SyntaxKind), Kind)}  {posStr} {IdentifierStr}";
            }
            return $"{Enum.GetName(typeof(SyntaxKind), Kind)}  {IdentifierStr}";
        }
        
        public IEnumerable<Node> OfKind(SyntaxKind kind) => GetDescendants(false).OfKind(kind);

        public IEnumerable<Node> GetDescendants(bool includeSelf = true)
        {
            if (includeSelf) yield return this;

            foreach (var descendant in Children)
            {
                foreach (var ch in descendant.GetDescendants())
                    yield return ch;
            }
        }
    }

    public interface INode : ITextRange
    {
        SyntaxKind Kind { get; set; }
        NodeFlags Flags { get; set; }
        ModifierFlags ModifierFlagsCache { get; set; }

        NodeArray<Decorator> Decorators { get; set; }

        /*ModifiersArray*/
        NodeArray<Modifier> Modifiers { get; set; }

        INode Parent { get; set; }
        List<Node> Children { get; set; }
        List<JsDoc> JsDoc { get; set; }

        string GetText(string source = null);
        string GetTextWithComments(string source = null);

        string ToString(bool withPos);

    }
}