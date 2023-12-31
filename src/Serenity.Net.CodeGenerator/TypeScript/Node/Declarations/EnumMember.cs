namespace Serenity.TypeScript;

internal class EnumMember(IPropertyName name, IExpression initializer)
    : NamedDeclaration<IPropertyName>(SyntaxKind.EnumMember, name), IGetRestChildren
{
    public IExpression Initializer { get; } = initializer;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Name, Initializer];
    }
}

