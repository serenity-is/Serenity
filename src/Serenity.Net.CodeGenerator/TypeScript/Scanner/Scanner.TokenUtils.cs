using Serenity.TypeScript;

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
        return SyntaxKind.FirstKeyword <= token && token <= SyntaxKind.LastKeyword;
    }

    internal static bool IsTrivia(SyntaxKind token)
    {
        return SyntaxKind.FirstTriviaToken <= token && token <= SyntaxKind.LastTriviaToken;
    }

    private static readonly string[] tokenStrings = MakeReverseMap(textToToken);

    internal static bool TokenIsIdentifierOrKeyword(SyntaxKind token)
    {
        return token >= SyntaxKind.Identifier;
    }

    private static bool TokenIsIdentifierOrKeywordOrGreaterThan(SyntaxKind token)
    {
        return token == SyntaxKind.GreaterThanToken || TokenIsIdentifierOrKeyword(token);
    }

    internal static string TokenToString(SyntaxKind t)
    {
        return t >= 0 && (int)t <= tokenStrings.Length ? tokenStrings[(int)t] : null;
    }

    private static SyntaxKind? StringToToken(string s)
    {
        if (textToToken.TryGetValue(s, out var token))
            return token;

        return null;
    }

}