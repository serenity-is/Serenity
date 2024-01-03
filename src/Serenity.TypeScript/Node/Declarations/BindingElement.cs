namespace Serenity.TypeScript;

public class BindingElement(DotDotDotToken dotDotDotToken, IPropertyName propertyName, IBindingName name, IExpression initializer)
    : NamedDeclaration<IBindingName>(SyntaxKind.BindingElement, name),
    IArrayBindingElement, IVariableLikeDeclaration, IGetRestChildren
{
    public DotDotDotToken DotDotDotToken { get; } = dotDotDotToken;
    public IPropertyName PropertyName { get; } = propertyName;
    public IExpression Initializer { get; } = initializer;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [DotDotDotToken, PropertyName, Name, Initializer];
    }
}
