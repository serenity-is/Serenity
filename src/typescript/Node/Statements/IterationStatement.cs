
namespace Serenity.TypeScript;

public class IterationStatement(SyntaxKind kind, IStatement statement)
    : Statement(kind), IGetRestChildren
{
    public IStatement Statement { get; } = statement;

    public virtual IEnumerable<INode> GetRestChildren()
    {
        return [Statement];
    }
}
