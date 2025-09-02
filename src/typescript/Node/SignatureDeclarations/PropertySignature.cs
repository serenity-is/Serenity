namespace Serenity.TypeScript;

public class PropertySignature(NodeArray<IModifierLike> modifiers, IPropertyName name,
    QuestionToken questionToken, ITypeNode type)
    : TypeElement(SyntaxKind.PropertySignature, name, questionToken), IVariableLikeDeclaration, IHasModifiers, IGetRestChildren
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
    public ITypeNode Type { get; set; } = type;
    public IExpression Initializer { get; set; } // error reporting only

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Name, QuestionToken, Type, Initializer];
    }
}