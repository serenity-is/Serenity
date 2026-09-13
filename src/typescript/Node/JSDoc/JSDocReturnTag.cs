namespace Serenity.TypeScript;

public class JSDocReturnTag() : JSDocTag(SyntaxKind.JSDocReturnTag), IGetRestChildren
{
    public JSDocTypeExpression? TypeExpression { get; set; }

    public override IEnumerable<INode?> GetRestChildren()
    {
        foreach (var x in base.GetRestChildren())
            yield return x;

        yield return TypeExpression;
    }
}