using CommentKind = Serenity.TypeScript.TsTypes.SyntaxKind;

namespace Serenity.TypeScript.TsTypes;

public class Map<T> : Dictionary<string, T>
{
}

public class MapLike<T> : Dictionary<string, T>
{
}

public class ReadonlyArray<T> : List<T>
{
}

public class OpenBracketToken : Token
{
}

public class DotDotDotToken : Token
{
    public DotDotDotToken()
    {
        Kind = SyntaxKind.DotDotDotToken;
    }
}

public class DotToken : Token
{
    public DotToken()
    {
        Kind = SyntaxKind.DotToken;
    }
}

public class QuestionToken : Token
{
    public QuestionToken()
    {
        Kind = SyntaxKind.QuestionToken;
    }
}

public class ColonToken : Token
{
    public ColonToken()
    {
        Kind = SyntaxKind.ColonToken;
    }
}

public class EqualsToken : Token
{
    public EqualsToken()
    {
        Kind = SyntaxKind.EqualsToken;
    }
}

public class AsteriskToken : Token
{
    public AsteriskToken()
    {
        Kind = SyntaxKind.AsteriskToken;
    }
}

public class EqualsGreaterThanToken : Token
{
    public EqualsGreaterThanToken()
    {
        Kind = SyntaxKind.EqualsGreaterThanToken;
    }
}

public class EndOfFileToken : Token
{
    public EndOfFileToken()
    {
        Kind = SyntaxKind.EndOfFileToken;
    }
}

public class AtToken : Token
{
    public AtToken()
    {
        Kind = SyntaxKind.AtToken;
    }
}

public class ReadonlyToken : Token
{
    public ReadonlyToken()
    {
        Kind = SyntaxKind.ReadonlyKeyword;
    }
}

public class AwaitKeywordToken : Token
{
    public AwaitKeywordToken()
    {
        Kind = SyntaxKind.AwaitKeyword;
    }
}

public class Modifier : Node
{
}

public class ModifiersArray : NodeArray<Modifier>
{
}

public interface IEntityName : INode
{
}

public interface IPropertyName : INode
{
}

public interface IDeclarationName : INode
{
}

public interface IBindingName : INode
{
}

public interface IObjectLiteralElementLike : INode
{
}

public interface IBindingPattern : INode
{
    NodeArray<IArrayBindingElement> Elements { get; set; }
}

public interface IArrayBindingElement : INode
{
}

public interface IAccessorDeclaration : ISignatureDeclaration, IClassElement, IObjectLiteralElementLike
{
    //object _classElementBrand { get; set; }
    IBlockOrExpression Body { get; set; } //  Block | Expression
}

public interface IFunctionOrConstructorTypeNode : ISignatureDeclaration, ITypeNode
{
}

public interface IUnionOrIntersectionTypeNode : ITypeNode
{
    NodeArray<ITypeNode> Types { get; set; }
}

//{
//    //public BinaryOperatorToken() { kind = SyntaxKind.BinaryOperator; }
//}
public class AssignmentOperatorToken : Token
{
}

public interface IDestructuringAssignment : INode // AssignmentExpression
{
}

public interface IBindingOrAssignmentElement : INode
{
}

public interface IBindingOrAssignmentElementRestIndicator : INode
{
}

public interface IBindingOrAssignmentElementTarget : INode
{
}

public interface IObjectBindingOrAssignmentPattern : INode
{
}

public interface IArrayBindingOrAssignmentPattern : INode
{
}

public interface IAssignmentPattern : INode
{
}

public interface IBindingOrAssignmentPattern : INode
{
}

public class FunctionBody : Block
{
}

public interface IConciseBody : INode
{
}

public interface ITemplateLiteral : INode
{
}

public interface IEntityNameExpression : INode
{
}

public interface IEntityNameOrEntityNameExpression : INode
{
}

public interface ISuperProperty : INode
{
}

public interface ICallLikeExpression : INode
{
}

public interface IAssertionExpression : IExpression
{
}

public interface IJsxOpeningLikeElement : IExpression
{
}

public interface IJsxAttributeLike : IObjectLiteralElement
{
}

public interface IJsxTagNameExpression : IExpression
{
}

public interface IJsxChild : INode
{
}

public interface IBlockLike : INode
{
}

public interface IForInitializer : INode
{
}

public interface IBreakOrContinueStatement : IStatement
{
    Identifier Label { get; set; }
}

public interface ICaseOrDefaultClause : INode
{
}

public interface IDeclarationWithTypeParameters : INode
{
}

public interface IModuleName : INode
{
}

public interface IModuleBody : INode
{
}

public interface INamespaceBody : INode
{
}

public interface IJsDocNamespaceBody : INode
{
}

public interface IModuleReference : INode
{
}

public interface INamedImportBindings : INode, INamedImportsOrExports
{
}

public interface INamedImportsOrExports : INode
{
}

public interface IImportOrExportSpecifier : IDeclaration
{
    Identifier PropertyName { get; set; }
}

public interface IJsDocTypeReferencingNode : IJsDocType
{
}

public class FlowType : object
{
}

public class ITypePredicate : TypePredicateBase
{
}

public interface IAnyImportSyntax : INode
{
}

public class SymbolTable : Map
{
}

public interface IDestructuringPattern : INode
{
}

public interface IBaseType : IType
{
}

public interface IStructuredType : IType
{
}

public class CompilerOptionsValue : object
{
}

public class CommandLineOption : CommandLineOptionBase
{
}

public class TransformerFactory<T>
{
}

public class Transformer
{
}

public class Visitor
{
}

public class VisitResult : object
{
}

public class MapLike
{
}

public class Map
{
    public int Size { get; set; }
}

public class Iterator
{
}

public class FileMap
{
}

public interface ITextRange
{
    int? Pos { get; set; }
    int? End { get; set; }
}

public class TextRange : ITextRange
{
    public int? Pos { get; set; }
    public int? End { get; set; }
}

public class NodeArray<T> : List<T>, ITextRange
{
    public NodeArray()
    {
    }

    public NodeArray(T[] elements)
        : base(elements.ToList())
    {
    }

    public bool HasTrailingComma { get; set; }
    public TransformFlags TransformFlags { get; set; }
    public int? Pos { get; set; }
    public int? End { get; set; }
}

public class Token : Node
{
}

public class Identifier : PrimaryExpression, IJsxTagNameExpression, IEntityName, IPropertyName
{
    public Identifier()
    {
        Kind = SyntaxKind.Identifier;
    }

    public string Text { get; set; }
    public SyntaxKind OriginalKeywordKind { get; set; }
    public GeneratedIdentifierKind AutoGenerateKind { get; set; }
    public int AutoGenerateId { get; set; }
    public bool IsInJsDocNamespace { get; set; }
}

public class TransientIdentifier : Identifier
{
    public Symbol ResolvedSymbol { get; set; }
}

public class GeneratedIdentifier : Identifier
{
}

public class QualifiedName : Node, IEntityName
{
    public QualifiedName()
    {
        Kind = SyntaxKind.QualifiedName;
    }

    public IEntityName Left { get; set; }
    public Identifier Right { get; set; }
}

public interface IDeclaration : INode
{
    object DeclarationBrand { get; set; }
    INode Name { get; set; }
}

public class Declaration : Node, IDeclaration
{
    public object DeclarationBrand { get; set; }
    public INode Name { get; set; }
}

public interface IDeclarationStatement : INode, IDeclaration, IStatement
{
    //Node name { get; set; } // Identifier | StringLiteral | NumericLiteral
}

public class DeclarationStatement : Node, IDeclarationStatement, IDeclaration, IStatement
{
    public object DeclarationBrand { get; set; }
    public INode Name { get; set; }
    public object StatementBrand { get; set; }
}

public class ComputedPropertyName : Node, IPropertyName
{
    public ComputedPropertyName()
    {
        Kind = SyntaxKind.ComputedPropertyName;
    }

    public IExpression Expression { get; set; }
}

public class Decorator : Node
{
    public Decorator()
    {
        Kind = SyntaxKind.Decorator;
    }

    public /*LeftHandSideExpression*/IExpression Expression { get; set; }
}

public class TypeParameterDeclaration : Declaration
{
    public TypeParameterDeclaration()
    {
        Kind = SyntaxKind.TypeParameter;
    }

    public ITypeNode Constraint { get; set; }
    public ITypeNode Default { get; set; }
    public IExpression Expression { get; set; }
}

public interface ISignatureDeclaration : IDeclaration
{
    NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    NodeArray<ParameterDeclaration> Parameters { get; set; }
    ITypeNode Type { get; set; }
}

public class SignatureDeclaration : Declaration, ISignatureDeclaration
{
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public NodeArray<ParameterDeclaration> Parameters { get; set; }
    public ITypeNode Type { get; set; }
}

public class CallSignatureDeclaration : Declaration, ISignatureDeclaration, ITypeElement
{
    public CallSignatureDeclaration()
    {
        Kind = SyntaxKind.CallSignature;
    }

    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public NodeArray<ParameterDeclaration> Parameters { get; set; }
    public ITypeNode Type { get; set; }
    public object TypeElementBrand { get; set; }
    public QuestionToken QuestionToken { get; set; }
}

public class ConstructSignatureDeclaration : Declaration, ISignatureDeclaration, ITypeElement
{
    public ConstructSignatureDeclaration()
    {
        Kind = SyntaxKind.ConstructSignature;
    }

    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public NodeArray<ParameterDeclaration> Parameters { get; set; }
    public ITypeNode Type { get; set; }
    public object TypeElementBrand { get; set; }
    public QuestionToken QuestionToken { get; set; }
}

public class VariableDeclaration : Declaration, IVariableLikeDeclaration
{
    public VariableDeclaration()
    {
        Kind = SyntaxKind.VariableDeclaration;
    }

    public ITypeNode Type { get; set; }
    public IExpression Initializer { get; set; }
    public IPropertyName PropertyName { get; set; }
    public DotDotDotToken DotDotDotToken { get; set; }
    public QuestionToken QuestionToken { get; set; }
}

public interface IVariableDeclarationListOrExpression : INode
{
}

public interface IVariableDeclarationList : INode, IVariableDeclarationListOrExpression
{
    NodeArray<VariableDeclaration> Declarations { get; set; }
}

public class VariableDeclarationList : Node, IVariableDeclarationList
{
    public VariableDeclarationList()
    {
        Kind = SyntaxKind.VariableDeclarationList;
    }

    public NodeArray<VariableDeclaration> Declarations { get; set; }
}

public class ParameterDeclaration : Declaration, IVariableLikeDeclaration
{
    public ParameterDeclaration()
    {
        Kind = SyntaxKind.Parameter;
    }

    public DotDotDotToken DotDotDotToken { get; set; }
    public QuestionToken QuestionToken { get; set; }
    public ITypeNode Type { get; set; }
    public IExpression Initializer { get; set; }
    public IPropertyName PropertyName { get; set; }
}

public class BindingElement : Declaration, IArrayBindingElement, IVariableLikeDeclaration
{
    public BindingElement()
    {
        Kind = SyntaxKind.BindingElement;
    }

    public IPropertyName PropertyName { get; set; }
    public DotDotDotToken DotDotDotToken { get; set; }
    public IExpression Initializer { get; set; }
    public QuestionToken QuestionToken { get; set; }
    public ITypeNode Type { get; set; }
}

public class PropertySignature : TypeElement, IVariableLikeDeclaration
{
    public PropertySignature()
    {
        Kind = SyntaxKind.PropertySignature;
    }

    public ITypeNode Type { get; set; }
    public IExpression Initializer { get; set; }
    public IPropertyName PropertyName { get; set; }
    public DotDotDotToken DotDotDotToken { get; set; }
}

public class PropertyDeclaration : ClassElement, IVariableLikeDeclaration
{
    public PropertyDeclaration()
    {
        Kind = SyntaxKind.PropertyDeclaration;
    }

