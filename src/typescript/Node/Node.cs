namespace Serenity.TypeScript;

public class Node(SyntaxKind kind) : TextRange, INode
{
    public SyntaxKind Kind { get; set; } = kind;
    public NodeFlags Flags { get; set; }
    public INode? Parent { get; set; }

    public override string ToString()
    {
        var posStr = $" [{Pos}, {End}]";

#if NETSTANDARD2_0
        return $"{Enum.GetName(typeof(SyntaxKind), Kind)}  {posStr} {this.GetText(null)}";
#else
        return $"{Enum.GetName(Kind)}  {posStr} {this.GetText(null)}";
#endif
    }

    public string ToString(bool withPos)
    {
        if (withPos)
            return ToString();

#if NETSTANDARD2_0
        return $"{Enum.GetName(typeof(SyntaxKind), Kind)}  {this.GetText(null)}";
#else
        return $"{Enum.GetName(Kind)}  {this.GetText(null)}";
#endif
    }

}
