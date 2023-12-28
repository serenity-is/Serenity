namespace Serenity.TypeScript;

internal class Declaration(SyntaxKind kind) : NodeBase(kind), IDeclaration
{
}

internal class PropertyLikeDeclaration(SyntaxKind kind) : Declaration(kind)
{
}

internal class NamedDeclaration(SyntaxKind kind, IDeclarationName name) : NodeBase(kind), INamedDeclaration
{
    public IDeclarationName Name { get; set; } = name;
}

internal class ClassElement(SyntaxKind kind, IDeclarationName name) : NamedDeclaration(kind, name), IClassElement
{
}

internal class SemicolonClassElement(IDeclarationName name) : ClassElement(SyntaxKind.SemicolonClassElement, name)
{
}


internal class TypeElement(SyntaxKind kind, IDeclarationName name, QuestionToken questionToken)
    : NamedDeclaration(kind, name), ITypeElement
{
    public QuestionToken QuestionToken { get; set; } = questionToken;
}

internal class MissingDeclaration() : NamedDeclaration(SyntaxKind.MissingDeclaration, null), IDeclarationStatement, IClassElement, IObjectLiteralElement, ITypeElement, IHasModifiers
{
    public QuestionToken QuestionToken { get; set; }
    public NodeArray<IModifierLike> Modifiers { get; set; }
}

internal class BindingElement(DotDotDotToken dotDotDotToken, IPropertyName propertyName, IDeclarationName name, IExpression initializer)
    : NamedDeclaration(SyntaxKind.BindingElement, name), IArrayBindingElement, IVariableLikeDeclaration
{
    public DotDotDotToken DotDotDotToken { get; } = dotDotDotToken;
    public IPropertyName PropertyName { get; } = propertyName;
    public IExpression Initializer { get; } = initializer;
}

internal class TypeParameterDeclaration(NodeArray<IModifierLike> modifiers, Identifier name,
    ITypeNode constraint, ITypeNode defaultType) : NamedDeclaration(SyntaxKind.TypeParameter, name), IHasModifiers
{
    public NodeArray<IModifierLike> Modifiers { get; set; } = modifiers;
    public ITypeNode Constraint { get; } = constraint;
    public ITypeNode Default { get; } = defaultType;
    public IExpression Expression { get; set; }
}

internal class VariableDeclaration(IBindingName name, ExclamationToken exclamationToken, ITypeNode type, IExpression initializer) 
    : NamedDeclaration(SyntaxKind.VariableDeclaration, name), IVariableLikeDeclaration
{
    public ExclamationToken ExclamationToken { get; } = exclamationToken;
    public ITypeNode Type { get; set; } = type;
    public IExpression Initializer { get; set; } = initializer;
}

internal class PropertyDeclaration(NodeArray<IModifierLike> modifiers, IPropertyName name,
    Token questionOrExclamationToken, ITypeNode type, IExpression initializer)
    : ClassElement(SyntaxKind.PropertyDeclaration, name), IVariableLikeDeclaration, IHasModifiers, IHasDecorators
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
    public INode QuestionToken { get; } = questionOrExclamationToken is { Kind: SyntaxKind.QuestionToken } q ? q : null;
    public INode ExclamationToken { get; } = questionOrExclamationToken is { Kind: SyntaxKind.ExclamationToken } q ? q : null;
    public ITypeNode Type { get; } = type;
    public IExpression Initializer { get; } = initializer;
}

internal class ParameterDeclaration(NodeArray<IModifierLike> modifiers, DotDotDotToken dotDotDotToken,
    IBindingName name, QuestionToken questionToken, ITypeNode type, IExpression initializer)
    : NamedDeclaration(SyntaxKind.Parameter, name), IVariableLikeDeclaration, IHasModifiers, IHasDecorators
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
    public DotDotDotToken DotDotDotToken { get; } = dotDotDotToken;
    public QuestionToken QuestionToken { get; } = questionToken;
    public ITypeNode Type { get; } = type;
    public IExpression Initializer { get; } = initializer;
}

internal class ClassLikeDeclarationBase(SyntaxKind kind, IDeclarationName name,
    NodeArray<TypeParameterDeclaration> typeParameters, NodeArray<HeritageClause> heritageClauses,
    NodeArray<IClassElement> members)
    : NamedDeclaration(kind, name), IClassLikeDeclaration
{
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; } = typeParameters;
    public NodeArray<HeritageClause> HeritageClauses { get; } = heritageClauses;
    public NodeArray<IClassElement> Members { get; } = members;
}

internal class ClassDeclaration(NodeArray<IModifierLike> modifiers, Identifier name,
    NodeArray<TypeParameterDeclaration> typeParameters, NodeArray<HeritageClause> heritageClauses,
    NodeArray<IClassElement> members)
    : ClassLikeDeclarationBase(SyntaxKind.ClassDeclaration, name, typeParameters, heritageClauses,
        members), IClassLikeDeclaration, IDeclarationStatement, IHasDecorators, IHasModifiers
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
}

internal class ClassExpression(NodeArray<IModifierLike> modifiers, Identifier name,
    NodeArray<TypeParameterDeclaration> typeParameters, NodeArray<HeritageClause> heritageClauses,
    NodeArray<IClassElement> members)
     : ClassLikeDeclarationBase(SyntaxKind.ClassDeclaration, name, typeParameters, heritageClauses,
        members), IClassLikeDeclaration, IPrimaryExpression, IHasDecorators, IHasModifiers
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
}