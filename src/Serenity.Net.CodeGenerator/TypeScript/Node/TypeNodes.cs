
namespace Serenity.TypeScript;

internal class TypeNodeBase : Node, ITypeNode
{
    public TypeNodeBase(SyntaxKind kind)
    {
        Kind = kind;
    }
}

internal class KeywordTypeNode() : TypeNodeBase(SyntaxKind.Unknown), IKeywordTypeNode
{
}

internal class ArrayTypeNode(ITypeNode elementType) 
    : TypeNodeBase(SyntaxKind.ArrayType), IGetRestChildren
{
    public ITypeNode ElementType { get; set; } = elementType;

    public IEnumerable<INode> GetRestChildren()
    {
        return [ElementType];
    }
}

internal class TupleTypeNode(NodeArray<ITypeNode> elementTypes) 
    : TypeNodeBase(SyntaxKind.TupleType), IGetRestChildren
{
    public NodeArray<ITypeNode> ElementTypes { get; set; } = elementTypes;

    public IEnumerable<INode> GetRestChildren()
    {
        return ElementTypes;
    }
}

internal class UnionTypeNode(NodeArray<ITypeNode> types)
    : TypeNodeBase(SyntaxKind.UnionType), IUnionOrIntersectionTypeNode, IGetRestChildren
{
    public NodeArray<ITypeNode> Types { get; set; } = types;

    public IEnumerable<INode> GetRestChildren()
    {
        return Types;
    }
}

internal class IntersectionTypeNode(NodeArray<ITypeNode> types)
    : TypeNodeBase(SyntaxKind.IntersectionType), IUnionOrIntersectionTypeNode, IGetRestChildren
{
    public NodeArray<ITypeNode> Types { get; set; } = types;

    public IEnumerable<INode> GetRestChildren()
    {
        return Types;
    }
}

internal class ParenthesizedTypeNode(ITypeNode type) 
    : TypeNodeBase(SyntaxKind.ParenthesizedType), IGetRestChildren
{
    public ITypeNode Type { get; set; } = type;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Type];
    }
}

internal class TypeOperatorNode(SyntaxKind op, ITypeNode type) 
    : TypeNodeBase(SyntaxKind.TypeOperator), IGetRestChildren
{
    public SyntaxKind Operator { get; } = op;
    public ITypeNode Type { get; } = type;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Type];
    }
}

internal class IndexedAccessTypeNode(ITypeNode objectType, ITypeNode indexType) 
    : TypeNodeBase(SyntaxKind.IndexedAccessType), IGetRestChildren
{
    public ITypeNode ObjectType { get; set; } = objectType;
    public ITypeNode IndexType { get; set; } = indexType;

    public IEnumerable<INode> GetRestChildren()
    {
        return [ObjectType, IndexType];
    }
}

internal class LiteralTypeNode(IExpression literal) 
    : TypeNodeBase(SyntaxKind.LiteralType), IGetRestChildren
{
    public IExpression Literal { get; set; } = literal;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Literal];
    }
}


internal class TypeReferenceNode(IEntityName typeName, NodeArray<ITypeNode> typeArguments)
    : TypeNodeBase(SyntaxKind.TypeReference), IGetRestChildren
{
    public IEntityName TypeName { get; } = typeName;
    public NodeArray<ITypeNode> TypeArguments { get; } = typeArguments;

    public IEnumerable<INode> GetRestChildren()
    {
        yield return TypeName;
        if (TypeArguments != null) foreach (var x in TypeArguments) yield return x;
    }
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
    : TypeNodeBase(SyntaxKind.TypePredicate), IGetRestChildren
{
    public AssertsKeyword AssertsModifier { get; } = assertsModifier;
    public INode ParameterName { get; } = parameterName; // Identifier | ThisTypeNode
    public ITypeNode Type { get; } = type;

    public IEnumerable<INode> GetRestChildren()
    {
        return [AssertsModifier, ParameterName, Type];
    }
}

internal class RestTypeNode(ITypeNode type) 
    : TypeNodeBase(SyntaxKind.RestType), IGetRestChildren
{
    public ITypeNode Type { get; } = type;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Type];
    }
}

internal class OptionalTypeNode(ITypeNode type) 
    : TypeNodeBase(SyntaxKind.OptionalType), IGetRestChildren
{
    public ITypeNode Type { get; } = type;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Type];
    }
}

internal class MappedTypeNode(Token readonlyToken, TypeParameterDeclaration typeParameter, ITypeNode nameType,
        Token questionToken, ITypeNode type, NodeArray<ITypeElement> members) 
    : Node(SyntaxKind.MappedType), ITypeNode, IDeclaration, IGetRestChildren
{
    public Token ReadonlyToken { get; } = readonlyToken;
    public TypeParameterDeclaration TypeParameter { get; } = typeParameter;
    public ITypeNode NameType { get; } = nameType;
    public Token QuestionToken { get; } = questionToken;
    public ITypeNode Type { get; } = type;
    public NodeArray<ITypeElement> Members { get; } = members;

    public IEnumerable<INode> GetRestChildren()
    {
        yield return ReadonlyToken;
        yield return TypeParameter;
        yield return NameType;
        yield return QuestionToken;
        yield return Type;
        if (Members != null) foreach (var x in Members) yield return x;
    }
}


internal class NamedTupleMember(DotDotDotToken dotDotDotToken, Identifier name, QuestionToken questionToken, ITypeNode type)
    : TypeNodeBase(SyntaxKind.NamedTupleMember), IDeclaration, IGetRestChildren
{
    public DotDotDotToken DotDotDotToken { get; set; } = dotDotDotToken;
    public Identifier Name { get; set; } = name;
    public QuestionToken QuestionToken { get; set; } = questionToken;
    public ITypeNode Type { get; set; } = type;

    public IEnumerable<INode> GetRestChildren()
    {
        return [DotDotDotToken, Name, QuestionToken, Type];
    }
}

internal class InferTypeNode(TypeParameterDeclaration typeParameter) 
    : TypeNodeBase(SyntaxKind.InferType), IInferTypeNode, IGetRestChildren
{
    public TypeParameterDeclaration TypeParameter { get; } = typeParameter;

    public IEnumerable<INode> GetRestChildren()
    {
        return [TypeParameter];
    }
}

internal class ConditionalTypeNode(ITypeNode checkType, ITypeNode extendsType,
    ITypeNode trueType, ITypeNode falseType)
    : TypeNodeBase(SyntaxKind.ConditionalType), IGetRestChildren
{
    public ITypeNode CheckType { get; } = checkType;
    public ITypeNode ExtendsType { get; } = extendsType;
    public ITypeNode TrueType { get; } = trueType;
    public ITypeNode FalseType { get; } = falseType;

    public IEnumerable<INode> GetRestChildren()
    {
        return [CheckType, ExtendsType, TrueType, FalseType];
    }
}