    public QuestionToken QuestionToken { get; set; }
    public ITypeNode Type { get; set; }
    public IExpression Initializer { get; set; }
    public IPropertyName PropertyName { get; set; }
    public DotDotDotToken DotDotDotToken { get; set; }
}

public interface IObjectLiteralElement : IDeclaration
{
    object ObjectLiteralBrandBrand { get; set; }
}

public class ObjectLiteralElement : Declaration, IObjectLiteralElement
{
    public object ObjectLiteralBrandBrand { get; set; }
}

public class PropertyAssignment : ObjectLiteralElement, IObjectLiteralElementLike, IVariableLikeDeclaration
{
    public PropertyAssignment()
    {
        Kind = SyntaxKind.PropertyAssignment;
    }

    public QuestionToken QuestionToken { get; set; }
    public IExpression Initializer { get; set; }
    public IPropertyName PropertyName { get; set; }
    public DotDotDotToken DotDotDotToken { get; set; }
    public ITypeNode Type { get; set; }
}

public class ShorthandPropertyAssignment : ObjectLiteralElement, IObjectLiteralElementLike
{
    public ShorthandPropertyAssignment()
    {
        Kind = SyntaxKind.ShorthandPropertyAssignment;
    }

    public QuestionToken QuestionToken { get; set; }
    public Token EqualsToken { get; set; } // Token<SyntaxKind.EqualsToken>
    public IExpression ObjectAssignmentInitializer { get; set; }
}

public class SpreadAssignment : ObjectLiteralElement, IObjectLiteralElementLike
{
    public SpreadAssignment()
    {
        Kind = SyntaxKind.SpreadAssignment;
    }

    public IExpression Expression { get; set; }
}

public interface IVariableLikeDeclaration : IDeclaration
{
    IPropertyName PropertyName { get; set; }
    DotDotDotToken DotDotDotToken { get; set; }
    QuestionToken QuestionToken { get; set; }
    ITypeNode Type { get; set; }
    IExpression Initializer { get; set; }
}

public class PropertyLikeDeclaration : Declaration
{
}

public class ObjectBindingPattern : Node, IBindingPattern
{
    public ObjectBindingPattern()
    {
        Kind = SyntaxKind.ObjectBindingPattern;
    }

    public NodeArray<IArrayBindingElement> Elements { get; set; }
}

public class ArrayBindingPattern : Node, IBindingPattern
{
    public ArrayBindingPattern()
    {
        Kind = SyntaxKind.ArrayBindingPattern;
    }

    public NodeArray<IArrayBindingElement> Elements { get; set; }
}

public interface IFunctionLikeDeclaration : ISignatureDeclaration
{
    object FunctionLikeDeclarationBrand { get; set; }
    AsteriskToken AsteriskToken { get; set; }
    QuestionToken QuestionToken { get; set; }
    IBlockOrExpression Body { get; set; } // Block | Expression
}

public class FunctionLikeDeclaration : SignatureDeclaration, IFunctionLikeDeclaration
{
    public object FunctionLikeDeclarationBrand { get; set; }
    public AsteriskToken AsteriskToken { get; set; }
    public QuestionToken QuestionToken { get; set; }
    public IBlockOrExpression Body { get; set; } // Block | Expression
}

public class FunctionDeclaration : Node, IFunctionLikeDeclaration, IDeclarationStatement
{
    public FunctionDeclaration()
    {
        Kind = SyntaxKind.FunctionDeclaration;
    }

    public object StatementBrand { get; set; }
    public object FunctionLikeDeclarationBrand { get; set; }
    public AsteriskToken AsteriskToken { get; set; }
    public QuestionToken QuestionToken { get; set; }
    public IBlockOrExpression Body { get; set; } //  Block | Expression
    public INode Name { get; set; }
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public NodeArray<ParameterDeclaration> Parameters { get; set; }
    public ITypeNode Type { get; set; }
    public object DeclarationBrand { get; set; }
}

public class MethodSignature : Declaration, ISignatureDeclaration, ITypeElement, IFunctionLikeDeclaration
{
    public MethodSignature()
    {
        Kind = SyntaxKind.MethodSignature;
    }

    public object FunctionLikeDeclarationBrand { get; set; }
    public AsteriskToken AsteriskToken { get; set; }
    public IBlockOrExpression Body { get; set; } //  Block | Expression
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public NodeArray<ParameterDeclaration> Parameters { get; set; }
    public ITypeNode Type { get; set; }
    public object TypeElementBrand { get; set; }
    public QuestionToken QuestionToken { get; set; }
}

public class MethodDeclaration : Declaration, IFunctionLikeDeclaration, IClassElement, IObjectLiteralElement,
    IObjectLiteralElementLike
{
    public MethodDeclaration()
    {
        Kind = SyntaxKind.MethodDeclaration;
    }

    public object ClassElementBrand { get; set; }
    public object FunctionLikeDeclarationBrand { get; set; }
    public AsteriskToken AsteriskToken { get; set; }
    public QuestionToken QuestionToken { get; set; }
    public IBlockOrExpression Body { get; set; } //  Block | Expression
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public NodeArray<ParameterDeclaration> Parameters { get; set; }
    public ITypeNode Type { get; set; }
    public object ObjectLiteralBrandBrand { get; set; }
}

public class ConstructorDeclaration : Declaration, IFunctionLikeDeclaration, IClassElement
{
    public ConstructorDeclaration()
    {
        Kind = SyntaxKind.Constructor;
    }

    public object ClassElementBrand { get; set; }
    public object FunctionLikeDeclarationBrand { get; set; }
    public AsteriskToken AsteriskToken { get; set; }
    public QuestionToken QuestionToken { get; set; }
    public IBlockOrExpression Body { get; set; } //  Block | Expression
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public NodeArray<ParameterDeclaration> Parameters { get; set; }
    public ITypeNode Type { get; set; }
}

public class SemicolonClassElement : ClassElement
{
    public SemicolonClassElement()
    {
        Kind = SyntaxKind.SemicolonClassElement;
    }
}

public class GetAccessorDeclaration : Declaration, IFunctionLikeDeclaration, IClassElement, IObjectLiteralElement,
    IAccessorDeclaration
{
    public GetAccessorDeclaration()
    {
        Kind = SyntaxKind.GetAccessor;
    }

    public object ClassElementBrand { get; set; }
    public object FunctionLikeDeclarationBrand { get; set; }
    public AsteriskToken AsteriskToken { get; set; }
    public QuestionToken QuestionToken { get; set; }
    public IBlockOrExpression Body { get; set; } //  Block | Expression
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public NodeArray<ParameterDeclaration> Parameters { get; set; }
    public ITypeNode Type { get; set; }
    public object ObjectLiteralBrandBrand { get; set; }
}

public class SetAccessorDeclaration : Declaration, IFunctionLikeDeclaration, IClassElement, IObjectLiteralElement,
    IAccessorDeclaration
{
    public SetAccessorDeclaration()
    {
        Kind = SyntaxKind.SetAccessor;
    }

    public object ClassElementBrand { get; set; }
    public object FunctionLikeDeclarationBrand { get; set; }
    public AsteriskToken AsteriskToken { get; set; }
    public QuestionToken QuestionToken { get; set; }
    public IBlockOrExpression Body { get; set; } //  Block | Expression
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public NodeArray<ParameterDeclaration> Parameters { get; set; }
    public ITypeNode Type { get; set; }
    public object ObjectLiteralBrandBrand { get; set; }
}

public class IndexSignatureDeclaration : Declaration, ISignatureDeclaration, IClassElement, ITypeElement
{
    public IndexSignatureDeclaration()
    {
        Kind = SyntaxKind.IndexSignature;
    }

    public object ClassElementBrand { get; set; }
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public NodeArray<ParameterDeclaration> Parameters { get; set; }
    public ITypeNode Type { get; set; }
    public object TypeElementBrand { get; set; }
    public QuestionToken QuestionToken { get; set; }
}

public interface ITypeNode : INode
{
    object TypeNodeBrand { get; set; }
}

public class TypeNode : Node, ITypeNode
{
    public object TypeNodeBrand { get; set; }
}

public interface IKeywordTypeNode : ITypeNode
{
}

public class KeywordTypeNode : TypeNode, IKeywordTypeNode
{
}

public class ThisTypeNode : TypeNode
{
    public ThisTypeNode()
    {
        Kind = SyntaxKind.ThisType;
    }
}

public class FunctionTypeNode : Node, ITypeNode, IFunctionOrConstructorTypeNode
{
    public FunctionTypeNode()
    {
        Kind = SyntaxKind.FunctionType;
    }

    public INode Name { get; set; }
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public NodeArray<ParameterDeclaration> Parameters { get; set; }
    public ITypeNode Type { get; set; }
    public object DeclarationBrand { get; set; }
    public object TypeNodeBrand { get; set; }
}

public class ConstructorTypeNode : Node, ITypeNode, IFunctionOrConstructorTypeNode
{
    public ConstructorTypeNode()
    {
        Kind = SyntaxKind.ConstructorType;
    }

    public INode Name { get; set; }
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public NodeArray<ParameterDeclaration> Parameters { get; set; }
    public ITypeNode Type { get; set; }
    public object DeclarationBrand { get; set; }
    public object TypeNodeBrand { get; set; }
}

public class TypeReferenceNode : TypeNode
{
    public TypeReferenceNode()
    {
        Kind = SyntaxKind.TypeReference;
    }

    public IEntityName TypeName { get; set; }
    public NodeArray<ITypeNode> TypeArguments { get; set; }
}

public class TypePredicateNode : TypeNode
{
    public TypePredicateNode()
    {
        Kind = SyntaxKind.TypePredicate;
    }

    public Node ParameterName { get; set; } // Identifier | ThisTypeNode
    public ITypeNode Type { get; set; }
}

public class TypeQueryNode : TypeNode
{
    public TypeQueryNode()
    {
        Kind = SyntaxKind.TypeQuery;
    }

    public IEntityName ExprName { get; set; }
}

public class TypeLiteralNode : Node, ITypeNode, IDeclaration
{
    public TypeLiteralNode()
    {
        Kind = SyntaxKind.TypeLiteral;
    }

    public NodeArray<ITypeElement> Members { get; set; }
    public object DeclarationBrand { get; set; }
    public INode Name { get; set; }
    public object TypeNodeBrand { get; set; }
}

public class ArrayTypeNode : TypeNode
{
    public ArrayTypeNode()
    {
        Kind = SyntaxKind.ArrayType;
    }

    public ITypeNode ElementType { get; set; }
}

public class TupleTypeNode : TypeNode
{
    public TupleTypeNode()
    {
        Kind = SyntaxKind.TupleType;
    }

    public NodeArray<ITypeNode> ElementTypes { get; set; }
}

public class UnionTypeNode : TypeNode, IUnionOrIntersectionTypeNode
{
    public UnionTypeNode()
    {
        Kind = SyntaxKind.UnionType;
    }

    public NodeArray<ITypeNode> Types { get; set; }
}

public class IntersectionTypeNode : TypeNode, IUnionOrIntersectionTypeNode
{
    public IntersectionTypeNode()
    {
        Kind = SyntaxKind.IntersectionType;
    }

    public NodeArray<ITypeNode> Types { get; set; }
}

public class ParenthesizedTypeNode : TypeNode
{
    public ParenthesizedTypeNode()
    {
        Kind = SyntaxKind.ParenthesizedType;
    }

    public ITypeNode Type { get; set; }
}

public class TypeOperatorNode : ParenthesizedTypeNode
{
    public TypeOperatorNode()
    {
        Kind = SyntaxKind.TypeOperator;
    }

    public SyntaxKind Operator { get; set; } = SyntaxKind.KeyOfKeyword;
}

public class IndexedAccessTypeNode : TypeNode
{
    public IndexedAccessTypeNode()
    {
        Kind = SyntaxKind.IndexedAccessType;
    }

    public ITypeNode ObjectType { get; set; }
    public ITypeNode IndexType { get; set; }
}

