namespace Serenity.TypeScript;

public class NodeArray<T> : List<T>, ITextRange
{
    public NodeArray()
    {
    }

    public NodeArray(IEnumerable<T> elements)
        : base(elements)
    {
    }

    public bool HasTrailingComma { get; set; }
    public int? Pos { get; set; }
    public int? End { get; set; }
    public bool IsMissingList { get; set; }
}

