namespace Serenity.TypeScript;

internal class TypeNodeBase : NodeBase, ITypeNode
{
    public TypeNodeBase(SyntaxKind kind)
    {
        Kind = kind;
    }
}

internal class ArrayTypeNode(ITypeNode elementType) : TypeNodeBase(SyntaxKind.ArrayType)
{
    public ITypeNode ElementType { get; set; } = elementType;
}

internal class TupleTypeNode(NodeArray<ITypeNode> elementTypes) : TypeNodeBase(SyntaxKind.TupleType)
{
    public NodeArray<ITypeNode> ElementTypes { get; set; } = elementTypes;
}

internal class UnionTypeNode(NodeArray<ITypeNode> types)
    : TypeNodeBase(SyntaxKind.UnionType), IUnionOrIntersectionTypeNode
{
    public NodeArray<ITypeNode> Types { get; set; } = types;
}

internal class IntersectionTypeNode(NodeArray<ITypeNode> types)
    : TypeNodeBase(SyntaxKind.IntersectionType), IUnionOrIntersectionTypeNode
{

    public NodeArray<ITypeNode> Types { get; set; } = types;
}

internal class ParenthesizedTypeNode(ITypeNode type) : TypeNodeBase(SyntaxKind.ParenthesizedType)
{
    public ITypeNode Type { get; set; } = type;
}

internal class TypeOperatorNode(SyntaxKind op, ITypeNode type) : TypeNodeBase(SyntaxKind.TypeOperator)
{
    public SyntaxKind Operator { get; } = op;
    public ITypeNode Type { get; } = type;
}

internal class IndexedAccessTypeNode(ITypeNode objectType, ITypeNode indexType) : TypeNodeBase(SyntaxKind.IndexedAccessType)
{
    public ITypeNode ObjectType { get; set; } = objectType;
    public ITypeNode IndexType { get; set; } = indexType;
}

internal class LiteralTypeNode(IExpression literal) : TypeNodeBase(SyntaxKind.LiteralType)
{
    public IExpression Literal { get; set; } = literal;
}


internal class TypeReferenceNode(IEntityName typeName, NodeArray<ITypeNode> typeArguments)
    : TypeNodeBase(SyntaxKind.TypeReference)
{
    public IEntityName TypeName { get; } = typeName;
    public NodeArray<ITypeNode> TypeArguments { get; } = typeArguments;
}

internal class FunctionOrConstructorTypeNodeBase(SyntaxKind kind, NodeArray<TypeParameterDeclaration> typeParameters,
    NodeArray<ParameterDeclaration> parameters, ITypeNode type)
        : SignatureDeclarationBase(kind, name: null, typeParameters, parameters, type), IFunctionOrConstructorTypeNode
{
}

internal class FunctionTypeNode(NodeArray<TypeParameterDeclaration> typeParameters, 
    NodeArray<ParameterDeclaration> parameters, ITypeNode type) 
    : FunctionOrConstructorTypeNodeBase(SyntaxKind.FunctionType, typeParameters, parameters, type)
{
}

internal class ConstructorTypeNode(NodeArray<IModifierLike> modifiers, NodeArray<TypeParameterDeclaration> typeParameters,
    NodeArray<ParameterDeclaration> parameters, ITypeNode type)
    : FunctionOrConstructorTypeNodeBase(SyntaxKind.ConstructorType, typeParameters, parameters, type), IHasModifiers
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
}

internal class ThisTypeNode() : TypeNodeBase(SyntaxKind.ThisType)
{
}

internal class TypePredicateNode(AssertsKeyword assertsModifier, INode parameterName, ITypeNode type)
    : TypeNodeBase(SyntaxKind.TypePredicate)
{
    public AssertsKeyword AssertsModifier { get; } = assertsModifier;
    public INode ParameterName { get; } = parameterName; // Identifier | ThisTypeNode
    public ITypeNode Type { get; } = type;
}

internal class RestTypeNode(ITypeNode type) : TypeNodeBase(SyntaxKind.RestType)
{
    public ITypeNode Type { get; } = type;
}

internal class OptionalTypeNode(ITypeNode type) : TypeNodeBase(SyntaxKind.OptionalType)
{
    public ITypeNode Type { get; } = type;
}

internal class MappedTypeNode : NodeBase, ITypeNode, IDeclaration
{
    public MappedTypeNode(Token readonlyToken, TypeParameterDeclaration typeParameter, ITypeNode nameType,
        Token questionToken, ITypeNode type, NodeArray<ITypeElement> members)
    {
        Kind = SyntaxKind.MappedType;
        ReadonlyToken = readonlyToken;
        TypeParameter = typeParameter;
        NameType = nameType;
        QuestionToken = questionToken;
        Type = type;
        Members = members;
    }

    public Token ReadonlyToken { get; }
    public TypeParameterDeclaration TypeParameter { get; }
    public Token QuestionToken { get; }
    public ITypeNode Type { get; }
    public INode Name { get; set; }
    public ITypeNode NameType { get; }
    public NodeArray<ITypeElement> Members { get; }
}


internal class NamedTupleMember(DotDotDotToken dotDotDotToken, Identifier name, QuestionToken questionToken, ITypeNode type)
    : TypeNodeBase(SyntaxKind.NamedTupleMember), IDeclaration
{
    public DotDotDotToken DotDotDotToken { get; set; } = dotDotDotToken;
    public Identifier Name { get; set; } = name;
    public QuestionToken QuestionToken { get; set; } = questionToken;
    public ITypeNode Type { get; set; } = type;
}


internal class InferTypeNode(TypeParameterDeclaration typeParameter) : TypeNodeBase(SyntaxKind.InferType), IInferTypeNode
{
    public TypeParameterDeclaration TypeParameter { get; } = typeParameter;
}

internal class ConditionalTypeNode(ITypeNode checkType, ITypeNode extendsType,
    ITypeNode trueType, ITypeNode falseType) : TypeNodeBase(SyntaxKind.ConditionalType)
{
    public ITypeNode CheckType { get; } = checkType;
    public ITypeNode ExtendsType { get; } = extendsType;
    public ITypeNode TrueType { get; } = trueType;
    public ITypeNode FalseType { get; } = falseType;
}
