namespace Serenity.TypeScript;

public class ModuleDeclaration(NodeArray<IModifierLike> modifiers, IModuleName name, IModuleBody body)
    : DeclarationStatement<IModuleName>(SyntaxKind.ModuleDeclaration, name), IHasModifiers, IGetRestChildren
{
    public ModuleDeclaration(NodeArray<IModifierLike> modifiers, IModuleName name, IModuleBody body, NodeFlags flags)
        : this(modifiers, name, body)
    {
        Flags |= flags;
    }

    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
    public IModuleBody Body { get; set; } = body;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Name, Body];
    }
}