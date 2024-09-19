namespace Serenity.TypeScript;

public class OperatorToken : Token
{
    public OperatorToken(SyntaxKind kind)
    : base(kind)
    {
    }

    public OperatorToken()
        : base(SyntaxKind.Unknown)
    {
    }
}