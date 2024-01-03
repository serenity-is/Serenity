namespace Serenity.TypeScript;

public class MethodSignature(NodeArray<IModifierLike> modifiers, IDeclarationName name, QuestionToken questionToken,
    NodeArray<TypeParameterDeclaration> typeParameters, NodeArray<ParameterDeclaration> parameters, ITypeNode type)
    : SignatureDeclarationBase(SyntaxKind.MethodSignature, name, typeParameters, parameters, type), ITypeElement, IMethodOrAccessorDeclaration
{
    public NodeArray<IModifierLike> Modifiers { get; set; } = modifiers;
    public AsteriskToken AsteriskToken { get; set; }
    public IBlockOrExpression Body { get; set; } //  Block | Expression
    public QuestionToken QuestionToken { get; set; } = questionToken;

    public override IEnumerable<INode> GetRestChildren()
    {
        yield return Name;
        yield return QuestionToken;
        if (TypeParameters != null) foreach (var x in TypeParameters) yield return x;
        if (Parameters != null) foreach (var x in Parameters) yield return x;
        yield return Type;
    }
}