public class MappedTypeNode : Node, ITypeNode, IDeclaration
{
    public MappedTypeNode()
    {
        Kind = SyntaxKind.MappedType;
    }

    public ReadonlyToken ReadonlyToken { get; set; }
    public TypeParameterDeclaration TypeParameter { get; set; }
    public QuestionToken QuestionToken { get; set; }
    public ITypeNode Type { get; set; }
    public object DeclarationBrand { get; set; }
    public INode Name { get; set; }
    public object TypeNodeBrand { get; set; }
}

public class LiteralTypeNode : TypeNode
{
    public LiteralTypeNode()
    {
        Kind = SyntaxKind.LiteralType;
    }

    public IExpression Literal { get; set; }
}

public class StringLiteral : LiteralExpression, IPropertyName
{
    public StringLiteral()
    {
        Kind = SyntaxKind.StringLiteral;
    }

    public Node TextSourceNode { get; set; } // Identifier | StringLiteral | NumericLiteral
}

public interface IExpression : IBlockOrExpression, IVariableDeclarationListOrExpression
{
    object ExpressionBrand { get; set; }
}

public class Expression : Node, IExpression
{
    public object ExpressionBrand { get; set; }
}

public class OmittedExpression : Expression, IArrayBindingElement
{
    public OmittedExpression()
    {
        Kind = SyntaxKind.OmittedExpression;
    }
}

public class PartiallyEmittedExpression : LeftHandSideExpression
{
    public PartiallyEmittedExpression()
    {
        Kind = SyntaxKind.PartiallyEmittedExpression;
    }

    public IExpression Expression { get; set; }
}

public interface IUnaryExpression : IExpression
{
    object UnaryExpressionBrand { get; set; }
}

public class UnaryExpression : Expression, IUnaryExpression
{
    public object UnaryExpressionBrand { get; set; }
}

public interface IIncrementExpression : IUnaryExpression
{
    object IncrementExpressionBrand { get; set; }
}

public class IncrementExpression : UnaryExpression, IIncrementExpression
{
    public object IncrementExpressionBrand { get; set; }
}

public class PrefixUnaryExpression : IncrementExpression
{
    public PrefixUnaryExpression()
    {
        Kind = SyntaxKind.PrefixUnaryExpression;
    }

    public /*PrefixUnaryOperator*/SyntaxKind Operator { get; set; }
    public /*UnaryExpression*/IExpression Operand { get; set; }
}

public class PostfixUnaryExpression : IncrementExpression
{
    public PostfixUnaryExpression()
    {
        Kind = SyntaxKind.PostfixUnaryExpression;
    }

    public /*LeftHandSideExpression*/IExpression Operand { get; set; }
    public /*PostfixUnaryOperator*/SyntaxKind Operator { get; set; }
}

public interface ILeftHandSideExpression : IIncrementExpression
{
    object LeftHandSideExpressionBrand { get; set; }
}

public class LeftHandSideExpression : IncrementExpression, ILeftHandSideExpression
{
    public object LeftHandSideExpressionBrand { get; set; }
}

public interface IMemberExpression : ILeftHandSideExpression
{
    object MemberExpressionBrand { get; set; }
}

public class MemberExpression : LeftHandSideExpression, IMemberExpression
{
    public object MemberExpressionBrand { get; set; }
}

public interface IPrimaryExpression : IMemberExpression
{
    object PrimaryExpressionBrand { get; set; }
}

public class PrimaryExpression : MemberExpression, IPrimaryExpression, IJsxTagNameExpression
{
    public object PrimaryExpressionBrand { get; set; }
}

public class NullLiteral : Node, IPrimaryExpression, ITypeNode
{
    public NullLiteral()
    {
        Kind = SyntaxKind.NullKeyword;
    }

    public object PrimaryExpressionBrand { get; set; }
    public object MemberExpressionBrand { get; set; }
    public object LeftHandSideExpressionBrand { get; set; }
    public object IncrementExpressionBrand { get; set; }
    public object UnaryExpressionBrand { get; set; }
    public object ExpressionBrand { get; set; }
    public object TypeNodeBrand { get; set; }
}

public class BooleanLiteral : Node, IPrimaryExpression, ITypeNode
{
    public BooleanLiteral()
    {
        Kind = SyntaxKind.BooleanKeyword;
    }

    public object PrimaryExpressionBrand { get; set; }
    public object MemberExpressionBrand { get; set; }
    public object LeftHandSideExpressionBrand { get; set; }
    public object IncrementExpressionBrand { get; set; }
    public object UnaryExpressionBrand { get; set; }
    public object ExpressionBrand { get; set; }
    public object TypeNodeBrand { get; set; }
}

public class ThisExpression : Node, IPrimaryExpression, IKeywordTypeNode
{
    public ThisExpression()
    {
        Kind = SyntaxKind.ThisKeyword;
    }

    public object TypeNodeBrand { get; set; }
    public object PrimaryExpressionBrand { get; set; }
    public object MemberExpressionBrand { get; set; }
    public object LeftHandSideExpressionBrand { get; set; }
    public object IncrementExpressionBrand { get; set; }
    public object UnaryExpressionBrand { get; set; }
    public object ExpressionBrand { get; set; }
}

public class SuperExpression : PrimaryExpression
{
    public SuperExpression()
    {
        Kind = SyntaxKind.SuperKeyword;
    }
}

public class DeleteExpression : UnaryExpression
{
    public DeleteExpression()
    {
        Kind = SyntaxKind.DeleteExpression;
    }

    public /*UnaryExpression*/IExpression Expression { get; set; }
}

public class TypeOfExpression : UnaryExpression
{
    public TypeOfExpression()
    {
        Kind = SyntaxKind.TypeOfExpression;
    }

    public /*UnaryExpression*/IExpression Expression { get; set; }
}

public class VoidExpression : UnaryExpression
{
    public VoidExpression()
    {
        Kind = SyntaxKind.VoidExpression;
    }

    public /*UnaryExpression*/IExpression Expression { get; set; }
}

public class AwaitExpression : UnaryExpression
{
    public AwaitExpression()
    {
        Kind = SyntaxKind.AwaitExpression;
    }

    public /*UnaryExpression*/IExpression Expression { get; set; }
}

public class YieldExpression : Expression
{
    public YieldExpression()
    {
        Kind = SyntaxKind.YieldExpression;
    }

    public AsteriskToken AsteriskToken { get; set; }
    public IExpression Expression { get; set; }
}

public class BinaryExpression : Node, IExpression, IDeclaration
{
    public BinaryExpression()
    {
        Kind = SyntaxKind.BinaryExpression;
    }

    public IExpression Left { get; set; }
    public /*BinaryOperator*/Token OperatorToken { get; set; }
    public IExpression Right { get; set; }
    public object DeclarationBrand { get; set; }
    public INode Name { get; set; }
    public object ExpressionBrand { get; set; }
}

public class AssignmentExpression : BinaryExpression
{
}

public class ObjectDestructuringAssignment : AssignmentExpression
{
}

public class ArrayDestructuringAssignment : AssignmentExpression
{
}

public class ConditionalExpression : Expression
{
    public ConditionalExpression()
    {
        Kind = SyntaxKind.ConditionalExpression;
    }

    public IExpression Condition { get; set; }
    public QuestionToken QuestionToken { get; set; }
    public IExpression WhenTrue { get; set; }
    public ColonToken ColonToken { get; set; }
    public IExpression WhenFalse { get; set; }
}

public class FunctionExpression : Node, IPrimaryExpression, IFunctionLikeDeclaration
{
    public FunctionExpression()
    {
        Kind = SyntaxKind.FunctionExpression;
    }

    public object FunctionLikeDeclarationBrand { get; set; }
    public AsteriskToken AsteriskToken { get; set; }
    public QuestionToken QuestionToken { get; set; }
    public IBlockOrExpression Body { get; set; } //  Block | Expression
    public INode Name { get; set; }
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public NodeArray<ParameterDeclaration> Parameters { get; set; }
    public ITypeNode Type { get; set; }
    public object DeclarationBrand { get; set; }
    public object PrimaryExpressionBrand { get; set; }
    public object MemberExpressionBrand { get; set; }
    public object LeftHandSideExpressionBrand { get; set; }
    public object IncrementExpressionBrand { get; set; }
    public object UnaryExpressionBrand { get; set; }
    public object ExpressionBrand { get; set; }
}

public interface IBlockOrExpression : INode
{
}

public class ArrowFunction : Node, IExpression, IFunctionLikeDeclaration
{
    public ArrowFunction()
    {
        Kind = SyntaxKind.ArrowFunction;
    }

    public EqualsGreaterThanToken EqualsGreaterThanToken { get; set; }
    public object ExpressionBrand { get; set; }
    public object FunctionLikeDeclarationBrand { get; set; }
    public AsteriskToken AsteriskToken { get; set; }
    public QuestionToken QuestionToken { get; set; }
    public IBlockOrExpression Body { get; set; } //  Block | Expression
    public INode Name { get; set; }
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public NodeArray<ParameterDeclaration> Parameters { get; set; }
    public ITypeNode Type { get; set; }
    public object DeclarationBrand { get; set; }
}

public interface ILiteralLikeNode : INode
{
    string Text { get; set; }
    bool IsUnterminated { get; set; }
    bool HasExtendedUnicodeEscape { get; set; }
    bool IsOctalLiteral { get; set; }
}

public class LiteralLikeNode : Node, ILiteralLikeNode
{
    public string Text { get; set; }
    public bool IsUnterminated { get; set; }
    public bool HasExtendedUnicodeEscape { get; set; }
    public bool IsOctalLiteral { get; set; }
}

public interface ILiteralExpression : ILiteralLikeNode, IPrimaryExpression
{
    object LiteralExpressionBrand { get; set; }
}

public class LiteralExpression : Node, ILiteralExpression, IPrimaryExpression
{
    public object LiteralExpressionBrand { get; set; }
    public string Text { get; set; }
    public bool IsUnterminated { get; set; }
    public bool HasExtendedUnicodeEscape { get; set; }
    public bool IsOctalLiteral { get; set; }
    public object PrimaryExpressionBrand { get; set; }
    public object MemberExpressionBrand { get; set; }
    public object LeftHandSideExpressionBrand { get; set; }
    public object IncrementExpressionBrand { get; set; }
    public object UnaryExpressionBrand { get; set; }

    public object ExpressionBrand { get; set; }
    //}

    //    };
    //        text = literalLikeNode.text,
    //    {
    //    return new LiteralExpression
    //    throw new NotImplementedException();
    //{

    //internal static LiteralExpression Make(LiteralLikeNode literalLikeNode)
}

public class RegularExpressionLiteral : LiteralExpression
{
    public RegularExpressionLiteral()
    {
        Kind = SyntaxKind.RegularExpressionLiteral;
    }
}

public class NoSubstitutionTemplateLiteral : LiteralExpression
{
    public NoSubstitutionTemplateLiteral()
    {
        Kind = SyntaxKind.NoSubstitutionTemplateLiteral;
    }
}

public class NumericLiteral : LiteralExpression, IPropertyName
{
    public NumericLiteral()
    {
        Kind = SyntaxKind.NumericLiteral;
    }
}

public class TemplateHead : LiteralLikeNode
{
    public TemplateHead()
    {
        Kind = SyntaxKind.TemplateHead;
    }
}

public class TemplateMiddle : LiteralLikeNode
{
    public TemplateMiddle()
    {
        Kind = SyntaxKind.TemplateMiddle;
    }
}

public class TemplateTail : LiteralLikeNode
{
    public TemplateTail()
    {
        Kind = SyntaxKind.TemplateTail;
    }
}

public class TemplateExpression : PrimaryExpression
{
    public TemplateExpression()
    {
        Kind = SyntaxKind.TemplateExpression;
    }

    public TemplateHead Head { get; set; }
    public NodeArray<TemplateSpan> TemplateSpans { get; set; }
}

public class TemplateSpan : Node
{
    public TemplateSpan()
    {
        Kind = SyntaxKind.TemplateSpan;
    }

