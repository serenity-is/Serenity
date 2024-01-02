namespace Serenity.TypeScript;

public class ArrowFunction(NodeArray<IModifierLike> modifiers, NodeArray<TypeParameterDeclaration> typeParameters,
    NodeArray<ParameterDeclaration> parameters, ITypeNode type, EqualsGreaterThanToken equalsGreaterThanToken,
    IBlockOrExpression body) : FunctionLikeDeclarationBase(SyntaxKind.ArrowFunction, 
        name: null, typeParameters, parameters,type, body: body), 
            IExpression, IFunctionLikeDeclaration, IHasModifiers, IGetRestChildren
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
