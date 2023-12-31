namespace Serenity.TypeScript;

internal class Token : Node
{
    public Token(SyntaxKind kind)
    {
        Kind = kind;
    }
}
