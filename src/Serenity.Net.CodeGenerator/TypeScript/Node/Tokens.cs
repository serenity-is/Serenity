namespace Serenity.TypeScript;

internal class Token : NodeBase
{
    public Token()
    {
    }

    public Token(SyntaxKind kind)
    {
        Kind = kind;
    }
}

internal class TokenTypeNode : Token, ITypeNode
{
}

internal class CommaToken() : Token(SyntaxKind.CommaToken);

internal class DotDotDotToken() : Token(SyntaxKind.DotDotDotToken);

internal class DotToken() : Token(SyntaxKind.DotToken);

internal class QuestionToken() : Token(SyntaxKind.QuestionToken);

internal class ExclamationToken() : Token(SyntaxKind.ExclamationToken);

internal class ColonToken() : Token(SyntaxKind.ColonToken);

internal class EqualsToken() : Token(SyntaxKind.EqualsToken);

internal class AsteriskToken() : Token(SyntaxKind.AsteriskToken);

internal class EqualsGreaterThanToken() : Token(SyntaxKind.EqualsGreaterThanToken);

internal class EndOfFileToken() : Token(SyntaxKind.EndOfFileToken);

internal class AtToken() : Token(SyntaxKind.AtToken);
internal class QuestionDotToken() : Token(SyntaxKind.QuestionDotToken);

internal class AbstractKeyword() : Token(SyntaxKind.AbstractKeyword), IModifier;
internal class AccessorKeyword() : Token(SyntaxKind.AccessorKeyword), IModifier;
internal class AsyncKeyword() : Token(SyntaxKind.AsyncKeyword), IModifier;
internal class ConstKeyword() : Token(SyntaxKind.ConstKeyword), IModifier;
internal class DeclareKeyword() : Token(SyntaxKind.DeclareKeyword), IModifier;
internal class DefaultKeyword() : Token(SyntaxKind.DefaultKeyword), IModifier;
internal class ExportKeyword() : Token(SyntaxKind.ExportKeyword), IModifier;
internal class InKeyword() : Token(SyntaxKind.InKeyword), IModifier;
internal class PrivateKeyword() : Token(SyntaxKind.PrivateKeyword), IModifier;
internal class ProtectedKeyword() : Token(SyntaxKind.ProtectedKeyword), IModifier;
internal class PublicKeyword() : Token(SyntaxKind.PublicKeyword), IModifier;
internal class OutKeyword() : Token(SyntaxKind.OutKeyword), IModifier;
internal class ReadonlyKeyword() : Token(SyntaxKind.ReadonlyKeyword), IModifier;
internal class StaticKeyword() : Token(SyntaxKind.StaticKeyword), IModifier;

internal class AwaitKeyword() : Token(SyntaxKind.AwaitKeyword);

internal class AssertKeyword() : Token(SyntaxKind.AssertKeyword);
internal class AssertsKeyword() : Token(SyntaxKind.AssertsKeyword);

internal class CaseKeyword() : Token(SyntaxKind.CaseKeyword);