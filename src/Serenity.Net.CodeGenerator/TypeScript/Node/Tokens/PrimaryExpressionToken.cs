namespace Serenity.TypeScript;

internal class PrimaryExpressionToken : Token, IPrimaryExpression
{
    public PrimaryExpressionToken()
        : base(SyntaxKind.Unknown)
    {
    }
}
