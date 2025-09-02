namespace Serenity.TypeScript;

public class JSDoc : Node, IGetRestChildren
{
    public NodeArray<IJSDocTag> Tags { get; set; }
    public string Comment { get; set; }

    public IEnumerable<INode> GetRestChildren()
    {
        if (Tags != null) foreach (var x in Tags) yield return x;
    }
}