    public IExpression Expression { get; set; }
    public ILiteralLikeNode Literal { get; set; } // TemplateMiddle | TemplateTail
}

public class ParenthesizedExpression : PrimaryExpression
{
    public ParenthesizedExpression()
    {
        Kind = SyntaxKind.ParenthesizedExpression;
    }

    public IExpression Expression { get; set; }
}

public class ArrayLiteralExpression : PrimaryExpression
{
    public ArrayLiteralExpression()
    {
        Kind = SyntaxKind.ArrayLiteralExpression;
    }

    public NodeArray<IExpression> Elements { get; set; }
    public bool MultiLine { get; set; }
}

public class SpreadElement : Expression
{
    public SpreadElement()
    {
        Kind = SyntaxKind.SpreadElement;
    }

    public IExpression Expression { get; set; }
}

public class ObjectLiteralExpressionBase<T> : Node, IPrimaryExpression, IDeclaration
{
    public NodeArray<T> Properties { get; set; }
    public object DeclarationBrand { get; set; }
    public INode Name { get; set; }
    public object PrimaryExpressionBrand { get; set; }
    public object MemberExpressionBrand { get; set; }
    public object LeftHandSideExpressionBrand { get; set; }
    public object IncrementExpressionBrand { get; set; }
    public object UnaryExpressionBrand { get; set; }
    public object ExpressionBrand { get; set; }
}

public class ObjectLiteralExpression : ObjectLiteralExpressionBase<IObjectLiteralElementLike>
{
    public ObjectLiteralExpression()
    {
        Kind = SyntaxKind.ObjectLiteralExpression;
    }

    public bool MultiLine { get; set; }
}

public class PropertyAccessExpression : Node, IMemberExpression, IDeclaration, IJsxTagNameExpression
{
    public PropertyAccessExpression()
    {
        Kind = SyntaxKind.PropertyAccessExpression;
    }

    public IExpression Expression { get; set; } //LeftHandSideExpression
    public object DeclarationBrand { get; set; }
    public INode Name { get; set; }
    public object MemberExpressionBrand { get; set; }
    public object LeftHandSideExpressionBrand { get; set; }
    public object IncrementExpressionBrand { get; set; }
    public object UnaryExpressionBrand { get; set; }
    public object ExpressionBrand { get; set; }
}

public class SuperPropertyAccessExpression : PropertyAccessExpression
{
}

public class PropertyAccessEntityNameExpression : PropertyAccessExpression
{
    public object PropertyAccessExpressionLikeQualifiedNameBrand { get; set; }
}

public class ElementAccessExpression : MemberExpression
{
    public ElementAccessExpression()
    {
        Kind = SyntaxKind.ElementAccessExpression;
    }

    public IExpression Expression { get; set; } //LeftHandSideExpression
    public IExpression ArgumentExpression { get; set; }
}

public class SuperElementAccessExpression : ElementAccessExpression
{
}

public class CallExpression : Node, IMemberExpression, IDeclaration
{
    public CallExpression()
    {
        Kind = SyntaxKind.CallExpression;
    }

    public /*LeftHandSideExpression*/IExpression Expression { get; set; }
    public NodeArray<ITypeNode> TypeArguments { get; set; }
    public NodeArray<IExpression> Arguments { get; set; }
    public object DeclarationBrand { get; set; }
    public INode Name { get; set; }
    public object LeftHandSideExpressionBrand { get; set; }
    public object IncrementExpressionBrand { get; set; }
    public object UnaryExpressionBrand { get; set; }
    public object ExpressionBrand { get; set; }
    public object MemberExpressionBrand { get; set; }
}

public class SuperCall : CallExpression
{
}

public class ExpressionWithTypeArguments : TypeNode
{
    public ExpressionWithTypeArguments()
    {
        Kind = SyntaxKind.ExpressionWithTypeArguments;
    }

    public /*LeftHandSideExpression*/IExpression Expression { get; set; }
    public NodeArray<ITypeNode> TypeArguments { get; set; }
}

public class NewExpression : CallExpression, IPrimaryExpression, IDeclaration
{
    public NewExpression()
    {
        Kind = SyntaxKind.NewExpression;
    }

    public object PrimaryExpressionBrand { get; set; }
}

public class TaggedTemplateExpression : MemberExpression
{
    public TaggedTemplateExpression()
    {
        Kind = SyntaxKind.TaggedTemplateExpression;
    }

    public IExpression Tag { get; set; } //LeftHandSideExpression
    public Node Template { get; set; } //TemplateLiteral
}

public class AsExpression : Expression
{
    public AsExpression()
    {
        Kind = SyntaxKind.AsExpression;
    }

    public IExpression Expression { get; set; }
    public ITypeNode Type { get; set; }
}

public class TypeAssertion : UnaryExpression
{
    public TypeAssertion()
    {
        Kind = SyntaxKind.TypeAssertionExpression;
    }

    public ITypeNode Type { get; set; }
    public /*UnaryExpression*/IExpression Expression { get; set; }
}

public class NonNullExpression : /*LeftHandSideExpression*/MemberExpression
{
    public NonNullExpression()
    {
        Kind = SyntaxKind.NonNullExpression;
    }

    public IExpression Expression { get; set; }
}

public class MetaProperty : PrimaryExpression
{
    public MetaProperty()
    {
        Kind = SyntaxKind.MetaProperty;
    }

    public SyntaxKind KeywordToken { get; set; }
    public Identifier Name { get; set; }
}

public class JsxElement : PrimaryExpression, IJsxChild
{
    public JsxElement()
    {
        Kind = SyntaxKind.JsxElement;
    }

    public /*JsxOpeningElement*/IExpression OpeningElement { get; set; }
    public NodeArray<IJsxChild> JsxChildren { get; set; }
    public JsxClosingElement ClosingElement { get; set; }
}

public class JsxAttributes : ObjectLiteralExpressionBase<ObjectLiteralElement> // JsxAttributeLike>
{
    public JsxAttributes()
    {
        Kind = SyntaxKind.JsxAttributes;
    }
}

public class JsxOpeningElement : JsxSelfClosingElement
{
    public JsxOpeningElement()
    {
        Kind = SyntaxKind.JsxOpeningElement;
    }
}

public class JsxSelfClosingElement : PrimaryExpression, IJsxChild
{
    public JsxSelfClosingElement()
    {
        Kind = SyntaxKind.JsxSelfClosingElement;
    }

    public IJsxTagNameExpression TagName { get; set; }
    public JsxAttributes Attributes { get; set; }
}

public class JsxAttribute : ObjectLiteralElement
{
    public JsxAttribute()
    {
        Kind = SyntaxKind.JsxAttribute;
    }

    public Node Initializer { get; set; } // StringLiteral | JsxExpression
}

public class JsxSpreadAttribute : ObjectLiteralElement
{
    public JsxSpreadAttribute()
    {
        Kind = SyntaxKind.JsxSpreadAttribute;
    }

    public IExpression Expression { get; set; }
}

public class JsxClosingElement : Node
{
    public JsxClosingElement()
    {
        Kind = SyntaxKind.JsxClosingElement;
    }

    public IJsxTagNameExpression TagName { get; set; }
}

public class JsxExpression : Expression, IJsxChild
{
    public JsxExpression()
    {
        Kind = SyntaxKind.JsxExpression;
    }

    public Token DotDotDotToken { get; set; } // Token<SyntaxKind.DotDotDotToken>
    public IExpression Expression { get; set; }
}

public class JsxText : Node, IJsxChild
{
    public JsxText()
    {
        Kind = SyntaxKind.JsxText;
    }
}

public interface IStatement : INode
{
    object StatementBrand { get; set; }
}

public class Statement : Node, IStatement
{
    public object StatementBrand { get; set; }
}

public class NotEmittedStatement : Statement
{
    public NotEmittedStatement()
    {
        Kind = SyntaxKind.NotEmittedStatement;
    }
}

public class EndOfDeclarationMarker : Statement
{
    public EndOfDeclarationMarker()
    {
        Kind = SyntaxKind.EndOfDeclarationMarker;
    }
}

public class MergeDeclarationMarker : Statement
{
    public MergeDeclarationMarker()
    {
        Kind = SyntaxKind.MergeDeclarationMarker;
    }
}

public class EmptyStatement : Statement
{
    public EmptyStatement()
    {
        Kind = SyntaxKind.EmptyStatement;
    }
}

public class DebuggerStatement : Statement
{
    public DebuggerStatement()
    {
        Kind = SyntaxKind.DebuggerStatement;
    }
}

public class MissingDeclaration : Node, IDeclarationStatement, IClassElement, IObjectLiteralElement, ITypeElement
{
    public MissingDeclaration()
    {
        Kind = SyntaxKind.MissingDeclaration;
    }

    public object ClassElementBrand { get; set; }
    public INode Name { get; set; }
    public object DeclarationBrand { get; set; }
    public object StatementBrand { get; set; }
    public object ObjectLiteralBrandBrand { get; set; }
    public object TypeElementBrand { get; set; }
    public QuestionToken QuestionToken { get; set; }
}

public class Block : Statement, IBlockOrExpression
{
    public Block()
    {
        Kind = SyntaxKind.Block;
    }

    public NodeArray<IStatement> Statements { get; set; }
    public bool MultiLine { get; set; }
}

public class VariableStatement : Statement
{
    public VariableStatement()
    {
        Kind = SyntaxKind.VariableStatement;
    }

    public IVariableDeclarationList DeclarationList { get; set; }
}

public class ExpressionStatement : Statement
{
    public ExpressionStatement()
    {
        Kind = SyntaxKind.ExpressionStatement;
    }

    public IExpression Expression { get; set; }
}

public class PrologueDirective : ExpressionStatement
{
}

public class IfStatement : Statement
{
    public IfStatement()
    {
        Kind = SyntaxKind.IfStatement;
    }

    public IExpression Expression { get; set; }
    public IStatement ThenStatement { get; set; }
    public IStatement ElseStatement { get; set; }
}

public class IterationStatement : Statement
{
    public IStatement Statement { get; set; }
}

public class DoStatement : IterationStatement
{
    public DoStatement()
    {
        Kind = SyntaxKind.DoStatement;
    }

    public IExpression Expression { get; set; }
}

public class WhileStatement : IterationStatement
{
    public WhileStatement()
    {
        Kind = SyntaxKind.WhileStatement;
    }

    public IExpression Expression { get; set; }
}

public class ForStatement : IterationStatement
{
    public ForStatement()
    {
        Kind = SyntaxKind.ForStatement;
    }

    public /*ForInitializer*/IVariableDeclarationListOrExpression Initializer { get; set; }
    public IExpression Condition { get; set; }
    public IExpression Incrementor { get; set; }
}

public class ForInStatement : IterationStatement
{
    public ForInStatement()
    {
        Kind = SyntaxKind.ForInStatement;
    }

    public /*ForInitializer*/IVariableDeclarationListOrExpression Initializer { get; set; }
    public IExpression Expression { get; set; }
}

public class ForOfStatement : IterationStatement
{
    public ForOfStatement()
    {
        Kind = SyntaxKind.ForOfStatement;
    }

    public AwaitKeywordToken AwaitModifier { get; set; }
    public /*ForInitializer*/IVariableDeclarationListOrExpression Initializer { get; set; }
    public IExpression Expression { get; set; }
}

public class BreakStatement : Statement, IBreakOrContinueStatement
{
    public BreakStatement()
    {
        Kind = SyntaxKind.BreakStatement;
    }

    public Identifier Label { get; set; }
}

public class ContinueStatement : Statement, IBreakOrContinueStatement
{
    public ContinueStatement()
    {
        Kind = SyntaxKind.ContinueStatement;
    }

    public Identifier Label { get; set; }
}

public class ReturnStatement : Statement
{
    public ReturnStatement()
    {
        Kind = SyntaxKind.ReturnStatement;
    }

    public IExpression Expression { get; set; }
}

public class WithStatement : Statement
{
    public WithStatement()
    {
        Kind = SyntaxKind.WithStatement;
    }

