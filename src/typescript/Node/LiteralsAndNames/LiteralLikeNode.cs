namespace Serenity.TypeScript;


public class LiteralLikeNode 
    : Node, ILiteralLikeNode, IHasLiteralText
{
    internal LiteralLikeNode(SyntaxKind kind, string text)
    {
        Kind = kind;
        Text = text;
    }

    public string Text { get; set; }
    public bool IsUnterminated { get; set; }
    public bool HasExtendedUnicodeEscape { get; set; }
    public bool IsOctalLiteral { get; set; }
}
