namespace Serenity.TypeScript;

internal class ObjectLiteralExpression(NodeArray<IObjectLiteralElementLike> properties, bool multiLine)
    : ObjectLiteralExpressionBase<IObjectLiteralElementLike>(SyntaxKind.ObjectLiteralExpression, properties)
{
    public bool MultiLine { get; } = multiLine;
}