    public IExpression Expression { get; set; }
    public IStatement Statement { get; set; }
}

public class SwitchStatement : Statement
{
    public SwitchStatement()
    {
        Kind = SyntaxKind.SwitchStatement;
    }

    public IExpression Expression { get; set; }
    public CaseBlock CaseBlock { get; set; }
    public bool PossiblyExhaustive { get; set; }
}

public class CaseBlock : Node
{
    public CaseBlock()
    {
        Kind = SyntaxKind.CaseBlock;
    }

    public NodeArray<ICaseOrDefaultClause> Clauses { get; set; }
}

public class CaseClause : Node, ICaseOrDefaultClause
{
    public CaseClause()
    {
        Kind = SyntaxKind.CaseClause;
    }

    public IExpression Expression { get; set; }
    public NodeArray<IStatement> Statements { get; set; }
}

public class DefaultClause : Node, ICaseOrDefaultClause
{
    public DefaultClause()
    {
        Kind = SyntaxKind.DefaultClause;
    }

    public NodeArray<IStatement> Statements { get; set; }
}

public class LabeledStatement : Statement
{
    public LabeledStatement()
    {
        Kind = SyntaxKind.LabeledStatement;
    }

    public Identifier Label { get; set; }
    public IStatement Statement { get; set; }
}

public class ThrowStatement : Statement
{
    public ThrowStatement()
    {
        Kind = SyntaxKind.ThrowStatement;
    }

    public IExpression Expression { get; set; }
}

public class TryStatement : Statement
{
    public TryStatement()
    {
        Kind = SyntaxKind.TryStatement;
    }

    public Block TryBlock { get; set; }
    public CatchClause CatchClause { get; set; }
    public Block FinallyBlock { get; set; }
}

public class CatchClause : Node
{
    public CatchClause()
    {
        Kind = SyntaxKind.CatchClause;
    }

    public VariableDeclaration VariableDeclaration { get; set; }
    public Block Block { get; set; }
}

public interface IClassLikeDeclaration : IDeclaration
{
    NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    NodeArray<HeritageClause> HeritageClauses { get; set; }
    NodeArray<IClassElement> Members { get; set; }
}

public class ClassLikeDeclaration : Declaration, IClassLikeDeclaration
{
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public NodeArray<HeritageClause> HeritageClauses { get; set; }
    public NodeArray<IClassElement> Members { get; set; }
}

public class ClassDeclaration : Node, IClassLikeDeclaration, IDeclarationStatement
{
    public ClassDeclaration()
    {
        Kind = SyntaxKind.ClassDeclaration;
    }

    public INode Name { get; set; }
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public NodeArray<HeritageClause> HeritageClauses { get; set; }
    public NodeArray<IClassElement> Members { get; set; }
    public object DeclarationBrand { get; set; }
    public object StatementBrand { get; set; }
}

public class ClassExpression : Node, IClassLikeDeclaration, IPrimaryExpression
{
    public ClassExpression()
    {
        Kind = SyntaxKind.ClassExpression;
    }

    public INode Name { get; set; }
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public NodeArray<HeritageClause> HeritageClauses { get; set; }
    public NodeArray<IClassElement> Members { get; set; }
    public object DeclarationBrand { get; set; }
    public object PrimaryExpressionBrand { get; set; }
    public object MemberExpressionBrand { get; set; }
    public object LeftHandSideExpressionBrand { get; set; }
    public object IncrementExpressionBrand { get; set; }
    public object UnaryExpressionBrand { get; set; }
    public object ExpressionBrand { get; set; }
}

public interface IClassElement : IDeclaration
{
    object ClassElementBrand { get; set; }
}

public class ClassElement : Declaration, IClassElement
{
    public object ClassElementBrand { get; set; }
}

public interface ITypeElement : IDeclaration
{
    object TypeElementBrand { get; set; }
    QuestionToken QuestionToken { get; set; }
}

public class TypeElement : Declaration, ITypeElement
{
    public object TypeElementBrand { get; set; }
    public QuestionToken QuestionToken { get; set; }
}

public class InterfaceDeclaration : DeclarationStatement
{
    public InterfaceDeclaration()
    {
        Kind = SyntaxKind.InterfaceDeclaration;
    }

    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public NodeArray<HeritageClause> HeritageClauses { get; set; }
    public NodeArray<ITypeElement> Members { get; set; }
}

public class HeritageClause : Node
{
    public HeritageClause()
    {
        Kind = SyntaxKind.HeritageClause;
    }

    public SyntaxKind Token { get; set; } //  SyntaxKind.ExtendsKeyword | SyntaxKind.ImplementsKeyword
    public NodeArray<ExpressionWithTypeArguments> Types { get; set; }
}

public class TypeAliasDeclaration : DeclarationStatement
{
    public TypeAliasDeclaration()
    {
        Kind = SyntaxKind.TypeAliasDeclaration;
    }

    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public ITypeNode Type { get; set; }
}

public class EnumMember : Declaration
{
    public EnumMember()
    {
        Kind = SyntaxKind.EnumMember;
    }

    public IExpression Initializer { get; set; }
}

public class EnumDeclaration : DeclarationStatement
{
    public EnumDeclaration()
    {
        Kind = SyntaxKind.EnumDeclaration;
    }

    public NodeArray<EnumMember> Members { get; set; }
}

public class ModuleDeclaration : DeclarationStatement
{
    public ModuleDeclaration()
    {
        Kind = SyntaxKind.ModuleDeclaration;
    }

    public /*ModuleDeclaration*/INode Body { get; set; } // ModuleBody | JSDocNamespaceDeclaration
}

public class NamespaceDeclaration : ModuleDeclaration
{
}

public class JsDocNamespaceDeclaration : ModuleDeclaration
{
}

public class ModuleBlock : Block
{
    public ModuleBlock()
    {
        Kind = SyntaxKind.ModuleBlock;
    }
}

public class ImportEqualsDeclaration : DeclarationStatement
{
    public ImportEqualsDeclaration()
    {
        Kind = SyntaxKind.ImportEqualsDeclaration;
    }

    public /*ModuleReference*/INode ModuleReference { get; set; }
}

public class ExternalModuleReference : Node
{
    public ExternalModuleReference()
    {
        Kind = SyntaxKind.ExternalModuleReference;
    }

    public IExpression Expression { get; set; }
}

public class ImportDeclaration : Statement
{
    public ImportDeclaration()
    {
        Kind = SyntaxKind.ImportDeclaration;
    }

    public ImportClause ImportClause { get; set; }
    public IExpression ModuleSpecifier { get; set; }
}

public class ImportClause : Declaration
{
    public ImportClause()
    {
        Kind = SyntaxKind.ImportClause;
    }

    public INamedImportBindings NamedBindings { get; set; }
}

public class NamespaceImport : Declaration, INamedImportBindings
{
    public NamespaceImport()
    {
        Kind = SyntaxKind.NamespaceImport;
    }
}

public class NamespaceExportDeclaration : DeclarationStatement
{
    public NamespaceExportDeclaration()
    {
        Kind = SyntaxKind.NamespaceExportDeclaration;
    }
}

public class ExportDeclaration : DeclarationStatement
{
    public ExportDeclaration()
    {
        Kind = SyntaxKind.ExportDeclaration;
    }

    public NamedExports ExportClause { get; set; }
    public IExpression ModuleSpecifier { get; set; }
}

public class NamedImports : Node, INamedImportsOrExports, INamedImportBindings
{
    public NamedImports()
    {
        Kind = SyntaxKind.NamedImports;
    }

    public NodeArray<ImportSpecifier> Elements { get; set; }
}

public class NamedExports : Node, INamedImportsOrExports
{
    public NamedExports()
    {
        Kind = SyntaxKind.NamedExports;
    }

    public NodeArray<ExportSpecifier> Elements { get; set; }
}

public class ImportSpecifier : Declaration, IImportOrExportSpecifier
{
    public ImportSpecifier()
    {
        Kind = SyntaxKind.ImportSpecifier;
    }

    public Identifier PropertyName { get; set; }
}

public class ExportSpecifier : Declaration, IImportOrExportSpecifier
{
    public ExportSpecifier()
    {
        Kind = SyntaxKind.ExportSpecifier;
    }

    public Identifier PropertyName { get; set; }
}

public class ExportAssignment : DeclarationStatement
{
    public ExportAssignment()
    {
        Kind = SyntaxKind.ExportAssignment;
    }

    public bool IsExportEquals { get; set; }
    public IExpression Expression { get; set; }
}

public class FileReference : TextRange
{
    public string FileName { get; set; }
}

public class CheckJsDirective : TextRange
{
    public bool Enabled { get; set; }
}

public class CommentRange : TextRange
{
    public bool HasTrailingNewLine { get; set; }
    public CommentKind Kind { get; set; }
}

public class SynthesizedComment : CommentRange
{
    public string Text { get; set; }
}

public class JsDocTypeExpression : Node
{
    public JsDocTypeExpression()
    {
        Kind = SyntaxKind.JsDocTypeExpression;
    }

    public IJsDocType Type { get; set; }
}

public interface IJsDocType : ITypeNode
{
    object JsDocTypeBrand { get; set; }
}

public class JsDocType : TypeNode, IJsDocType
{
    public object JsDocTypeBrand { get; set; }
}

public class JsDocAllType : JsDocType
{
    public JsDocAllType()
    {
        Kind = SyntaxKind.JsDocAllType;
    }
}

public class JsDocUnknownType : JsDocType
{
    public JsDocUnknownType()
    {
        Kind = SyntaxKind.JsDocUnknownType;
    }
}

public class JsDocArrayType : JsDocType
{
    public JsDocArrayType()
    {
        Kind = SyntaxKind.JsDocArrayType;
    }

    public IJsDocType ElementType { get; set; }
}

public class JsDocUnionType : JsDocType
{
    public JsDocUnionType()
    {
        Kind = SyntaxKind.JsDocUnionType;
    }

    public NodeArray<IJsDocType> Types { get; set; }
}

public class JsDocTupleType : JsDocType
{
    public JsDocTupleType()
    {
        Kind = SyntaxKind.JsDocTupleType;
    }

    public NodeArray<IJsDocType> Types { get; set; }
}

public class JsDocNonNullableType : JsDocType
{
    public JsDocNonNullableType()
    {
        Kind = SyntaxKind.JsDocNonNullableType;
    }

    public IJsDocType Type { get; set; }
}

public class JsDocNullableType : JsDocType
{
    public JsDocNullableType()
    {
        Kind = SyntaxKind.JsDocNullableType;
    }

    public IJsDocType Type { get; set; }
}

public class JsDocRecordType : JsDocType
{
    public JsDocRecordType()
    {
        Kind = SyntaxKind.JsDocRecordType;
    }

    public TypeLiteralNode Literal { get; set; }
}

public class JsDocTypeReference : JsDocType
{
    public JsDocTypeReference()
    {
        Kind = SyntaxKind.JsDocTypeReference;
    }

    public IEntityName Name { get; set; }
    public NodeArray<IJsDocType> TypeArguments { get; set; }
}

public class JsDocOptionalType : JsDocType
{
    public JsDocOptionalType()
    {
        Kind = SyntaxKind.JsDocOptionalType;
    }

    public IJsDocType Type { get; set; }
}

public class JsDocFunctionType : Node, IJsDocType, ISignatureDeclaration
{
    public JsDocFunctionType()
    {
        Kind = SyntaxKind.JsDocFunctionType;
    }

    public object JsDocTypeBrand { get; set; }
    public object TypeNodeBrand { get; set; }
    public INode Name { get; set; }
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public NodeArray<ParameterDeclaration> Parameters { get; set; }
    public ITypeNode Type { get; set; }
    public object DeclarationBrand { get; set; }
}

public class JsDocVariadicType : JsDocType
{
    public JsDocVariadicType()
    {
        Kind = SyntaxKind.JsDocVariadicType;
    }

    public IJsDocType Type { get; set; }
}

