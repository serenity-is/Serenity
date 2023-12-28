namespace Serenity.TypeScript;

internal class SignatureDeclarationBase(SyntaxKind kind, IDeclarationName name, NodeArray<TypeParameterDeclaration> typeParameters,
    NodeArray<ParameterDeclaration> parameters, ITypeNode type) : NamedDeclaration(kind, name), ISignatureDeclaration
{
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; } = typeParameters;
    public NodeArray<ParameterDeclaration> Parameters { get; set; } = parameters;
    public ITypeNode Type { get; set; } = type;
}

internal class MethodSignature(NodeArray<IModifierLike> modifiers, IDeclarationName name, QuestionToken questionToken,
    NodeArray<TypeParameterDeclaration> typeParameters, NodeArray<ParameterDeclaration> parameters, ITypeNode type)
    : SignatureDeclarationBase(SyntaxKind.MethodSignature, name, typeParameters, parameters, type), ITypeElement, IMethodOrAccessorDeclaration
{
    public NodeArray<IModifierLike> Modifiers { get; set; } = modifiers;
    public AsteriskToken AsteriskToken { get; set; }
    public IBlockOrExpression Body { get; set; } //  Block | Expression
    public QuestionToken QuestionToken { get; set; } = questionToken;
}

internal class CallSignatureDeclaration : SignatureDeclarationBase, ISignatureDeclaration, ITypeElement
{
    public CallSignatureDeclaration(NodeArray<TypeParameterDeclaration> typeParameters, NodeArray<ParameterDeclaration> parameters, ITypeNode type)
        : base(SyntaxKind.CallSignature, name: null, typeParameters, parameters, type)
    {
        Kind = SyntaxKind.CallSignature;
        TypeParameters = typeParameters;
        Parameters = parameters;
        Type = type;
    }

    public QuestionToken QuestionToken { get; set; }
}

internal class ConstructSignatureDeclaration(NodeArray<TypeParameterDeclaration> typeParameters, NodeArray<ParameterDeclaration> parameters, ITypeNode type)
    : SignatureDeclarationBase(SyntaxKind.ConstructSignature, name: null, typeParameters, parameters, type), ISignatureDeclaration, ITypeElement
{
    public QuestionToken QuestionToken { get; set; }
}

internal class IndexSignatureDeclaration(NodeArray<IModifierLike> modifiers, NodeArray<ParameterDeclaration> parameters, ITypeNode type)
    : SignatureDeclarationBase(SyntaxKind.IndexSignature, name: null, null, parameters, type), IClassElement, ITypeElement, IHasModifiers
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
    public QuestionToken QuestionToken { get; set; }
}


internal class PropertySignature(NodeArray<IModifierLike> modifiers, IPropertyName name,
    QuestionToken questionToken, ITypeNode type)
    : TypeElement(SyntaxKind.PropertySignature, name, questionToken), IVariableLikeDeclaration, IHasModifiers
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
    public ITypeNode Type { get; set; } = type;
    public IExpression Initializer { get; set; } // error reporting only
}