namespace Serenity.TypeScript;

public class ImportEqualsDeclaration(NodeArray<IModifierLike> modifiers, bool isTypeOnly, Identifier name,
    IModuleReference moduleReference) : DeclarationStatement<Identifier>(SyntaxKind.ImportEqualsDeclaration, name), IHasModifiers, IGetRestChildren
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
    public bool IsTypeOnly { get; } = isTypeOnly;
    public IModuleReference ModuleReference { get; set; } = moduleReference;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Name, ModuleReference];
    }
}
