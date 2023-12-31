namespace Serenity.TypeScript;

internal class ObjectLiteralElement<TName>(SyntaxKind kind, TName name)
    : NamedDeclaration<TName>(kind, name), IObjectLiteralElement
    where TName : IDeclarationName
{
}