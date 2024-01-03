namespace Serenity.TypeScript;

public class JSDocParameterTag : JSDocTag, IGetRestChildren
{
    public JSDocParameterTag()
    {
        Kind = SyntaxKind.JSDocParameterTag;
    }

    public Identifier PreParameterName { get; set; }
    public JSDocTypeExpression TypeExpression { get; set; }
    public Identifier PostParameterName { get; set; }
    public Identifier ParameterName { get; set; }
    public bool IsBracketed { get; set; }

    public override IEnumerable<INode> GetRestChildren()
    {
        foreach (var x in base.GetRestChildren())
            yield return x;

        yield return PreParameterName;
        yield return TypeExpression;
        yield return PostParameterName;
        yield return ParameterName;
    }
}