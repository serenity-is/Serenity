namespace Serenity.TypeScript;

public class NamespaceExportDeclaration(Identifier name)
    : DeclarationStatement<Identifier>(SyntaxKind.NamespaceExportDeclaration, name), IHasModifiers
{
    public NodeArray<IModifierLike> Modifiers { get; set; }
}
