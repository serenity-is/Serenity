
namespace Serenity.TypeScript;

public class CatchClause(VariableDeclaration variableDeclaration, Block block)
    : Node(SyntaxKind.CatchClause), IGetRestChildren
{
    public VariableDeclaration VariableDeclaration { get; set; } = variableDeclaration;
    public Block Block { get; set; } = block;

    public IEnumerable<INode> GetRestChildren()
    {
        return [VariableDeclaration, Block];
    }
}
