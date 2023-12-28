namespace Serenity.TypeScript;

internal class NullLiteral() : NodeBase(SyntaxKind.NullKeyword), IPrimaryExpression, ITypeNode
{
}

internal class BooleanLiteral() : NodeBase(SyntaxKind.BooleanKeyword), IPrimaryExpression, ITypeNode
{
}

internal class NumericLiteral : LiteralExpression, IPropertyName
{
    public NumericLiteral(string value, TokenFlags numericLiteralFlags = TokenFlags.None)
    {
        Kind = SyntaxKind.NumericLiteral;
        Text = value;
        NumericLiteralFlags = numericLiteralFlags;
    }

    public TokenFlags NumericLiteralFlags { get; set; }
}

internal class StringLiteral : LiteralExpression, IPropertyName, IJsxAttributeValue
{
    internal StringLiteral(string text, bool? isSingleQuote = null, bool hasExtendedUnicodeEscape = false)
    {
        Kind = SyntaxKind.StringLiteral;
        Text = text;
        IsSingleQuote = isSingleQuote;
        HasExtendedUnicodeEscape = hasExtendedUnicodeEscape;
    }

    public bool? IsSingleQuote { get; set; }

    public INode TextSourceNode { get; set; } // Identifier | StringLiteral | NumericLiteral
}

internal interface ILiteralLikeNode : INode
{
    string Text { get; set; }
    bool IsUnterminated { get; set; }
    bool HasExtendedUnicodeEscape { get; set; }
    bool IsOctalLiteral { get; set; }
}

internal class LiteralLikeNode : NodeBase, ILiteralLikeNode
{
    internal LiteralLikeNode(SyntaxKind kind, string text)
    {
        Kind = kind;
    }

    public string Text { get; set; }
    public bool IsUnterminated { get; set; }
    public bool HasExtendedUnicodeEscape { get; set; }
    public bool IsOctalLiteral { get; set; }
}

internal interface ILiteralExpression : ILiteralLikeNode, IPrimaryExpression
{
}

internal class LiteralExpression : NodeBase, ILiteralExpression, IPrimaryExpression
{
    public string Text { get; set; }
    public bool IsUnterminated { get; set; }
    public bool HasExtendedUnicodeEscape { get; set; }
    public bool IsOctalLiteral { get; set; }
}

internal class RegularExpressionLiteral : LiteralExpression
{
    public RegularExpressionLiteral(string text)
    {
        Kind = SyntaxKind.RegularExpressionLiteral;
        Text = text;
    }
}