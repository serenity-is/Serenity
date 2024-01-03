namespace Serenity.TypeScript;

public class JSDocReturnTag : JSDocTag, IGetRestChildren
{
    public JSDocReturnTag()
    {
        Kind = SyntaxKind.JSDocReturnTag;
    }

    public JSDocTypeExpression TypeExpression { get; set; }

    public override IEnumerable<INode> GetRestChildren()
    {
        foreach (var x in base.GetRestChildren())
            yield return x;

        yield return TypeExpression;
    }
}