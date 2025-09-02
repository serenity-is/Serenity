namespace Serenity.TypeScript;

public class DeclarationStatement<TName>(SyntaxKind kind, TName name)
    : NamedDeclaration<TName>(kind, name), IDeclarationStatement, IStatement, INamedDeclaration
    where TName : IDeclarationName
{
}