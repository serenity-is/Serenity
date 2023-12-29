namespace Serenity.TypeScript;

internal class Token : Node
{
    public Token(SyntaxKind kind)
    {
        Kind = kind;
    }
}


internal class ModifierToken : Token, IModifier
{
    public ModifierToken(SyntaxKind kind)
        : base(kind)
    {
    }

    public ModifierToken()
        : base(SyntaxKind.Unknown)
    {
    }
}

internal class OperatorToken : Token
{
    public OperatorToken(SyntaxKind kind)
    : base(kind)
    {
    }

    public OperatorToken()
        : base(SyntaxKind.Unknown)
    {
    }
}

internal class PrimaryExpressionToken : Token, IPrimaryExpression
{
    public PrimaryExpressionToken()
        : base(SyntaxKind.Unknown)
    {
    }
}

internal class CommaToken() : Token(SyntaxKind.CommaToken);

internal class DotDotDotToken() : Token(SyntaxKind.DotDotDotToken);

internal class DotToken() : Token(SyntaxKind.DotToken);

internal class QuestionToken() : Token(SyntaxKind.QuestionToken);

internal class ExclamationToken() : Token(SyntaxKind.ExclamationToken);

internal class ColonToken() : Token(SyntaxKind.ColonToken);

internal class EqualsToken() : OperatorToken(SyntaxKind.EqualsToken);

internal class AsteriskToken() : Token(SyntaxKind.AsteriskToken);

internal class EqualsGreaterThanToken() : Token(SyntaxKind.EqualsGreaterThanToken);

internal class EndOfFileToken() : Token(SyntaxKind.EndOfFileToken);

internal class AtToken() : Token(SyntaxKind.AtToken);
internal class QuestionDotToken() : Token(SyntaxKind.QuestionDotToken);

internal class AbstractKeyword() : ModifierToken(SyntaxKind.AbstractKeyword);
internal class AccessorKeyword() : ModifierToken(SyntaxKind.AccessorKeyword);
internal class AsyncKeyword() : ModifierToken(SyntaxKind.AsyncKeyword);
internal class ConstKeyword() : ModifierToken(SyntaxKind.ConstKeyword);
internal class DeclareKeyword() : ModifierToken(SyntaxKind.DeclareKeyword);
internal class DefaultKeyword() : ModifierToken(SyntaxKind.DefaultKeyword);
internal class ExportKeyword() : ModifierToken(SyntaxKind.ExportKeyword);
internal class InKeyword() : ModifierToken(SyntaxKind.InKeyword);
internal class PrivateKeyword() : ModifierToken(SyntaxKind.PrivateKeyword);
internal class ProtectedKeyword() : ModifierToken(SyntaxKind.ProtectedKeyword);
internal class PublicKeyword() : ModifierToken(SyntaxKind.PublicKeyword);
internal class OutKeyword() : ModifierToken(SyntaxKind.OutKeyword);
internal class ReadonlyKeyword() : ModifierToken(SyntaxKind.ReadonlyKeyword);
internal class StaticKeyword() : ModifierToken(SyntaxKind.StaticKeyword);

internal class AwaitKeyword() : Token(SyntaxKind.AwaitKeyword);

internal class AssertKeyword() : Token(SyntaxKind.AssertKeyword);
internal class AssertsKeyword() : Token(SyntaxKind.AssertsKeyword);

internal class CaseKeyword() : Token(SyntaxKind.CaseKeyword);
internal class ConstructorKeyword() : Token(SyntaxKind.ConstructorKeyword);