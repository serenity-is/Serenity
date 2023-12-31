namespace Serenity.TypeScript;

internal class OperatorToken : Token
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