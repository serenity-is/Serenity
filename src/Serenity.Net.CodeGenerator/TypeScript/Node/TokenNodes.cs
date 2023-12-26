namespace Serenity.TypeScript;

public class Token : Node
{
    public Token()
    {
    }

    public Token(SyntaxKind kind)
    {
        Kind = kind;
    }
}

public class DotDotDotToken() : Token(SyntaxKind.DotDotDotToken);

public class DotToken() : Token(SyntaxKind.DotToken);

public class QuestionToken() : Token(SyntaxKind.QuestionToken);

public class ColonToken() : Token(SyntaxKind.ColonToken);

public class EqualsToken() : Token(SyntaxKind.EqualsToken);

public class AsteriskToken() : Token(SyntaxKind.AsteriskToken);

public class EqualsGreaterThanToken() : Token(SyntaxKind.EqualsGreaterThanToken);

public class EndOfFileToken() : Token(SyntaxKind.EndOfFileToken);

public class AtToken() : Token(SyntaxKind.AtToken);

public class ReadonlyToken() : Token(SyntaxKind.ReadonlyKeyword);

public class AwaitKeywordToken() : Token(SyntaxKind.AwaitKeyword);

