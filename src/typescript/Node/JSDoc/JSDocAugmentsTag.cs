namespace Serenity.TypeScript;

public class JSDocAugmentsTag() : JSDocTag(SyntaxKind.JSDocAugmentsTag), IGetRestChildren
{
    public JSDocTypeExpression? TypeExpression { get; set; }

    public override IEnumerable<INode?> GetRestChildren()
    {
        foreach (var x in base.GetRestChildren())
            yield return x;
        yield return TypeExpression;
    }
}
