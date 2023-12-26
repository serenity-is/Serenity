namespace Serenity.TypeScript;

public class NodeArray<T> : List<T>, ITextRange
{
    public NodeArray()
    {
    }

    public NodeArray(T[] elements)
        : base(elements.ToList())
    {
    }

    public bool HasTrailingComma { get; set; }
    public TransformFlags TransformFlags { get; set; }
    public int? Pos { get; set; }
    public int? End { get; set; }
}
