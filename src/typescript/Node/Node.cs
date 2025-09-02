namespace Serenity.TypeScript;

public class Node : TextRange, INode
{
    public Node()
    {
    }

    public Node(SyntaxKind kind)
    {
        Kind = kind;
    }

    public SyntaxKind Kind { get; set; }
    public NodeFlags Flags { get; set; }
    public INode Parent { get; set; }


    public override string ToString()
    {
        var posStr = $" [{Pos}, {End}]";

        return $"{Enum.GetName(typeof(SyntaxKind), Kind)}  {posStr} {this.GetText(null)}";
    }

    public string ToString(bool withPos)
    {
        if (withPos)
            return ToString();

        return $"{Enum.GetName(typeof(SyntaxKind), Kind)}  {this.GetText(null)}";
    }

}
