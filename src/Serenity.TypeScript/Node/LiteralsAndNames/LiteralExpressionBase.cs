namespace Serenity.TypeScript;

public class LiteralExpressionBase(SyntaxKind kind, string text)
    : Node(kind), ILiteralExpression, IPrimaryExpression, IHasLiteralText
{
    internal LiteralExpressionBase(SyntaxKind kind, string text, bool isUnterminated = false,
        bool hasExtendedUnicodeEscape = false, bool isOctalLiteral = false) : this(kind, text)
    {
        IsUnterminated = isUnterminated;
        HasExtendedUnicodeEscape = hasExtendedUnicodeEscape;
        IsOctalLiteral = isOctalLiteral;
    }

    public string Text { get; set; } = text;
    public bool IsUnterminated { get; set; }
    public bool HasExtendedUnicodeEscape { get; set; }
    public bool IsOctalLiteral { get; set; }
}
