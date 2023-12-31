namespace Serenity.TypeScript;

internal class ModifierToken : Token, IModifier
{
    public ModifierToken(SyntaxKind kind)
        : base(kind)
    {
    }

    public ModifierToken()
        : base(SyntaxKind.Unknown)
    {
    }
}
