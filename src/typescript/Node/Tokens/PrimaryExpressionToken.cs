namespace Serenity.TypeScript;

public class PrimaryExpressionToken : Token, IPrimaryExpression
{
    public PrimaryExpressionToken()
        : base(SyntaxKind.Unknown)
    {
    }
}
