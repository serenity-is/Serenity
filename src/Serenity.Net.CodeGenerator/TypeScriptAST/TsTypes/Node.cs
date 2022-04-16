using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdcb.TypeScript.TsParser;

namespace Sdcb.TypeScript.TsTypes
{
    public class Node : TextRange, INode
    {
        public List<Node> Children { get; set; } = new List<Node>();
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
        public int Depth { get; set; }
        public int NodeStart { get; set; } = -1;
        public SyntaxKind Kind { get; set; }
        public NodeFlags Flags { get; set; }
        public ModifierFlags ModifierFlagsCache { get; set; }
        public TransformFlags TransformFlags { get; set; }
        public NodeArray<Decorator> Decorators { get; set; }
        public /*ModifiersArray*/NodeArray<Modifier> Modifiers { get; set; }
        public int Id { get; set; }
        public INode Parent { get; set; }
        public Node Original { get; set; }
        public bool StartsOnNewLine { get; set; }
        public List<JsDoc> JsDoc { get; set; }
        public List<JsDoc> JsDocCache { get; set; }
        public Symbol Symbol { get; set; }
        public SymbolTable Locals { get; set; }
        public Node NextContainer { get; set; }
        public Symbol LocalSymbol { get; set; }
        public FlowNode FlowNode { get; set; }
        public EmitNode EmitNode { get; set; }

        public Type ContextualType { get; set; }
        public TypeMapper ContextualMapper { get; set; }

        public int TagInt { get; set; }

        public void MakeChildren(TypeScriptAST ast)
        {
            Children = new List<Node>();
            Ts.ForEachChild(this, node =>
            {
                if (node == null)
                    return null;
                var n = (Node)node;
                n.Ast = ast;
                n.Depth = Depth + 1;
                n.Parent = this;
                if (n.Pos != null) n.NodeStart = Scanner.SkipTriviaM(SourceStr, (int)n.Pos);
                Children.Add(n);
                n.MakeChildren(ast);
                return null;
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
        public Node First => Children.FirstOrDefault();
        public Node Last => Children.LastOrDefault();
        public int Count => Children.Count;

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
        public string GetTreeString(bool withPos = true)
        {
            var sb = new StringBuilder();
            var descendants = GetDescendants().ToList();
            foreach (var node in descendants)
            {
                //var anc = node.GetAncestors().ToList();
                for (int i = 1; i < node.Depth; i++) //anc.Count()
                {
                    sb.Append("  ");
                }
                sb.AppendLine(node.ToString(withPos));
            }
            return sb.ToString();
        }

    }

    public interface INode : ITextRange
    {
        SyntaxKind Kind { get; set; }
        NodeFlags Flags { get; set; }
        ModifierFlags ModifierFlagsCache { get; set; }
        TransformFlags TransformFlags { get; set; }

        NodeArray<Decorator> Decorators { get; set; }

        /*ModifiersArray*/
        NodeArray<Modifier> Modifiers { get; set; }

        int Id { get; set; }
        INode Parent { get; set; }
        List<Node> Children { get; set; }
        int Depth { get; set; }
        Node Original { get; set; }
        bool StartsOnNewLine { get; set; }
        List<JsDoc> JsDoc { get; set; }
        List<JsDoc> JsDocCache { get; set; }
        Symbol Symbol { get; set; }
        SymbolTable Locals { get; set; }
        Node NextContainer { get; set; }
        Symbol LocalSymbol { get; set; }
        FlowNode FlowNode { get; set; }
        EmitNode EmitNode { get; set; }
        Type ContextualType { get; set; }
        TypeMapper ContextualMapper { get; set; }
        int TagInt { get; set; }

        string GetText(string source = null);
        string GetTextWithComments(string source = null);
        string GetTreeString(bool withPos = true);

        string ToString(bool withPos);
        Node First { get; }
        Node Last { get; }
        int Count { get; }

    }
}