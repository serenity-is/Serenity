
namespace Serenity.TypeScript;

internal class Declaration(SyntaxKind kind) : Node(kind), IDeclaration
{
}

internal class PropertyLikeDeclaration(SyntaxKind kind) : Declaration(kind)
{
}

internal class NamedDeclaration<TName>(SyntaxKind kind, TName name) : Node(kind), INamedDeclaration, IGetRestChildren
    where TName : IDeclarationName

{
    public TName Name { get; set; } = name;

    IDeclarationName IHasNameProperty.Name => Name;

    public virtual IEnumerable<INode> GetRestChildren()
    {
        return [Name];
    }
}

internal class ClassElement(SyntaxKind kind, IPropertyName name) 
    : NamedDeclaration<IPropertyName>(kind, name), IClassElement
{
}

internal class SemicolonClassElement() : ClassElement(SyntaxKind.SemicolonClassElement, name: null)
{
}

internal class TypeElement(SyntaxKind kind, IPropertyName name, QuestionToken questionToken)
    : NamedDeclaration<IPropertyName>(kind, name), ITypeElement, IGetRestChildren
{
    public QuestionToken QuestionToken { get; set; } = questionToken;

    public override IEnumerable<INode> GetRestChildren() 
    {
        return [Name, QuestionToken];
    }
}

internal class MissingDeclaration() : NamedDeclaration<Identifier>(SyntaxKind.MissingDeclaration, null), 
    IDeclarationStatement, IClassElement, IObjectLiteralElement, ITypeElement, IHasModifiers, IGetRestChildren, IPrimaryExpression
{
    public NodeArray<IModifierLike> Modifiers { get; set; }
    public QuestionToken QuestionToken { get; set; }

    public override IEnumerable<INode> GetRestChildren() 
    {
        return [Name, QuestionToken];
    }
}

internal class BindingElement(DotDotDotToken dotDotDotToken, IPropertyName propertyName, IBindingName name, IExpression initializer)
    : NamedDeclaration<IBindingName>(SyntaxKind.BindingElement, name), 
    IArrayBindingElement, IVariableLikeDeclaration, IGetRestChildren
{
    public DotDotDotToken DotDotDotToken { get; } = dotDotDotToken;
    public IPropertyName PropertyName { get; } = propertyName;
    public IExpression Initializer { get; } = initializer;

    public override IEnumerable<INode> GetRestChildren() 
    {
        return [DotDotDotToken, PropertyName, Name, Initializer];
    }
}

internal class TypeParameterDeclaration(NodeArray<IModifierLike> modifiers, Identifier name,
    ITypeNode constraint, ITypeNode defaultType = null) 
    : NamedDeclaration<Identifier>(SyntaxKind.TypeParameter, name), IHasModifiers, IGetRestChildren
{
    public NodeArray<IModifierLike> Modifiers { get; set; } = modifiers;
    public ITypeNode Constraint { get; } = constraint;
    public ITypeNode Default { get; } = defaultType;
    public IExpression Expression { get; set; }

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Name, Constraint, Default, Expression];
    }
}

internal class VariableDeclaration(IBindingName name, ExclamationToken exclamationToken, ITypeNode type, IExpression initializer) 
    : NamedDeclaration<IBindingName>(SyntaxKind.VariableDeclaration, name), IVariableLikeDeclaration, IGetRestChildren
{
    public ExclamationToken ExclamationToken { get; } = exclamationToken;
    public ITypeNode Type { get; set; } = type;
    public IExpression Initializer { get; set; } = initializer;
    
    public override IEnumerable<INode> GetRestChildren()
    {
        return [Name, ExclamationToken, Type, Initializer];
    }
}

internal class PropertyDeclaration(NodeArray<IModifierLike> modifiers, IPropertyName name,
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

internal class ParameterDeclaration(NodeArray<IModifierLike> modifiers, DotDotDotToken dotDotDotToken,
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

internal class HeritageClause(SyntaxKind token, NodeArray<ExpressionWithTypeArguments> types)
    : Node(SyntaxKind.HeritageClause), IGetRestChildren
{
    public SyntaxKind Token { get; set; } = token; //  SyntaxKind.ExtendsKeyword | SyntaxKind.ImplementsKeyword
    public NodeArray<ExpressionWithTypeArguments> Types { get; } = types;

    public IEnumerable<INode> GetRestChildren()
    {
        return Types;
    }
}

internal class ClassLikeDeclarationBase<TName>(SyntaxKind kind, TName name,
    NodeArray<TypeParameterDeclaration> typeParameters, NodeArray<HeritageClause> heritageClauses,
    NodeArray<IClassElement> members)
    : NamedDeclaration<TName>(kind, name), IClassLikeDeclaration, IGetRestChildren
    where TName: IDeclarationName
{
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; } = typeParameters;
    public NodeArray<HeritageClause> HeritageClauses { get; } = heritageClauses;
    public NodeArray<IClassElement> Members { get; } = members;

    public override IEnumerable<INode> GetRestChildren()
    {
        yield return Name;
        if (TypeParameters != null) foreach (var x in TypeParameters) yield return x;
        if (HeritageClauses != null) foreach (var x in HeritageClauses) yield return x;
        if (Members != null) foreach (var x in Members) yield return x;
    }
}

internal class ClassDeclaration(NodeArray<IModifierLike> modifiers, Identifier name,
    NodeArray<TypeParameterDeclaration> typeParameters, NodeArray<HeritageClause> heritageClauses,
    NodeArray<IClassElement> members)
    : ClassLikeDeclarationBase<Identifier>(SyntaxKind.ClassDeclaration, name, typeParameters, heritageClauses,
        members), IClassLikeDeclaration, IDeclarationStatement, IHasDecorators, IHasModifiers
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
}

