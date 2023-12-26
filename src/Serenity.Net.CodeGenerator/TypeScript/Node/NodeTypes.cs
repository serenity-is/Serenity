namespace Serenity.TypeScript;


public class Modifier : Node
{
}

public interface IEntityName : INode
{
}

public interface IPropertyName : INode
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
    IBlockOrExpression Body { get; set; } //  Block | Expression
}

public interface IFunctionOrConstructorTypeNode : ISignatureDeclaration, ITypeNode
{
}

public interface IUnionOrIntersectionTypeNode : ITypeNode
{
    NodeArray<ITypeNode> Types { get; set; }
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
public interface IAnyImportSyntax : INode
{
}

public interface IDestructuringPattern : INode
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


public class Identifier : PrimaryExpression, IJsxTagNameExpression, IEntityName, IPropertyName
{
    public Identifier()
    {
        Kind = SyntaxKind.Identifier;
    }

    public string Text { get; set; }
    public SyntaxKind OriginalKeywordKind { get; set; }
    public bool IsInJsDocNamespace { get; set; }
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
    INode Name { get; set; }
}


public interface IDeclarationStatement : INode, IDeclaration, IStatement
{
    //Node name { get; set; } // Identifier | StringLiteral | NumericLiteral
}

public class DeclarationStatement : Node, IDeclarationStatement, IDeclaration, IStatement
{
    public INode Name { get; set; }
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
}

public class ObjectLiteralElement : Declaration, IObjectLiteralElement
{
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
    AsteriskToken AsteriskToken { get; set; }
    QuestionToken QuestionToken { get; set; }
    IBlockOrExpression Body { get; set; } // Block | Expression
}

public class FunctionLikeDeclaration : SignatureDeclaration, IFunctionLikeDeclaration
{
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

    public AsteriskToken AsteriskToken { get; set; }
    public QuestionToken QuestionToken { get; set; }
    public IBlockOrExpression Body { get; set; } //  Block | Expression
    public INode Name { get; set; }
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public NodeArray<ParameterDeclaration> Parameters { get; set; }
    public ITypeNode Type { get; set; }
}

public class MethodSignature : Declaration, ISignatureDeclaration, ITypeElement, IFunctionLikeDeclaration
{
    public MethodSignature()
    {
        Kind = SyntaxKind.MethodSignature;
    }

    public AsteriskToken AsteriskToken { get; set; }
    public IBlockOrExpression Body { get; set; } //  Block | Expression
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public NodeArray<ParameterDeclaration> Parameters { get; set; }
    public ITypeNode Type { get; set; }
    public QuestionToken QuestionToken { get; set; }
}

public class MethodDeclaration : Declaration, IFunctionLikeDeclaration, IClassElement, IObjectLiteralElement,
    IObjectLiteralElementLike
{
    public MethodDeclaration()
    {
        Kind = SyntaxKind.MethodDeclaration;
    }

    public AsteriskToken AsteriskToken { get; set; }
    public QuestionToken QuestionToken { get; set; }
    public IBlockOrExpression Body { get; set; } //  Block | Expression
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public NodeArray<ParameterDeclaration> Parameters { get; set; }
    public ITypeNode Type { get; set; }
}

public class ConstructorDeclaration : Declaration, IFunctionLikeDeclaration, IClassElement
{
    public ConstructorDeclaration()
    {
        Kind = SyntaxKind.Constructor;
    }

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

