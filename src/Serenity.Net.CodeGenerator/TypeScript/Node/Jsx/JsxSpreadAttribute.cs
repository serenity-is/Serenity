namespace Serenity.TypeScript;

internal class JsxSpreadAttribute(IExpression expression)
    : ObjectLiteralElement<IPropertyName>(SyntaxKind.JsxSpreadAttribute, name: null), IGetRestChildren
{
    public IExpression Expression { get; } = expression;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Name, Expression];
    }
}
