namespace Serenity.TypeScript;

internal class SignatureDeclarationBase(SyntaxKind kind, IDeclarationName name, NodeArray<TypeParameterDeclaration> typeParameters,
    NodeArray<ParameterDeclaration> parameters, ITypeNode type)
    : NamedDeclaration<IDeclarationName>(kind, name), ISignatureDeclaration, IGetRestChildren
{
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; } = typeParameters;
    public NodeArray<ParameterDeclaration> Parameters { get; set; } = parameters;
    public ITypeNode Type { get; set; } = type;

    public override IEnumerable<INode> GetRestChildren()
    {
        yield return Name;
        if (TypeParameters != null) foreach (var x in TypeParameters) yield return x;
        if (Parameters != null) foreach (var x in Parameters) yield return x;
        yield return Type;
    }
}

internal class MethodSignature(NodeArray<IModifierLike> modifiers, IDeclarationName name, QuestionToken questionToken,
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

internal class CallSignatureDeclaration(NodeArray<TypeParameterDeclaration> typeParameters,
    NodeArray<ParameterDeclaration> parameters, ITypeNode type)
    : SignatureDeclarationBase(SyntaxKind.CallSignature, name: null, 
        typeParameters, parameters, type), ISignatureDeclaration, ITypeElement
{
}

internal class ConstructSignatureDeclaration(NodeArray<TypeParameterDeclaration> typeParameters, 
    NodeArray<ParameterDeclaration> parameters, ITypeNode type)
    : SignatureDeclarationBase(SyntaxKind.ConstructSignature, name: null, 
        typeParameters, parameters, type), ISignatureDeclaration, ITypeElement
{
}

internal class IndexSignatureDeclaration(NodeArray<IModifierLike> modifiers, NodeArray<ParameterDeclaration> parameters, ITypeNode type)
    : SignatureDeclarationBase(SyntaxKind.IndexSignature, name: null, null, parameters, type), IClassElement, ITypeElement, IHasModifiers
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
}


internal class PropertySignature(NodeArray<IModifierLike> modifiers, IPropertyName name,
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