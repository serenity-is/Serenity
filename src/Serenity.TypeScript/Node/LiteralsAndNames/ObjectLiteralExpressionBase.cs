namespace Serenity.TypeScript;

public class ObjectLiteralExpressionBase<T>(SyntaxKind kind, NodeArray<T> properties)
    : Node(kind), IPrimaryExpression, IDeclaration, IGetRestChildren where T : INode
{
    public NodeArray<T> Properties { get; set; } = properties;

    public virtual IEnumerable<INode> GetRestChildren()
    {
        if (Properties != null) foreach (var x in Properties) yield return x;
    }
}