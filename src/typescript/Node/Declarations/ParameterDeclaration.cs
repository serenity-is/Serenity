namespace Serenity.TypeScript;

public class ParameterDeclaration(NodeArray<IModifierLike> modifiers, DotDotDotToken dotDotDotToken,
    IBindingName name, QuestionToken questionToken, ITypeNode type, IExpression initializer)
    : NamedDeclaration<IBindingName>(SyntaxKind.Parameter, name), IVariableLikeDeclaration, IHasModifiers, IHasDecorators, IGetRestChildren
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
    public DotDotDotToken DotDotDotToken { get; } = dotDotDotToken;
    public QuestionToken QuestionToken { get; } = questionToken;
    public ITypeNode Type { get; } = type;
    public IExpression Initializer { get; } = initializer;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [DotDotDotToken, Name, QuestionToken, Type, Initializer];
    }
}