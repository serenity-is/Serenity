namespace Serenity.TypeScript;

internal class NamespaceImport(Identifier name)
    : NamedDeclaration<Identifier>(SyntaxKind.NamespaceImport, name), INamedImportBindings
{
}