public class JsDocConstructorType : JsDocType
{
    public JsDocConstructorType()
    {
        Kind = SyntaxKind.JsDocConstructorType;
    }

    public IJsDocType Type { get; set; }
}

public class JsDocThisType : JsDocType
{
    public JsDocThisType()
    {
        Kind = SyntaxKind.JsDocThisType;
    }

    public IJsDocType Type { get; set; }
}

public class JsDocLiteralType : JsDocType
{
    public JsDocLiteralType()
    {
        Kind = SyntaxKind.JsDocLiteralType;
    }

    public LiteralTypeNode Literal { get; set; }
}

public class JsDocRecordMember : PropertySignature
{
    public JsDocRecordMember()
    {
        Kind = SyntaxKind.JsDocRecordMember;
    }
}

public class JsDoc : Node
{
    public NodeArray<IJsDocTag> Tags { get; set; }
    public string Comment { get; set; }
}

public interface IJsDocTag : INode
{
    AtToken AtToken { get; set; }
    Identifier TagName { get; set; }
    string Comment { get; set; }
}

public class JsDocTag : Node, IJsDocTag
{
    public JsDocTag()
    {
        Kind = SyntaxKind.JsDocTag;
    }

    public AtToken AtToken { get; set; }
    public Identifier TagName { get; set; }
    public string Comment { get; set; }
}

public class JsDocUnknownTag : JsDocTag
{
}

public class JsDocAugmentsTag : JsDocTag
{
    public JsDocAugmentsTag()
    {
        Kind = SyntaxKind.JsDocAugmentsTag;
    }

    public JsDocTypeExpression TypeExpression { get; set; }
}

public class JsDocTemplateTag : JsDocTag
{
    public JsDocTemplateTag()
    {
        Kind = SyntaxKind.JsDocTemplateTag;
    }

    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
}

public class JsDocReturnTag : JsDocTag
{
    public JsDocReturnTag()
    {
        Kind = SyntaxKind.JsDocReturnTag;
    }

    public JsDocTypeExpression TypeExpression { get; set; }
}

public class JsDocTypeTag : JsDocTag
{
    public JsDocTypeTag()
    {
        Kind = SyntaxKind.JsDocTypeTag;
    }

    public JsDocTypeExpression TypeExpression { get; set; }
}

public class JsDocTypedefTag : Node, IJsDocTag, IDeclaration
{
    public JsDocTypedefTag()
    {
        Kind = SyntaxKind.JsDocTypedefTag;
    }

    public INode FullName { get; set; } // JSDocNamespaceDeclaration | Identifier
    public JsDocTypeExpression TypeExpression { get; set; }
    public JsDocTypeLiteral JsDocTypeLiteral { get; set; }
    public object DeclarationBrand { get; set; }
    public INode Name { get; set; }
    public AtToken AtToken { get; set; }
    public Identifier TagName { get; set; }
    public string Comment { get; set; }
}

public class JsDocPropertyTag : Node, IJsDocTag, ITypeElement
{
    public JsDocPropertyTag()
    {
        Kind = SyntaxKind.JsDocPropertyTag;
    }

    public JsDocTypeExpression TypeExpression { get; set; }
    public AtToken AtToken { get; set; }
    public Identifier TagName { get; set; }
    public string Comment { get; set; }
    public object TypeElementBrand { get; set; }
    public INode Name { get; set; }
    public QuestionToken QuestionToken { get; set; }
    public object DeclarationBrand { get; set; }
}

public class JsDocTypeLiteral : JsDocType
{
    public JsDocTypeLiteral()
    {
        Kind = SyntaxKind.JsDocTypeLiteral;
    }

    public NodeArray<JsDocPropertyTag> JsDocPropertyTags { get; set; }
    public JsDocTypeTag JsDocTypeTag { get; set; }
}

public class JsDocParameterTag : JsDocTag
{
    public JsDocParameterTag()
    {
        Kind = SyntaxKind.JsDocParameterTag;
    }

    public Identifier PreParameterName { get; set; }
    public JsDocTypeExpression TypeExpression { get; set; }
    public Identifier PostParameterName { get; set; }
    public Identifier ParameterName { get; set; }
    public bool IsBracketed { get; set; }
}

public interface IFlowLock
{
    bool Locked { get; set; }
}

public class FlowLock : IFlowLock
{
    public bool Locked { get; set; }
}

public class AfterFinallyFlow : IFlowNode, IFlowLock
{
    public FlowNode Antecedent { get; set; }
    public bool Locked { get; set; }
    public FlowFlags Flags { get; set; }
    public int Id { get; set; }
}

public class PreFinallyFlow : FlowNode
{
    public FlowNode Antecedent { get; set; }
    public FlowLock Lock { get; set; }
}

public interface IFlowNode
{
    FlowFlags Flags { get; set; }
    int Id { get; set; }
}

public class FlowNode : IFlowNode
{
    public FlowFlags Flags { get; set; }
    public int Id { get; set; }
}

public class FlowStart : FlowNode
{
    public Node Container { get; set; } // FunctionExpression | ArrowFunction | MethodDeclaration
}

public class FlowLabel : FlowNode
{
    public FlowNode[] Antecedents { get; set; }
}

public class FlowAssignment : FlowNode
{
    public Node Node { get; set; } // Expression | VariableDeclaration | BindingElement
    public FlowNode Antecedent { get; set; }
}

public class FlowCondition : FlowNode
{
    public IExpression Expression { get; set; }
    public FlowNode Antecedent { get; set; }
}

public class FlowSwitchClause : FlowNode
{
    public SwitchStatement SwitchStatement { get; set; }
    public int ClauseStart { get; set; }
    public int ClauseEnd { get; set; }
    public FlowNode Antecedent { get; set; }
}

public class FlowArrayMutation : FlowNode
{
    public Node Node { get; set; } // CallExpression | BinaryExpression
    public FlowNode Antecedent { get; set; }
}

public class IncompleteType
{
    public TypeFlags Flags { get; set; }
    public Type Type { get; set; }
}

public class AmdDependency
{
    public string Path { get; set; }
    public string Name { get; set; }
}

public interface ISourceFileLike
{
    string Text { get; set; }
    int[] LineMap { get; set; }
}

public class SourceFile : Declaration, ISourceFileLike
{
    public SourceFile()
    {
        Kind = SyntaxKind.SourceFile;
    }

    public NodeArray<IStatement> Statements { get; set; }
    public Token EndOfFileToken { get; set; } // Token<SyntaxKind.EndOfFileToken>
    public string FileName { get; set; }
    public AmdDependency[] AmdDependencies { get; set; }
    public string ModuleName { get; set; }
    public FileReference[] ReferencedFiles { get; set; }
    public FileReference[] TypeReferenceDirectives { get; set; }
    public LanguageVariant LanguageVariant { get; set; }
    public bool IsDeclarationFile { get; set; }
    public Map<string> RenamedDependencies { get; set; }
    public bool HasNoDefaultLib { get; set; }
    public ScriptKind ScriptKind { get; set; }
    public INode ExternalModuleIndicator { get; set; }
    public Node CommonJsModuleIndicator { get; set; }
    public List<string> Identifiers { get; set; }
    public int NodeCount { get; set; }
    public int IdentifierCount { get; set; }
    public int SymbolCount { get; set; }
    public List<Diagnostic> ParseDiagnostics { get; set; }
    public List<Diagnostic> AdditionalSyntacticDiagnostics { get; set; }
    public List<Diagnostic> BindDiagnostics { get; set; }
    public Map<string> ClassifiableNames { get; set; }
    public Map<ResolvedModuleFull> ResolvedModules { get; set; }
    public Map<ResolvedTypeReferenceDirective> ResolvedTypeReferenceDirectiveNames { get; set; }
    public LiteralExpression[] Imports { get; set; }
    public LiteralExpression[] ModuleAugmentations { get; set; }
    public PatternAmbientModule[] PatternAmbientModules { get; set; }
    public string[] AmbientModuleNames { get; set; }
    public TextRange CheckJsDirective { get; set; } // CheckJsDirective
    public string Text { get; set; }
    public int[] LineMap { get; set; }
}

public class Bundle : Node
{
    public Bundle()
    {
        Kind = SyntaxKind.Bundle;
    }

    public SourceFile[] SourceFiles { get; set; }
}

public class ScriptReferenceHost
{
}

public class ParseConfigHost
{
    public bool UseCaseSensitiveFileNames { get; set; }
}

public class WriteFileCallback
{
}

public class Program : ScriptReferenceHost
{
    public bool StructureIsReused { get; set; }
}

public class CustomTransformers
{
    public TransformerFactory<SourceFile>[] Before { get; set; }
    public TransformerFactory<SourceFile>[] After { get; set; }
}

public class SourceMapSpan
{
    public int EmittedLine { get; set; }
    public int EmittedColumn { get; set; }
    public int SourceLine { get; set; }
    public int SourceColumn { get; set; }
    public int NameIndex { get; set; }
    public int SourceIndex { get; set; }
}

public class SourceMapData
{
    public string SourceMapFilePath { get; set; }
    public string JsSourceMappingUrl { get; set; }
    public string SourceMapFile { get; set; }
    public string SourceMapSourceRoot { get; set; }
    public string[] SourceMapSources { get; set; }
    public string[] SourceMapSourcesContent { get; set; }
    public string[] InputSourceFileNames { get; set; }
    public string[] SourceMapNames { get; set; }
    public string SourceMapMappings { get; set; }
    public SourceMapSpan[] SourceMapDecodedMappings { get; set; }
}

public class EmitResult
{
    public bool EmitSkipped { get; set; }
    public Diagnostic[] Diagnostics { get; set; }
    public string[] EmittedFiles { get; set; }
    public SourceMapData[] SourceMaps { get; set; }
}

public class TypeCheckerHost
{
}

public class TypeChecker
{
}

public class SymbolDisplayBuilder
{
}

public class SymbolWriter
{
}

public class TypePredicateBase
{
    public TypePredicateKind Kind { get; set; }
    public Type Type { get; set; }
}

public class ThisTypePredicate : TypePredicateBase
{
    public ThisTypePredicate()
    {
        Kind = TypePredicateKind.This;
    }
}

public class IdentifierTypePredicate : TypePredicateBase
{
    public IdentifierTypePredicate()
    {
        Kind = TypePredicateKind.Identifier;
    }

    public string ParameterName { get; set; }
    public int ParameterIndex { get; set; }
}

public class SymbolVisibilityResult
{
    public SymbolAccessibility Accessibility { get; set; }
    public IAnyImportSyntax[] AliasesToMakeVisible { get; set; }
    public string ErrorSymbolName { get; set; }
    public Node ErrorNode { get; set; }
}

public class SymbolAccessibilityResult : SymbolVisibilityResult
{
    public string ErrorModuleName { get; set; }
}

public class EmitResolver
{
}

public interface ISymbol
{
    SymbolFlags Flags { get; set; }
    string Name { get; set; }
    Declaration[] Declarations { get; set; }
    Declaration ValueDeclaration { get; set; }
    SymbolTable Members { get; set; }
    SymbolTable Exports { get; set; }
    SymbolTable GlobalExports { get; set; }
    int Id { get; set; }
    int MergeId { get; set; }
    Symbol Parent { get; set; }
    Symbol ExportSymbol { get; set; }
    bool ConstEnumOnlyModule { get; set; }
    bool IsReferenced { get; set; }
    bool IsReplaceableByMethod { get; set; }
    bool IsAssigned { get; set; }
}

public class Symbol : ISymbol
{
    public SymbolFlags Flags { get; set; }
    public string Name { get; set; }
    public Declaration[] Declarations { get; set; }
    public Declaration ValueDeclaration { get; set; }
    public SymbolTable Members { get; set; }
    public SymbolTable Exports { get; set; }
    public SymbolTable GlobalExports { get; set; }
    public int Id { get; set; }
    public int MergeId { get; set; }
    public Symbol Parent { get; set; }
    public Symbol ExportSymbol { get; set; }
    public bool ConstEnumOnlyModule { get; set; }
    public bool IsReferenced { get; set; }
    public bool IsReplaceableByMethod { get; set; }
    public bool IsAssigned { get; set; }
}

