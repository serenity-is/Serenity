namespace Serenity.TypeScript;

public class EnumDeclaration(NodeArray<IModifierLike> modifiers, Identifier name,
    NodeArray<EnumMember> members) : DeclarationStatement<Identifier>(SyntaxKind.EnumDeclaration, name),
        IHasModifiers, IGetRestChildren
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
    public NodeArray<EnumMember> Members { get; set; } = members;

    public override IEnumerable<INode> GetRestChildren()
    {
        yield return Name;
        if (Members != null) foreach (var x in Members) yield return x;
    }
}