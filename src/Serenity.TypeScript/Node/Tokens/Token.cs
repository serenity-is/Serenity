namespace Serenity.TypeScript;

public class Token : Node
{
    public Token(SyntaxKind kind)
    {
        Kind = kind;
    }
}