public interface ISymbolLinks
{
    Symbol Target { get; set; }
    Type Type { get; set; }
    Type DeclaredType { get; set; }
    TypeParameter[] TypeParameters { get; set; }
    Type InferredClassType { get; set; }
    Map<Type> Instantiations { get; set; }
    TypeMapper Mapper { get; set; }
    bool Referenced { get; set; }
    UnionOrIntersectionType ContainingType { get; set; }
    Symbol LeftSpread { get; set; }
    Symbol RightSpread { get; set; }
    Symbol MappedTypeOrigin { get; set; }
    bool IsDiscriminantProperty { get; set; }
    SymbolTable ResolvedExports { get; set; }
    bool ExportsChecked { get; set; }
    bool TypeParametersChecked { get; set; }
    bool IsDeclarationWithCollidingName { get; set; }
    BindingElement BindingElement { get; set; }
    bool ExportsSomeValue { get; set; }
}

public class SymbolLinks : ISymbolLinks
{
    public Symbol Target { get; set; }
    public Type Type { get; set; }
    public Type DeclaredType { get; set; }
    public TypeParameter[] TypeParameters { get; set; }
    public Type InferredClassType { get; set; }
    public Map<Type> Instantiations { get; set; }
    public TypeMapper Mapper { get; set; }
    public bool Referenced { get; set; }
    public UnionOrIntersectionType ContainingType { get; set; }
    public Symbol LeftSpread { get; set; }
    public Symbol RightSpread { get; set; }
    public Symbol MappedTypeOrigin { get; set; }
    public bool IsDiscriminantProperty { get; set; }
    public SymbolTable ResolvedExports { get; set; }
    public bool ExportsChecked { get; set; }
    public bool TypeParametersChecked { get; set; }
    public bool IsDeclarationWithCollidingName { get; set; }
    public BindingElement BindingElement { get; set; }
    public bool ExportsSomeValue { get; set; }
}

public class TransientSymbol : ISymbol, ISymbolLinks
{
    public CheckFlags CheckFlags { get; set; }
    public SymbolFlags Flags { get; set; }
    public string Name { get; set; }
    public Declaration[] Declarations { get; set; }
    public Declaration ValueDeclaration { get; set; }
    public SymbolTable Members { get; set; }
    public SymbolTable Exports { get; set; }
    public SymbolTable GlobalExports { get; set; }
    public int Id { get; set; }
    public int MergeId { get; set; }
    public Symbol Parent { get; set; }
    public Symbol ExportSymbol { get; set; }
    public bool ConstEnumOnlyModule { get; set; }
    public bool IsReferenced { get; set; }
    public bool IsReplaceableByMethod { get; set; }
    public bool IsAssigned { get; set; }
    public Symbol Target { get; set; }
    public Type Type { get; set; }
    public Type DeclaredType { get; set; }
    public TypeParameter[] TypeParameters { get; set; }
    public Type InferredClassType { get; set; }
    public Map<Type> Instantiations { get; set; }
    public TypeMapper Mapper { get; set; }
    public bool Referenced { get; set; }
    public UnionOrIntersectionType ContainingType { get; set; }
    public Symbol LeftSpread { get; set; }
    public Symbol RightSpread { get; set; }
    public Symbol MappedTypeOrigin { get; set; }
    public bool IsDiscriminantProperty { get; set; }
    public SymbolTable ResolvedExports { get; set; }
    public bool ExportsChecked { get; set; }
    public bool TypeParametersChecked { get; set; }
    public bool IsDeclarationWithCollidingName { get; set; }
    public BindingElement BindingElement { get; set; }
    public bool ExportsSomeValue { get; set; }
}

public class Pattern
{
    public string Prefix { get; set; }
    public string Suffix { get; set; }
}

public class PatternAmbientModule
{
    public Pattern Pattern { get; set; }
    public Symbol Symbol { get; set; }
}

public class NodeLinks
{
    public NodeCheckFlags Flags { get; set; }
    public Type ResolvedType { get; set; }
    public Signature ResolvedSignature { get; set; }
    public Symbol ResolvedSymbol { get; set; }
    public IndexInfo ResolvedIndexInfo { get; set; }
    public bool MaybeTypePredicate { get; set; }
    public int EnumMemberValue { get; set; }
    public bool IsVisible { get; set; }
    public bool HasReportedStatementInAmbientContext { get; set; }
    public JsxFlags JsxFlags { get; set; }
    public Type ResolvedJsxElementAttributesType { get; set; }
    public bool HasSuperCall { get; set; }
    public ExpressionStatement SuperCall { get; set; }
    public Type[] SwitchTypes { get; set; }
}

public interface IType
{
    TypeFlags Flags { get; set; }
    int Id { get; set; }
    Symbol Symbol { get; set; }
    IDestructuringPattern Pattern { get; set; }
    Symbol AliasSymbol { get; set; }
    Type[] AliasTypeArguments { get; set; }
}

public class Type : IType
{
    public TypeFlags Flags { get; set; }
    public int Id { get; set; }
    public Symbol Symbol { get; set; }
    public IDestructuringPattern Pattern { get; set; }
    public Symbol AliasSymbol { get; set; }
    public Type[] AliasTypeArguments { get; set; }
}

public class IntrinsicType : Type
{
    public string IntrinsicName { get; set; }
}

public class LiteralType : Type
{
    public string Text { get; set; }
    public LiteralType FreshType { get; set; }
    public LiteralType RegularType { get; set; }
}

public class EnumType : Type
{
    public EnumLiteralType[] MemberTypes { get; set; }
}

public class EnumLiteralType : LiteralType
{
}

public interface IObjectType : IType
{
    ObjectFlags ObjectFlags { get; set; }
}

public class ObjectType : Type, IObjectType
{
    public ObjectFlags ObjectFlags { get; set; }
}

public interface IInterfaceType : IObjectType
{
    TypeParameter[] TypeParameters { get; set; }
    TypeParameter[] OuterTypeParameters { get; set; }
    TypeParameter[] LocalTypeParameters { get; set; }
    TypeParameter ThisType { get; set; }
    Type ResolvedBaseConstructorType { get; set; }
    IBaseType[] ResolvedBaseTypes { get; set; }
}

public class InterfaceType : ObjectType, IInterfaceType
{
    public TypeParameter[] TypeParameters { get; set; }
    public TypeParameter[] OuterTypeParameters { get; set; }
    public TypeParameter[] LocalTypeParameters { get; set; }
    public TypeParameter ThisType { get; set; }
    public Type ResolvedBaseConstructorType { get; set; }
    public IBaseType[] ResolvedBaseTypes { get; set; }
}

public class InterfaceTypeWithDeclaredMembers : InterfaceType
{
    public Symbol[] DeclaredProperties { get; set; }
    public Signature[] DeclaredCallSignatures { get; set; }
    public Signature[] DeclaredConstructSignatures { get; set; }
    public IndexInfo DeclaredStringIndexInfo { get; set; }
    public IndexInfo DeclaredNumberIndexInfo { get; set; }
}

public interface ITypeReference : IObjectType
{
    GenericType Target { get; set; }
    Type[] TypeArguments { get; set; }
}

public class TypeReference : ObjectType, ITypeReference
{
    public GenericType Target { get; set; }
    public Type[] TypeArguments { get; set; }
}

public class GenericType : ObjectType, IInterfaceType, ITypeReference
{
    public Map<TypeReference> Instantiations { get; set; }
    public TypeParameter[] TypeParameters { get; set; }
    public TypeParameter[] OuterTypeParameters { get; set; }
    public TypeParameter[] LocalTypeParameters { get; set; }
    public TypeParameter ThisType { get; set; }
    public Type ResolvedBaseConstructorType { get; set; }
    public IBaseType[] ResolvedBaseTypes { get; set; }
    public GenericType Target { get; set; }
    public Type[] TypeArguments { get; set; }
}

public interface IUnionOrIntersectionType : IType
{
    Type[] Types { get; set; }
    SymbolTable PropertyCache { get; set; }
    Symbol[] ResolvedProperties { get; set; }
    IndexType ResolvedIndexType { get; set; }
    Type ResolvedBaseConstraint { get; set; }
    bool CouldContainTypeVariables { get; set; }
}

public class UnionOrIntersectionType : Type, IUnionOrIntersectionType
{
    public Type[] Types { get; set; }
    public SymbolTable PropertyCache { get; set; }
    public Symbol[] ResolvedProperties { get; set; }
    public IndexType ResolvedIndexType { get; set; }
    public Type ResolvedBaseConstraint { get; set; }
    public bool CouldContainTypeVariables { get; set; }
}

public interface IUnionType : IUnionOrIntersectionType
{
}

public class UnionType : UnionOrIntersectionType, IUnionType
{
}

public class IntersectionType : UnionOrIntersectionType
{
    public Type ResolvedApparentType { get; set; }
}

public class AnonymousType : ObjectType
{
    public AnonymousType Target { get; set; }
    public TypeMapper Mapper { get; set; }
}

public class MappedType : ObjectType
{
    public MappedTypeNode Declaration { get; set; }
    public TypeParameter TypeParameter { get; set; }
    public Type ConstraintType { get; set; }
    public Type TemplateType { get; set; }
    public Type ModifiersType { get; set; }
    public TypeMapper Mapper { get; set; }
}

public class EvolvingArrayType : ObjectType
{
    public Type ElementType { get; set; }
    public Type FinalArrayType { get; set; }
}

public class ResolvedType : Type, IObjectType, IUnionOrIntersectionType
{
    public SymbolTable Members { get; set; }
    public Symbol[] Properties { get; set; }
    public Signature[] CallSignatures { get; set; }
    public Signature[] ConstructSignatures { get; set; }
    public IndexInfo StringIndexInfo { get; set; }
    public IndexInfo NumberIndexInfo { get; set; }
    public ObjectFlags ObjectFlags { get; set; }
    public Type[] Types { get; set; }
    public SymbolTable PropertyCache { get; set; }
    public Symbol[] ResolvedProperties { get; set; }
    public IndexType ResolvedIndexType { get; set; }
    public Type ResolvedBaseConstraint { get; set; }
    public bool CouldContainTypeVariables { get; set; }
}

public class FreshObjectLiteralType : ResolvedType
{
    public ResolvedType RegularType { get; set; }
}

public class IterableOrIteratorType : Type, IObjectType, IUnionType
{
    public Type IteratedTypeOfIterable { get; set; }
    public Type IteratedTypeOfIterator { get; set; }
    public Type IteratedTypeOfAsyncIterable { get; set; }
    public Type IteratedTypeOfAsyncIterator { get; set; }
    public ObjectFlags ObjectFlags { get; set; }
    public Type[] Types { get; set; }
    public SymbolTable PropertyCache { get; set; }
    public Symbol[] ResolvedProperties { get; set; }
    public IndexType ResolvedIndexType { get; set; }
    public Type ResolvedBaseConstraint { get; set; }
    public bool CouldContainTypeVariables { get; set; }
}

public class PromiseOrAwaitableType : Type, IObjectType, IUnionType
{
    public Type PromiseTypeOfPromiseConstructor { get; set; }
    public Type PromisedTypeOfPromise { get; set; }
    public Type AwaitedTypeOfType { get; set; }
    public ObjectFlags ObjectFlags { get; set; }
    public Type[] Types { get; set; }
    public SymbolTable PropertyCache { get; set; }
    public Symbol[] ResolvedProperties { get; set; }
    public IndexType ResolvedIndexType { get; set; }
    public Type ResolvedBaseConstraint { get; set; }
    public bool CouldContainTypeVariables { get; set; }
}

public class TypeVariable : Type
{
    public Type ResolvedBaseConstraint { get; set; }
    public IndexType ResolvedIndexType { get; set; }
}

