using Serenity.TypeScript.TsParser;

namespace Serenity.TypeScript.TsTypes;

public class Node : TextRange, INode
{
    public List<Node> Children { get; set; }
    public string SourceStr { get; set; }

    public string IdentifierStr
    {
        get
        {
            if (Kind == SyntaxKind.Identifier)
                return GetText();

            var child = Children?.FirstOrDefault(v => v.Kind == SyntaxKind.Identifier);
            if (child == null)
                return null;

            return GetText().Trim();
        }
    }
    
    public int ParentId { get; set; }
    public int NodeStart { get; set; } = -1;
    public SyntaxKind Kind { get; set; }
    public NodeFlags Flags { get; set; }
    public ModifierFlags ModifierFlagsCache { get; set; }
    public NodeArray<Decorator> Decorators { get; set; }
    public /*ModifiersArray*/NodeArray<Modifier> Modifiers { get; set; }
    public INode Parent { get; set; }
    public List<JsDoc> JsDoc { get; set; }

    public void MakeChildren(string sourceStr)
    {
        Children = new List<Node>();
        Ts.ForEachChild(this, node =>
        {
            if (node == null)
                return null;
            var n = (Node)node;
            n.SourceStr = sourceStr;
            if (n.Pos != null) n.NodeStart = Scanner.SkipTriviaM(SourceStr, (int)n.Pos);
            Children.Add(n);
            n.MakeChildren(sourceStr);
            return null;
        });
    }

    public void MakeChildrenOptimized(string sourceStr)
    {
        Ts.ForEachChildOptimized(this, node =>
        {
            if (node == null)
                return;

            var n = (Node)node;
            n.SourceStr = sourceStr;
            n.Parent = this;
            if (n.Pos != null) 
                n.NodeStart = Scanner.SkipTriviaM(SourceStr, (int)n.Pos);
            n.MakeChildrenOptimized(sourceStr);
        });
    }

    public string GetText()
    {
        var sourceStr = SourceStr ?? (Parent as Node)?.SourceStr ?? 
            ((Parent as Node)?.Parent as Node)?.SourceStr;
        if (NodeStart == -1)
        {
            if (Pos != null && End != null)
                return sourceStr?[Pos.Value..End.Value];
        }
        else if (End != null && End.Value > NodeStart)
            return sourceStr?[NodeStart..End.Value];

        return null;
    }

    public string GetTextWithComments(string source = null)
    {
        source ??= SourceStr;
        if (Pos != null && End != null)
            return source[(int)Pos..(int)End];
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

    string GetText();
    string GetTextWithComments(string source = null);

    string ToString(bool withPos);

}