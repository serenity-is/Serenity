namespace Serenity.TypeScript;

public class StringLiteral(string text, bool? isSingleQuote = null, bool hasExtendedUnicodeEscape = false)
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