public class TypeParameter : TypeVariable
{
    public Type Constraint { get; set; }
    public Type Default { get; set; }
    public TypeParameter Target { get; set; }
    public TypeMapper Mapper { get; set; }
    public bool IsThisType { get; set; }
    public Type ResolvedDefaultType { get; set; }
}

public class IndexedAccessType : TypeVariable
{
    public Type ObjectType { get; set; }
    public Type IndexType { get; set; }
    public Type Constraint { get; set; }
}

public class IndexType : Type
{
    public Type Type { get; set; } // TypeVariable | UnionOrIntersectionType
}

public class Signature
{
    public SignatureDeclaration Declaration { get; set; }
    public TypeParameter[] TypeParameters { get; set; }
    public Symbol[] Parameters { get; set; }
    public Symbol ThisParameter { get; set; }
    public Type ResolvedReturnType { get; set; }
    public int MinArgumentCount { get; set; }
    public bool HasRestParameter { get; set; }
    public bool HasLiteralTypes { get; set; }
    public Signature Target { get; set; }
    public TypeMapper Mapper { get; set; }
    public Signature[] UnionSignatures { get; set; }
    public Signature ErasedSignatureCache { get; set; }
    public ObjectType IsolatedSignatureType { get; set; }
    public ITypePredicate TypePredicate { get; set; }
    public Map<Signature> Instantiations { get; set; }
}

public class IndexInfo
{
    public Type Type { get; set; }
    public bool IsReadonly { get; set; }
    public SignatureDeclaration Declaration { get; set; }
}

public class TypeMapper
{
    public Type[] MappedTypes { get; set; }
    public Type[] Instantiations { get; set; }
    public InferenceContext Context { get; set; }
}

public class TypeInferences
{
    public Type[] Primary { get; set; }
    public Type[] Secondary { get; set; }
    public bool TopLevel { get; set; }
    public bool IsFixed { get; set; }
}

public class InferenceContext
{
    public Signature Signature { get; set; }
    public bool InferUnionTypes { get; set; }
    public TypeInferences[] Inferences { get; set; }
    public Type[] InferredTypes { get; set; }
    public TypeMapper Mapper { get; set; }
    public int FailedTypeParameterIndex { get; set; }
    public bool UseAnyForNoInferences { get; set; }
}

public class JsFileExtensionInfo
{
    public string Extension { get; set; }
    public bool IsMixedContent { get; set; }
}

public class DiagnosticMessage
{
    public string Key { get; set; }
    public DiagnosticCategory Category { get; set; }
    public int Code { get; set; }
    public string Message { get; set; }
}

public class DiagnosticMessageChain
{
    public string MessageText { get; set; }
    public DiagnosticCategory Category { get; set; }
    public int Code { get; set; }
    public DiagnosticMessageChain Next { get; set; }
}

public class Diagnostic
{
    public SourceFile File { get; set; }
    public int Start { get; set; }
    public int Length { get; set; }
    public object MessageText { get; set; } //  string | DiagnosticMessageChain
    public DiagnosticCategory Category { get; set; }
    public int Code { get; set; }
}

public class PluginImport
{
    public string Name { get; set; }
}

public class CompilerOptions
{
    public bool All { get; set; }
    public bool AllowJs { get; set; }
    public bool AllowNonTsExtensions { get; set; }
    public bool AllowSyntheticDefaultImports { get; set; }
    public bool AllowUnreachableCode { get; set; }
    public bool AllowUnusedLabels { get; set; }
    public bool AlwaysStrict { get; set; }
    public string BaseUrl { get; set; }
    public string Charset { get; set; }
    public bool CheckJs { get; set; }
    public string ConfigFilePath { get; set; }
    public bool Declaration { get; set; }
    public string DeclarationDir { get; set; }
    public bool Diagnostics { get; set; }
    public bool ExtendedDiagnostics { get; set; }
    public bool DisableSizeLimit { get; set; }
    public bool DownlevelIteration { get; set; }
    public bool EmitBom { get; set; }
    public bool EmitDecoratorMetadata { get; set; }
    public bool ExperimentalDecorators { get; set; }
    public bool ForceConsistentCasingInFileNames { get; set; }
    public bool Help { get; set; }
    public bool ImportHelpers { get; set; }
    public bool Init { get; set; }
    public bool InlineSourceMap { get; set; }
    public bool InlineSources { get; set; }
    public bool IsolatedModules { get; set; }
    public JsxEmit Jsx { get; set; }
    public string[] Lib { get; set; }
    public bool ListEmittedFiles { get; set; }
    public bool ListFiles { get; set; }
    public string Locale { get; set; }
    public string MapRoot { get; set; }
    public int MaxNodeModuleJsDepth { get; set; }
    public ModuleKind Module { get; set; }
    public ModuleResolutionKind ModuleResolution { get; set; }
    public NewLineKind NewLine { get; set; }
    public bool NoEmit { get; set; }
    public bool NoEmitForJsFiles { get; set; }
    public bool NoEmitHelpers { get; set; }
    public bool NoEmitOnError { get; set; }
    public bool NoErrorTruncation { get; set; }
    public bool NoFallthroughCasesInSwitch { get; set; }
    public bool NoImplicitAny { get; set; }
    public bool NoImplicitReturns { get; set; }
    public bool NoImplicitThis { get; set; }
    public bool NoUnusedLocals { get; set; }
    public bool NoUnusedParameters { get; set; }
    public bool NoImplicitUseStrict { get; set; }
    public bool NoLib { get; set; }
    public bool NoResolve { get; set; }
    public string Out { get; set; }
    public string OutDir { get; set; }
    public string OutFile { get; set; }
    public MapLike<string[]> Paths { get; set; }
    public PluginImport[] Plugins { get; set; }
    public bool PreserveConstEnums { get; set; }
    public string Project { get; set; }
    public DiagnosticStyle Pretty { get; set; }
    public string ReactNamespace { get; set; }
    public string JsxFactory { get; set; }
    public bool RemoveComments { get; set; }
    public string RootDir { get; set; }
    public string[] RootDirs { get; set; }
    public bool SkipLibCheck { get; set; }
    public bool SkipDefaultLibCheck { get; set; }
    public bool SourceMap { get; set; }
    public string SourceRoot { get; set; }
    public bool Strict { get; set; }
    public bool StrictNullChecks { get; set; }
    public bool StripInternal { get; set; }
    public bool SuppressExcessPropertyErrors { get; set; }
    public bool SuppressImplicitAnyIndexErrors { get; set; }
    public bool SuppressOutputPathCheck { get; set; }
    public bool TraceResolution { get; set; }
    public string[] Types { get; set; }
    public string[] TypeRoots { get; set; }
    public bool Version { get; set; }
    public bool Watch { get; set; }
}

public class TypeAcquisition
{
    public bool EnableAutoDiscovery { get; set; }
    public bool Enable { get; set; }
    public string[] Include { get; set; }
    public string[] Exclude { get; set; }
}

public class DiscoverTypingsInfo
{
    public string[] FileNames { get; set; }
    public string ProjectRootPath { get; set; }
    public string SafeListPath { get; set; }
    public Map<string> PackageNameToTypingLocation { get; set; }
    public TypeAcquisition TypeAcquisition { get; set; }
    public CompilerOptions CompilerOptions { get; set; }
    public ReadonlyArray<string> UnresolvedImports { get; set; }
}

public class LineAndCharacter
{
    public int Line { get; set; }
    public int Character { get; set; }
}

public class ParsedCommandLine
{
    public CompilerOptions Options { get; set; }
    public TypeAcquisition TypeAcquisition { get; set; }
    public string[] FileNames { get; set; }
    public object Raw { get; set; }
    public Diagnostic[] Errors { get; set; }
    public MapLike<WatchDirectoryFlags> WildcardDirectories { get; set; }
    public bool CompileOnSave { get; set; }
}

public class ExpandResult
{
    public string[] FileNames { get; set; }
    public MapLike<WatchDirectoryFlags> WildcardDirectories { get; set; }
}

public class CommandLineOptionBase
{
    public string Name { get; set; }
    public object Type { get; set; } //  "string" | "number" | "boolean" | "object" | "list" | Map<number | string>
    public bool IsFilePath { get; set; }
    public string ShortName { get; set; }
    public DiagnosticMessage Description { get; set; }
    public DiagnosticMessage ParamType { get; set; }
    public bool IsTsConfigOnly { get; set; }
    public bool IsCommandLineOnly { get; set; }
    public bool ShowInSimplifiedHelpView { get; set; }
    public DiagnosticMessage Category { get; set; }
}

public class CommandLineOptionOfPrimitiveType : CommandLineOptionBase
{
}

public class CommandLineOptionOfCustomType : CommandLineOptionBase
{
}

public class TsConfigOnlyOption : CommandLineOptionBase
{
}

public class CommandLineOptionOfListType : CommandLineOptionBase
{
    public CommandLineOptionBase
        Element
    {
        get;
        set;
    } // CommandLineOptionOfCustomType | CommandLineOptionOfPrimitiveType | TsConfigOnlyOption
}

public class ModuleResolutionHost
{
}

public class ResolvedModule
{
    public string ResolvedFileName { get; set; }
    public bool IsExternalLibraryImport { get; set; }
}

public class ResolvedModuleFull : ResolvedModule
{
    public Extension Extension { get; set; }
}

public class ResolvedModuleWithFailedLookupLocations
{
    public ResolvedModule ResolvedModule { get; set; } // ResolvedModuleFull
    public string[] FailedLookupLocations { get; set; }
}

public class ResolvedTypeReferenceDirective
{
    public bool Primary { get; set; }
    public string ResolvedFileName { get; set; }
}

public class ResolvedTypeReferenceDirectiveWithFailedLookupLocations
{
    public ResolvedTypeReferenceDirective ResolvedTypeReferenceDirective { get; set; }
    public string[] FailedLookupLocations { get; set; }
}

public class CompilerHost : ModuleResolutionHost
{
    public WriteFileCallback WriteFile { get; set; }
}

public class EmitNode
{
    public Node[] AnnotatedNodes { get; set; }
    public EmitFlags Flags { get; set; }
    public SynthesizedComment[] LeadingComments { get; set; }
    public SynthesizedComment[] TrailingComments { get; set; }
    public TextRange CommentRange { get; set; }
    public TextRange SourceMapRange { get; set; }
    public TextRange[] TokenSourceMapRanges { get; set; }
    public int ConstantValue { get; set; }
    public Identifier ExternalHelpersModuleName { get; set; }
    public EmitHelper[] Helpers { get; set; }
}

public class EmitHelper
{
    public string Name { get; set; }
    public bool Scoped { get; set; }
    public string Text { get; set; }
    public int Priority { get; set; }
}

public class EmitHost : ScriptReferenceHost
{
    public WriteFileCallback WriteFile { get; set; }
}

public class TransformationContext
{
    // onSubstituteNode { get; set; }
    // onEmitNode { get; set; }
}

public class TransformationResult<T>
{
    public T[] Transformed { get; set; }
    public Diagnostic[] Diagnostics { get; set; }
}

public class Printer
{
}

public class PrintHandlers
{
    // onEmitSourceMapOfNode { get; set; }
    // onEmitSourceMapOfToken { get; set; }
    // onEmitSourceMapOfPosition { get; set; }
    // onEmitHelpers { get; set; }
    // onSetSourceFile { get; set; }
    // onBeforeEmitNodeArray { get; set; }
    // onAfterEmitNodeArray { get; set; }
}

public class PrinterOptions
{
    public bool RemoveComments { get; set; }
    public NewLineKind NewLine { get; set; }
    public bool SourceMap { get; set; }
    public bool InlineSourceMap { get; set; }
    public bool ExtendedDiagnostics { get; set; }
}

public class EmitTextWriter
{
}

public class TextSpan
{
    public int Start { get; set; }
    public int Length { get; set; }
}

public class TextChangeRange
{
    public TextSpan Span { get; set; }
    public int NewLength { get; set; }
}
