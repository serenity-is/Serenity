
namespace Serenity.TypeScript;

public class VariableStatement(NodeArray<IModifierLike> modifiers, IVariableDeclarationList variableDeclarationList)
    : Statement(SyntaxKind.VariableStatement), IFlowContainer, IHasModifiers, IGetRestChildren
{
    public NodeArray<IModifierLike> Modifiers => modifiers;
    public IVariableDeclarationList DeclarationList { get; } = variableDeclarationList;

    public IEnumerable<INode> GetRestChildren()
    {
        return [DeclarationList];
    }
}
