namespace Serenity.TypeScript;

public class JSDocTypeTag : JSDocTag, IGetRestChildren
{
    public JSDocTypeTag()
    {
        Kind = SyntaxKind.JSDocTypeTag;
    }

    public JSDocTypeExpression TypeExpression { get; set; }

    public override IEnumerable<INode> GetRestChildren()
    {
        foreach (var x in base.GetRestChildren())
            yield return x;

        yield return TypeExpression;
    }
}