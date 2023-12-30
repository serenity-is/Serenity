namespace Serenity.TypeScript;

internal class FunctionLikeDeclarationBase(SyntaxKind kind, IDeclarationName name,
    NodeArray<TypeParameterDeclaration> typeParameters, NodeArray<ParameterDeclaration> parameters, ITypeNode type,
    AsteriskToken asteriskToken = null, QuestionToken questionToken = null, ExclamationToken exclamationToken = null,
    IBlockOrExpression body = null) : SignatureDeclarationBase(kind, name, 
        typeParameters, parameters, type), IFunctionLikeDeclaration, IGetRestChildren
{
    public AsteriskToken AsteriskToken { get; set; } = asteriskToken;
    public QuestionToken QuestionToken { get; set; } = questionToken;
    public ExclamationToken ExclamationToken { get; set; } = exclamationToken;
    public IBlockOrExpression Body { get; set; } = body;//  Block | Expression

    public override IEnumerable<INode> GetRestChildren()
    {
        yield return AsteriskToken;
        foreach (var x in base.GetRestChildren()) yield return x;
        yield return QuestionToken;
        yield return ExclamationToken;
        yield return Body;
    }
}

internal class FunctionDeclaration(NodeArray<IModifierLike> modifiers, AsteriskToken asteriskToken, Identifier name,
    NodeArray<TypeParameterDeclaration> typeParameters, NodeArray<ParameterDeclaration> parameters, ITypeNode type, Block body)
    : FunctionLikeDeclarationBase(SyntaxKind.FunctionDeclaration, name, typeParameters, parameters, type,
        asteriskToken: asteriskToken, body: body), IDeclarationStatement, IHasModifiers
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
}

internal class MethodDeclaration(NodeArray<IModifierLike> modifiers, AsteriskToken asteriskToken, IPropertyName name,
    QuestionToken questionToken, NodeArray<TypeParameterDeclaration> typeParameters, NodeArray<ParameterDeclaration> parameters,
    ITypeNode type, Block body)
    : FunctionLikeDeclarationBase(SyntaxKind.MethodDeclaration, name, typeParameters, parameters, type,
        asteriskToken: asteriskToken, questionToken: questionToken, body: body),
    IMethodOrAccessorDeclaration, IObjectLiteralElementLike, IHasModifiers, IHasDecorators
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
}

internal class ConstructorDeclaration(NodeArray<IModifierLike> modifiers, NodeArray<ParameterDeclaration> parameters, Block body)
    : FunctionLikeDeclarationBase(SyntaxKind.Constructor, name: null, typeParameters: null, type: null,
        parameters: parameters, body: body), IClassElement, IHasModifiers
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
}


internal class GetAccessorDeclaration(NodeArray<IModifierLike> modifiers, IPropertyName name,
    NodeArray<ParameterDeclaration> parameters, ITypeNode type, Block body) :
    FunctionLikeDeclarationBase(SyntaxKind.GetAccessor, name, typeParameters: null,
        parameters, type, body: body), IAccessorDeclaration, IHasModifiers, IHasDecorators
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
}

internal class SetAccessorDeclaration(NodeArray<IModifierLike> modifiers, IPropertyName name,
    NodeArray<ParameterDeclaration> parameters, Block body)
    : FunctionLikeDeclarationBase(SyntaxKind.SetAccessor, name, typeParameters: null,
        parameters, type: null, body: body), IAccessorDeclaration, IHasModifiers, IHasDecorators
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
}

internal class FunctionExpression(NodeArray<IModifierLike> modifiers, AsteriskToken asteriskToken,
    Identifier name, NodeArray<TypeParameterDeclaration> typeParameters, NodeArray<ParameterDeclaration> parameters,
    ITypeNode type, Block body)
    : FunctionLikeDeclarationBase(SyntaxKind.FunctionExpression, name, typeParameters, parameters, 
        type, asteriskToken, body: body), IPrimaryExpression, IFunctionLikeDeclaration, IHasModifiers
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
}

internal class ArrowFunction(NodeArray<IModifierLike> modifiers, NodeArray<TypeParameterDeclaration> typeParameters, 
    NodeArray<ParameterDeclaration> parameters, ITypeNode type, EqualsGreaterThanToken equalsGreaterThanToken, 
    IBlockOrExpression body)
    : FunctionLikeDeclarationBase(SyntaxKind.ArrowFunction, name: null, typeParameters, parameters,
        type, body: body), IExpression, IFunctionLikeDeclaration, IHasModifiers, IGetRestChildren
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
    public EqualsGreaterThanToken EqualsGreaterThanToken { get; } = equalsGreaterThanToken;

    public override IEnumerable<INode> GetRestChildren()
    {
        foreach (var x in base.GetRestChildren().Where(x => x != Body)) yield return x;
        yield return EqualsGreaterThanToken;
        yield return Body;
    }
}
