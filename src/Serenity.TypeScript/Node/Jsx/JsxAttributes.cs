namespace Serenity.TypeScript;

public class JsxAttributes(NodeArray<IObjectLiteralElement> properties)
    : ObjectLiteralExpressionBase<IObjectLiteralElement>(SyntaxKind.JsxAttributes, properties) // JsxAttributeLike>
{
}
