
namespace Serenity.TypeScript;

internal class NullLiteral() : Node(SyntaxKind.NullKeyword), IPrimaryExpression, ITypeNode
{
}

internal class BooleanLiteral() : Node(SyntaxKind.BooleanKeyword), IPrimaryExpression, ITypeNode
{
}

internal class NumericLiteral(string value, TokenFlags numericLiteralFlags = TokenFlags.None)
    : LiteralExpressionBase(SyntaxKind.NumericLiteral, value), IPropertyName
{
    public TokenFlags NumericLiteralFlags { get; set; } = numericLiteralFlags;
}

internal class StringLiteral(string text, bool? isSingleQuote = null, bool hasExtendedUnicodeEscape = false)
    : LiteralExpressionBase(SyntaxKind.StringLiteral, text,
        hasExtendedUnicodeEscape: hasExtendedUnicodeEscape), IPropertyName, IJsxAttributeValue, IGetRestChildren, 
    IModuleName, IStringLiteralLike
{
    public bool? IsSingleQuote { get; set; } = isSingleQuote;

    public INode TextSourceNode { get; set; } // Identifier | StringLiteral | NumericLiteral

    public IEnumerable<INode> GetRestChildren()
    {
        return [TextSourceNode];
    }
}

internal class BigIntLiteral(string text) :
    LiteralExpressionBase(SyntaxKind.BigIntLiteral, text), IPropertyName
{
}

internal class NoSubstitutionTemplateLiteral(string text, string rawText, TokenFlags templateFlags)
    : LiteralExpressionBase(SyntaxKind.NoSubstitutionTemplateLiteral, text),
    IPropertyName, ITemplateLiteralLikeNode, IDeclaration, IStringLiteralLike
{
    public string RawText { get; set; } = rawText;
    public TokenFlags? TemplateFlags { get; set; } = templateFlags & TokenFlags.TemplateLiteralLikeFlags;
}

internal class LiteralLikeNode : Node, ILiteralLikeNode, IHasLiteralText
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

internal class LiteralExpressionBase(SyntaxKind kind, string text)
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

internal class RegularExpressionLiteral(string text)
    : LiteralExpressionBase(SyntaxKind.RegularExpressionLiteral, text)
{
}
