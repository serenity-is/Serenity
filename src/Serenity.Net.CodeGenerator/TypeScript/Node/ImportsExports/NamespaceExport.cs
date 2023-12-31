namespace Serenity.TypeScript;

internal class NamespaceExport(Identifier name)
    : NamedDeclaration<Identifier>(SyntaxKind.NamespaceExport, name), INamedExportBindings
{
}