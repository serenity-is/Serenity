
namespace Serenity.TypeScript;

public class NamedDeclaration<TName>(SyntaxKind kind, TName name) : Node(kind), INamedDeclaration, IGetRestChildren
    where TName : IDeclarationName

{
    public TName Name { get; set; } = name;

    IDeclarationName IHasNameProperty.Name => Name;

    public virtual IEnumerable<INode> GetRestChildren()
    {
        return [Name];
    }
}
