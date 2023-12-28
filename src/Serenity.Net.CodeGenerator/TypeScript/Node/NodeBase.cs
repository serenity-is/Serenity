namespace Serenity.TypeScript;

internal class TextRange : ITextRange
{
    public int? Pos { get; set; }
    public int? End { get; set; }
}

internal class NodeBase : TextRange, INode
{
    public NodeBase()
    {
    }

    public NodeBase(SyntaxKind kind)
    {
        Kind = kind;
    }

    public SyntaxKind Kind { get; set; }
    public NodeFlags Flags { get; set; }
    public INode Parent { get; set; }

    public string GetText(string sourceText)
    {
        if ((sourceText ??= this.GetSourceFile()?.Text) == null)
            return null;

        if (Pos == null)
            return null;

        var pos = Scanner.SkipTrivia(sourceText, Pos);
        if (End < pos)
            return null;

        return sourceText[Pos.Value..End.Value];
    }

    public string GetTextWithTrivia(string sourceText)
    {
        if ((sourceText ??= this.GetSourceFile()?.Text) == null)
            return null;

        if (Pos == null || End < Pos)
            return null;

        return sourceText[(int)Pos..(int)End];
    }

    public override string ToString()
    {
        var posStr = $" [{Pos}, {End}]";

        return $"{Enum.GetName(typeof(SyntaxKind), Kind)}  {posStr} {GetText(null)}";
    }

    public string ToString(bool withPos)
    {
        if (withPos)
            return ToString();

        return $"{Enum.GetName(typeof(SyntaxKind), Kind)}  {GetText(null)}";
    }

}

internal class NodeWithChildren : NodeBase, IHasChildren
{
    IEnumerable<INode> IHasChildren.Children { get; set; }
}
