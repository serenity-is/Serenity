namespace Serenity.TypeScript;

public class JsxAttribute(IJsxAttributeName name, IExpression initializer)
    : ObjectLiteralElement<IJsxAttributeName>(SyntaxKind.JsxAttribute, name), IGetRestChildren
{
    public IExpression Initializer { get; } = initializer; // StringLiteral | JsxExpression

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Name, Initializer];
    }
}