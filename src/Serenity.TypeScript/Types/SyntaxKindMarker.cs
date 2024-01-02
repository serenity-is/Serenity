namespace Serenity.TypeScript;

public static class SyntaxKindMarker
{
    // Markers
    public const SyntaxKind FirstAssignment = SyntaxKind.EqualsToken;
    public const SyntaxKind LastAssignment = SyntaxKind.CaretEqualsToken;
    public const SyntaxKind FirstCompoundAssignment = SyntaxKind.PlusEqualsToken;
    public const SyntaxKind LastCompoundAssignment = SyntaxKind.CaretEqualsToken;
    public const SyntaxKind FirstReservedWord = SyntaxKind.BreakKeyword;
    public const SyntaxKind LastReservedWord = SyntaxKind.WithKeyword;
    public const SyntaxKind FirstKeyword = SyntaxKind.BreakKeyword;
    public const SyntaxKind LastKeyword = SyntaxKind.OfKeyword;
    public const SyntaxKind FirstFutureReservedWord = SyntaxKind.ImplementsKeyword;
    public const SyntaxKind LastFutureReservedWord = SyntaxKind.YieldKeyword;
    public const SyntaxKind FirstTypeNode = SyntaxKind.TypePredicate;
    public const SyntaxKind LastTypeNode = SyntaxKind.ImportType;
    public const SyntaxKind FirstPunctuation = SyntaxKind.OpenBraceToken;
    public const SyntaxKind LastPunctuation = SyntaxKind.CaretEqualsToken;
    public const SyntaxKind FirstToken = SyntaxKind.Unknown;
    public const SyntaxKind LastToken = LastKeyword;
    public const SyntaxKind FirstTriviaToken = SyntaxKind.SingleLineCommentTrivia;
    public const SyntaxKind LastTriviaToken = SyntaxKind.ConflictMarkerTrivia;
    public const SyntaxKind FirstLiteralToken = SyntaxKind.NumericLiteral;
    public const SyntaxKind LastLiteralToken = SyntaxKind.NoSubstitutionTemplateLiteral;
    public const SyntaxKind FirstTemplateToken = SyntaxKind.NoSubstitutionTemplateLiteral;
    public const SyntaxKind LastTemplateToken = SyntaxKind.TemplateTail;
    public const SyntaxKind FirstBinaryOperator = SyntaxKind.LessThanToken;
    public const SyntaxKind LastBinaryOperator = SyntaxKind.CaretEqualsToken;
    public const SyntaxKind FirstStatement = SyntaxKind.VariableStatement;
    public const SyntaxKind LastStatement = SyntaxKind.DebuggerStatement;
    public const SyntaxKind FirstNode = SyntaxKind.QualifiedName;
    public const SyntaxKind FirstJSDocNode = SyntaxKind.JSDocTypeExpression;
    public const SyntaxKind LastJSDocNode = SyntaxKind.JSDocSatisfiesTag;
    public const SyntaxKind FirstJSDocTagNode = SyntaxKind.JSDocTag;
    public const SyntaxKind LastJSDocTagNode = SyntaxKind.JSDocSatisfiesTag;

    internal const SyntaxKind FirstContextualKeyword = SyntaxKind.AbstractKeyword;
    internal const SyntaxKind LastContextualKeyword = SyntaxKind.OfKeyword;
}