internal class ClassExpression(NodeArray<IModifierLike> modifiers, Identifier name,
    NodeArray<TypeParameterDeclaration> typeParameters, NodeArray<HeritageClause> heritageClauses,
    NodeArray<IClassElement> members)
     : ClassLikeDeclarationBase<Identifier>(SyntaxKind.ClassDeclaration, name, typeParameters, heritageClauses,
        members), IClassLikeDeclaration, IPrimaryExpression, IHasDecorators, IHasModifiers
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
}

internal class DeclarationStatement<TName>(SyntaxKind kind, TName name)
    : NamedDeclaration<TName>(kind, name), IDeclarationStatement, IStatement, INamedDeclaration
    where TName : IDeclarationName
{
}

internal class InterfaceDeclaration(NodeArray<IModifierLike> modifiers, Identifier name,
    NodeArray<TypeParameterDeclaration> typeParameters, NodeArray<HeritageClause> heritageClauses,
    NodeArray<ITypeElement> members) : DeclarationStatement<Identifier>(SyntaxKind.InterfaceDeclaration, name), 
        IHasModifiers, IGetRestChildren, IObjectTypeDeclaration, IDeclarationWithTypeParameterChildren
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; } = typeParameters;
    public NodeArray<HeritageClause> HeritageClauses { get; set; } = heritageClauses;
    public NodeArray<ITypeElement> Members { get; set; } = members;

    public override IEnumerable<INode> GetRestChildren()
    {
        yield return Name;
        if (TypeParameters != null) foreach (var x in TypeParameters) yield return x;
        if (HeritageClauses != null) foreach (var x in HeritageClauses) yield return x;
        if (Members != null) foreach (var x in Members) yield return x;
    }
}

internal class TypeAliasDeclaration(NodeArray<IModifierLike> modifiers, Identifier name,
    NodeArray<TypeParameterDeclaration> typeParameters, ITypeNode type)
    : DeclarationStatement<Identifier>(SyntaxKind.TypeAliasDeclaration, name), IHasModifiers, IGetRestChildren,
    IDeclarationWithTypeParameterChildren
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; } = typeParameters;
    public ITypeNode Type { get; } = type;

    public override IEnumerable<INode> GetRestChildren()
    {
        yield return Name;
        if (TypeParameters != null) foreach (var x in TypeParameters) yield return x;
        yield return Type;
    }
}

internal class EnumMember(IPropertyName name, IExpression initializer) 
    : NamedDeclaration<IPropertyName>(SyntaxKind.EnumMember, name), IGetRestChildren
{
    public IExpression Initializer { get; } = initializer;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Name, Initializer];
    }
}


internal class EnumDeclaration(NodeArray<IModifierLike> modifiers, Identifier name,
    NodeArray<EnumMember> members) : DeclarationStatement<Identifier>(SyntaxKind.EnumDeclaration, name), 
        IHasModifiers, IGetRestChildren
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
    public NodeArray<EnumMember> Members { get; set; } = members;

    public override IEnumerable<INode> GetRestChildren()
    {
        yield return Name;
        if (Members != null) foreach (var x in Members) yield return x;
    }
}

internal class ModuleDeclaration(NodeArray<IModifierLike> modifiers, IModuleName name, IModuleBody body) 
    : DeclarationStatement<IModuleName>(SyntaxKind.ModuleDeclaration, name), IHasModifiers, IGetRestChildren
{
    public ModuleDeclaration(NodeArray<IModifierLike> modifiers, IModuleName name, IModuleBody body, NodeFlags flags)
        : this(modifiers, name, body)
    {
        Flags |= flags;
    }

    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
    public IModuleBody Body { get; set; } = body;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Name, Body];
    }
}

internal class NamespaceDeclaration(NodeArray<IModifierLike> modifiers, Identifier name, IModuleBody body, NodeFlags flags)
    : ModuleDeclaration(modifiers, name, body, flags), INamespaceBody
{
}

internal class NamespaceExportDeclaration(Identifier name)
    : DeclarationStatement<Identifier>(SyntaxKind.NamespaceExportDeclaration, name), IHasModifiers
{
    public NodeArray<IModifierLike> Modifiers { get; set; }
}

internal class VariableDeclarationList(NodeArray<VariableDeclaration> declarations)
    : Node(SyntaxKind.VariableDeclarationList), IVariableDeclarationList, IGetRestChildren, IForInitializer
{

    public VariableDeclarationList(NodeArray<VariableDeclaration> declarations, NodeFlags nodeFlags)
        : this(declarations)
    {
        Flags &= nodeFlags;
    }

    public NodeArray<VariableDeclaration> Declarations { get; } = declarations;
    public NodeFlags NodeFlags { get; }

    public IEnumerable<INode> GetRestChildren()
    {
        return Declarations;
    }
}

internal class ClassStaticBlockDeclaration(Block body)
    : ClassElement(SyntaxKind.ClassStaticBlockDeclaration, name: null), IGetRestChildren, IHasModifiers
{
    public NodeArray<IModifierLike> Modifiers { get; set; }
    public Block Body { get; } = body;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Name, Body];
    }
}
