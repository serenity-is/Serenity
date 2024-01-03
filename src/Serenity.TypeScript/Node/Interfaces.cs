namespace Serenity.TypeScript;

public interface ITextRange
{
    int? Pos { get; set; }
    int? End { get; set; }
}

public interface INode : ITextRange
{
    SyntaxKind Kind { get; set; }
    NodeFlags Flags { get; set; }

    INode Parent { get; set; }

    string ToString(bool withPos);
}

public interface ISyntaxCursor
{
    public INode CurrentNode(int pos);
}

public interface IModifierLike : INode
{
}

public interface IModifier : IModifierLike
{
}

public class NodeArray<T> : List<T>, ITextRange
{
    public static readonly NodeArray<T> EmptyArray = [];

    public NodeArray()
    {
    }

    public NodeArray(IEnumerable<T> elements)
        : base(elements)
    {
    }

    public bool HasTrailingComma { get; set; }
    public int? Pos { get; set; }
    public int? End { get; set; }
    public bool IsMissingList { get; set; }
}


public interface IHasModifierLike : INode
{
    NodeArray<IModifierLike> Modifiers { get; }
}

public interface IHasDecorators : IHasModifierLike
{
}

public interface IHasModifiers : IHasModifierLike
{
}

public interface IGetRestChildren : INode
{
    IEnumerable<INode> GetRestChildren();
}

public interface IHasJSDoc : INode
{
    JSDocArray JSDoc { get; set; }
}

public interface IStringLiteralLike : IDeclarationName
{
}

public interface ILiteralLikeNode : INode
{
    string Text { get; set; }
    bool IsUnterminated { get; set; }
    bool HasExtendedUnicodeEscape { get; set; }
    bool IsOctalLiteral { get; set; }
}

public interface ILiteralExpression : ILiteralLikeNode, IPrimaryExpression
{
}


public interface IHasLiteralText : INode
{
    string Text { get; set; }
}

public interface IDeclaration : INode
{
}

public interface IEntityName : INode, IModuleReference
{
}


public interface IDeclarationName : INode
{
}

public interface IPropertyName : IDeclarationName
{
}

public interface IBindingName : IDeclarationName
{
}

public interface IHasNameProperty : INode
{
    IDeclarationName Name { get; }
}

public interface INamedDeclaration : IDeclaration, IHasNameProperty
{
}

public interface IVariableLikeDeclaration : INamedDeclaration
{
}

public interface ITypeElement : INamedDeclaration
{
}


public interface IJsDocContainer : INode
{
}

public interface ILocalsContainer : INode
{
}

public interface IFlowContainer : INode
{
}

public interface IObjectLiteralElementLike : INode
{
}

public interface IObjectLiteralElement : INamedDeclaration, IObjectLiteralElementLike
{
}

public interface IObjectTypeDeclaration : IDeclaration
{
}

public interface IFunctionLikeDeclaration : ISignatureDeclaration
{
    AsteriskToken AsteriskToken { get; set; }
    QuestionToken QuestionToken { get; set; }
    IBlockOrExpression Body { get; set; } // Block | Expression
}

public interface IMethodOrAccessorDeclaration : IFunctionLikeDeclaration, IClassElement, IObjectLiteralElement
{
}

public interface IAccessorDeclaration : IMethodOrAccessorDeclaration, ISignatureDeclaration, 
    ITypeElement, IObjectLiteralElementLike, IDeclarationWithTypeParameterChildren
{
}

public interface IClassLikeDeclaration : INamedDeclaration, IObjectTypeDeclaration, IDeclarationWithTypeParameterChildren
{
    NodeArray<HeritageClause> HeritageClauses { get; }
    NodeArray<IClassElement> Members { get; }
}

public interface IClassElement : INamedDeclaration
{
}


public interface ITypeNode : INode
{
}

public interface IKeywordTypeNode : ITypeNode
{
}

public interface IInferTypeNode : ITypeNode
{
    TypeParameterDeclaration TypeParameter { get; }
}

