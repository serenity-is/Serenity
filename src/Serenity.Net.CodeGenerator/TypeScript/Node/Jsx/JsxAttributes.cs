namespace Serenity.TypeScript;

internal class JsxAttributes(NodeArray<IObjectLiteralElement> properties)
    : ObjectLiteralExpressionBase<IObjectLiteralElement>(SyntaxKind.JsxAttributes, properties) // JsxAttributeLike>
{
}
