namespace Serenity.TypeScript;

public class PropertyDeclaration(NodeArray<IModifierLike> modifiers, IPropertyName name,
    INode questionOrExclamationToken, ITypeNode type, IExpression initializer)
    : ClassElement(SyntaxKind.PropertyDeclaration, name), IVariableLikeDeclaration,
    IHasModifiers, IHasDecorators, IGetRestChildren
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
    public INode QuestionToken { get; } = questionOrExclamationToken is { Kind: SyntaxKind.QuestionToken } q ? q : null;
    public INode ExclamationToken { get; } = questionOrExclamationToken is { Kind: SyntaxKind.ExclamationToken } q ? q : null;
    public ITypeNode Type { get; } = type;
    public IExpression Initializer { get; } = initializer;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Name, QuestionToken, ExclamationToken, Type, Initializer];
    }
}
