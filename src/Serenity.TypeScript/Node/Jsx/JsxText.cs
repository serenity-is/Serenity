namespace Serenity.TypeScript;

public class JsxText(string text, bool containsOnlyTriviaWhiteSpaces)
    : LiteralLikeNode(SyntaxKind.JsxText, text), IJsxChild
{
    public bool ContainsOnlyTriviaWhiteSpaces { get; } = containsOnlyTriviaWhiteSpaces;
}

