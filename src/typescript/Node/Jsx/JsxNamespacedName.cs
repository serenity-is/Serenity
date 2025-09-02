namespace Serenity.TypeScript;

public class JsxNamespacedName(Identifier name, Identifier @namespace)
    : Node(SyntaxKind.JsxNamespacedName), IJsxTagNameExpression, IJsxAttributeName, IHasNameProperty, IGetRestChildren
{
    public Identifier Name { get; } = name;
    public Identifier Namespace { get; } = @namespace;

    IDeclarationName IHasNameProperty.Name => Name;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Name, Namespace];
    }
}