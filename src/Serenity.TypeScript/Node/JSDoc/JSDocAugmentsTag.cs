namespace Serenity.TypeScript;

public class JSDocAugmentsTag : JSDocTag, IGetRestChildren
{
    public JSDocAugmentsTag()
    {
        Kind = SyntaxKind.JSDocAugmentsTag;
    }

    public JSDocTypeExpression TypeExpression { get; set; }

    public override IEnumerable<INode> GetRestChildren()
    {
        foreach (var x in base.GetRestChildren())
            yield return x;
        yield return TypeExpression;
    }
}