    public AsteriskToken AsteriskToken { get; set; }
    public QuestionToken QuestionToken { get; set; }
    public IBlockOrExpression Body { get; set; } //  Block | Expression
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public NodeArray<ParameterDeclaration> Parameters { get; set; }
    public ITypeNode Type { get; set; }
}

public class SetAccessorDeclaration : Declaration, IFunctionLikeDeclaration, IClassElement, IObjectLiteralElement,
    IAccessorDeclaration
{
    public SetAccessorDeclaration()
    {
        Kind = SyntaxKind.SetAccessor;
    }

    public AsteriskToken AsteriskToken { get; set; }
    public QuestionToken QuestionToken { get; set; }
    public IBlockOrExpression Body { get; set; } //  Block | Expression
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public NodeArray<ParameterDeclaration> Parameters { get; set; }
    public ITypeNode Type { get; set; }
}

public class IndexSignatureDeclaration : Declaration, ISignatureDeclaration, IClassElement, ITypeElement
{
    public IndexSignatureDeclaration()
    {
        Kind = SyntaxKind.IndexSignature;
    }

    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public NodeArray<ParameterDeclaration> Parameters { get; set; }
    public ITypeNode Type { get; set; }
    public QuestionToken QuestionToken { get; set; }
}

public interface ITypeNode : INode
{
}

public class TypeNode : Node, ITypeNode
{
}

public interface IKeywordTypeNode : ITypeNode
{
}

public class KeywordTypeNode : TypeNode, IKeywordTypeNode
{
}

public interface IInferTypeNode : ITypeNode
{
    TypeParameterDeclaration TypeParameter { get; set; }
    TransformFlags TransformFlags { get; set; }
}

public class InferTypeNode : TypeNode, IInferTypeNode
{
    public InferTypeNode()
    {
        Kind = SyntaxKind.InferType;
    }
    public TypeParameterDeclaration TypeParameter { get; set; }
    public TransformFlags TransformFlags { get; set; }
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
    public NodeArray<ITypeNode> TypeArguments { get; set; }
}

public class TypeLiteralNode : Node, ITypeNode, IDeclaration
{
    public TypeLiteralNode()
    {
        Kind = SyntaxKind.TypeLiteral;
    }

    public NodeArray<ITypeElement> Members { get; set; }
    public INode Name { get; set; }
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
    public INode Name { get; set; }
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
}

public class Expression : Node, IExpression
{
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
}

public class UnaryExpression : Expression, IUnaryExpression
{
}

public interface IIncrementExpression : IUnaryExpression
{
}

public class IncrementExpression : UnaryExpression, IIncrementExpression
{
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
}

public class LeftHandSideExpression : IncrementExpression, ILeftHandSideExpression
{
}

public interface IMemberExpression : ILeftHandSideExpression
{
}

public class MemberExpression : LeftHandSideExpression, IMemberExpression
{
}

public interface IPrimaryExpression : IMemberExpression
{
}

public class PrimaryExpression : MemberExpression, IPrimaryExpression, IJsxTagNameExpression
{
}

public class NullLiteral : Node, IPrimaryExpression, ITypeNode
{
    public NullLiteral()
    {
        Kind = SyntaxKind.NullKeyword;
    }

}

public class BooleanLiteral : Node, IPrimaryExpression, ITypeNode
{
    public BooleanLiteral()
    {
        Kind = SyntaxKind.BooleanKeyword;
    }

}

public class ThisExpression : Node, IPrimaryExpression, IKeywordTypeNode
{
    public ThisExpression()
    {
        Kind = SyntaxKind.ThisKeyword;
    }

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
    public INode Name { get; set; }
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

    public AsteriskToken AsteriskToken { get; set; }
    public QuestionToken QuestionToken { get; set; }
    public IBlockOrExpression Body { get; set; } //  Block | Expression
    public INode Name { get; set; }
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public NodeArray<ParameterDeclaration> Parameters { get; set; }
    public ITypeNode Type { get; set; }
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
    public AsteriskToken AsteriskToken { get; set; }
    public QuestionToken QuestionToken { get; set; }
    public IBlockOrExpression Body { get; set; } //  Block | Expression
    public INode Name { get; set; }
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public NodeArray<ParameterDeclaration> Parameters { get; set; }
    public ITypeNode Type { get; set; }
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
}

public class LiteralExpression : Node, ILiteralExpression, IPrimaryExpression
{
    public string Text { get; set; }
    public bool IsUnterminated { get; set; }
    public bool HasExtendedUnicodeEscape { get; set; }
    public bool IsOctalLiteral { get; set; }
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
    public INode Name { get; set; }
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
    public INode Name { get; set; }
}

public class SuperPropertyAccessExpression : PropertyAccessExpression
{
}

public class PropertyAccessEntityNameExpression : PropertyAccessExpression
{
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
    public INode Name { get; set; }
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
}

public class Statement : Node, IStatement
{
}

public class NotEmittedStatement : Statement
{
    public NotEmittedStatement()
    {
        Kind = SyntaxKind.NotEmittedStatement;
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

    public INode Name { get; set; }
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
}

public interface IClassElement : IDeclaration
{
}

public class ClassElement : Declaration, IClassElement
{
}

public interface ITypeElement : IDeclaration
{
    QuestionToken QuestionToken { get; set; }
}

public class TypeElement : Declaration, ITypeElement
{
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

public class CommentRange : TextRange
{
    public bool HasTrailingNewLine { get; set; }
    public SyntaxKind Kind { get; set; }
}

public class SynthesizedComment : CommentRange
{
    public string Text { get; set; }
}
