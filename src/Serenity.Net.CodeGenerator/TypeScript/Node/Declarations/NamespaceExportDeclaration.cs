namespace Serenity.TypeScript;

internal class NamespaceExportDeclaration(Identifier name)
    : DeclarationStatement<Identifier>(SyntaxKind.NamespaceExportDeclaration, name), IHasModifiers
{
    public NodeArray<IModifierLike> Modifiers { get; set; }
}
