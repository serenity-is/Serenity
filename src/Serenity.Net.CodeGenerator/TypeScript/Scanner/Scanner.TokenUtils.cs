namespace Serenity.TypeScript;

partial class Scanner
{
    private static string[] MakeReverseMap(Dictionary<string, SyntaxKind> source)
    {
        var result = new string[(int)SyntaxKind.Count + 1];
        foreach (var pair in source)
            result[(int)pair.Value] = pair.Key;
        return result;
    }

    internal static bool IsKeyword(SyntaxKind token)
    {
        return SyntaxKindMarker.FirstKeyword <= token && token <= SyntaxKindMarker.LastKeyword;
    }

    internal static bool IsKeywordOrPunctuation(SyntaxKind token)
    {
        return IsKeyword(token) || IsPunctuation(token);
    }

    internal static bool IsLiteralKind(SyntaxKind kind)
    {
        return SyntaxKindMarker.FirstLiteralToken <= kind && kind <= SyntaxKindMarker.LastLiteralToken;
    }

    internal static bool IsPunctuation(SyntaxKind token)
    {
        return SyntaxKindMarker.FirstPunctuation <= token && token <= SyntaxKindMarker.LastPunctuation;
    }

    internal static bool IsTemplateLiteralKind(SyntaxKind kind)
    {
        return SyntaxKindMarker.FirstTemplateToken <= kind && kind <= SyntaxKindMarker.LastTemplateToken;
    }

    internal static bool IsTaggedTemplateExpression(SyntaxKind kind)
    {
        return kind == SyntaxKind.TaggedTemplateExpression;
    }

    internal static bool IsTrivia(SyntaxKind token)
    {
        return SyntaxKindMarker.FirstTriviaToken <= token && token <= SyntaxKindMarker.LastTriviaToken;
    }

    private static readonly string[] tokenStrings = MakeReverseMap(textToToken);

    internal static bool TokenIsIdentifierOrKeyword(SyntaxKind token)
    {
        return token >= SyntaxKind.Identifier;
    }
    internal static bool TokenIsIdentifierOrKeywordOrGreaterThan(SyntaxKind token)
    {
        return token == SyntaxKind.GreaterThanToken || TokenIsIdentifierOrKeyword(token);
    }

    internal static string TokenToString(SyntaxKind t)
    {
        return t >= 0 && (int)t <= tokenStrings.Length ? tokenStrings[(int)t] : null;
    }

    internal static SyntaxKind? StringToToken(string s)
    {
        if (textToToken.TryGetValue(s, out var token))
            return token;

        return null;
    }

}