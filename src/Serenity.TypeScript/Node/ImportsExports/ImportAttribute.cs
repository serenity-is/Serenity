namespace Serenity.TypeScript;

public class ImportAttribute(IDeclarationName name, IExpression value)
    : Node(SyntaxKind.ImportAttribute), IGetRestChildren, IHasNameProperty
{
    public IDeclarationName Name { get; } = name;
    public IExpression Value { get; } = value;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Name, Value];
    }
}