public interface IDeclarationStatement : INamedDeclaration, IStatement
{
}

public interface ISignatureDeclaration : INamedDeclaration, IDeclarationWithTypeParameterChildren
{
    NodeArray<ParameterDeclaration> Parameters { get; }
    ITypeNode Type { get; }
}

public interface IFunctionOrConstructorTypeNode : ISignatureDeclaration, ITypeNode
{
}


public interface IBlockOrExpression : INode
{
}

public interface IVariableDeclarationList : INode, IForInitializer
{
    NodeArray<VariableDeclaration> Declarations { get; }
}

public interface IIntersectsChange : INode
{
    bool InterectsChange { get; }
}


public interface IJsxAttributeName : IDeclarationName
{
}

public interface IBindingPattern : INode, IBindingName
{
    NodeArray<IArrayBindingElement> Elements { get; }
}

public interface IArrayBindingElement : INode
{
}

public interface IUnionOrIntersectionTypeNode : ITypeNode
{
    NodeArray<ITypeNode> Types { get; set; }
}

public interface IConciseBody : INode
{
}

public interface ITemplateLiteralLikeNode : ILiteralLikeNode
{
    string RawText { get; }
    TokenFlags? TemplateFlags { get; }
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


public interface IExpression : IBlockOrExpression, IForInitializer
{
}

public interface IUnaryExpression : IExpression
{
}

public interface IAssertionExpression : IExpression
{
}

public interface IJsxNamespacedNameOrIdentifier : IExpression
{

}

public interface IJsxOpeningLikeElementOrOpeningFragment : INode
{
}

public interface IJsxElementOrSelfClosingOrFragment : IJsxChild, IPrimaryExpression, IJsxAttributeValue
{
}

public interface IJsxOpeningLikeElement : IJsxOpeningLikeElementOrOpeningFragment, IExpression
{
}

public interface IJsxAttributeLike : IObjectLiteralElement
{
}

public interface IJsxAttributeValue : IExpression
{
}

public interface IJsxTagNameExpression : IExpression
{
}

public interface IJsxHasTagName : INode
{
    IJsxTagNameExpression TagName { get; }
}

public interface IJsxChild : INode
{
}

public interface IBlockLike : INode
{
    NodeArray<IStatement> Statements { get; }
}

public interface IForInitializer : INode
{
}

public interface IBreakOrContinueStatement : IStatement
{
    Identifier Label { get; }
}

public interface ICaseOrDefaultClause : INode, IBlockLike
{
}

public interface IDeclarationWithTypeParameterChildren : INode, IDeclarationWithTypeParameters
{
    NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
}

public interface IDeclarationWithTypeParameters : INode
{
}

public interface IModuleName : INode, IDeclarationName
{
}

public interface IModuleBody : INode
{
}

public interface INamespaceBody : INode, IModuleBody
{
}

public interface IJsDocNamespaceBody : INode
{
}

public interface IModuleReference : INode
{
}

public interface INamedImportsOrExports : INode
{
}


public interface INamedImportBindings : INode, INamedImportsOrExports
{
}

public interface INamedExportBindings : INode, INamedImportsOrExports
{
}


public interface IImportOrExportSpecifier : INamedDeclaration
{
    Identifier PropertyName { get; }
    bool IsTypeOnly { get; }
}

public interface IJsDocTypeReferencingNode : IJSDocType
{
}

public interface IAnyImportSyntax : INode
{
}

public interface IDestructuringPattern : INode
{
}

public interface IUpdateExpression : IUnaryExpression
{
}

public interface ILeftHandSideExpression : IUpdateExpression
{
}

public interface IMemberExpression : ILeftHandSideExpression
{
}

public interface IPrimaryExpression : IMemberExpression
{
}

public interface IStatement : INode
{
}

public interface IJSDocType : ITypeNode
{
}

public interface IJSDocTag : INode
{
    AtToken AtToken { get; set; }
    Identifier TagName { get; set; }
    string Comment { get; set; }
}