namespace Serenity.TypeScript;
internal interface ITextRange
{
    int? Pos { get; set; }
    int? End { get; set; }
}

internal interface INode : ITextRange
{
    SyntaxKind Kind { get; set; }
    internal NodeFlags Flags { get; set; }

    INode Parent { get; set; }

    string ToString(bool withPos);
}

internal interface ISyntaxCursor
{
    public INode CurrentNode(int pos);
}

internal interface IModifierLike : INode
{
}

internal interface IModifier : IModifierLike
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


internal interface IHasModifierLike : INode
{
    NodeArray<IModifierLike> Modifiers { get; }
}

internal interface IHasDecorators : IHasModifierLike
{
}

internal interface IHasModifiers : IHasModifierLike
{
}

internal interface IGetRestChildren : INode
{
    IEnumerable<INode> GetRestChildren();
}

internal interface IHasJSDoc : INode
{
    JSDocArray JSDoc { get; set; }
}

internal interface IStringLiteralLike : IDeclarationName
{
}

internal interface ILiteralLikeNode : INode
{
    string Text { get; set; }
    bool IsUnterminated { get; set; }
    bool HasExtendedUnicodeEscape { get; set; }
    bool IsOctalLiteral { get; set; }
}

internal interface ILiteralExpression : ILiteralLikeNode, IPrimaryExpression
{
}


internal interface IHasLiteralText : INode
{
    string Text { get; set; }
}

internal interface IDeclaration : INode
{
}

internal interface IEntityName : INode, IModuleReference
{
}


internal interface IDeclarationName : INode
{
}

internal interface IPropertyName : IDeclarationName
{
}

internal interface IBindingName : IDeclarationName
{
}

internal interface IHasNameProperty : INode
{
    IDeclarationName Name { get; }
}

internal interface INamedDeclaration : IDeclaration, IHasNameProperty
{
}

internal interface IVariableLikeDeclaration : INamedDeclaration
{
}

internal interface ITypeElement : INamedDeclaration
{
}


internal interface IJsDocContainer : INode
{
}

internal interface ILocalsContainer : INode
{
}

internal interface IFlowContainer : INode
{
}

internal interface IObjectLiteralElementLike : INode
{
}

internal interface IObjectLiteralElement : INamedDeclaration, IObjectLiteralElementLike
{
}

internal interface IObjectTypeDeclaration : IDeclaration
{
}

internal interface IFunctionLikeDeclaration : ISignatureDeclaration
{
    AsteriskToken AsteriskToken { get; set; }
    QuestionToken QuestionToken { get; set; }
    IBlockOrExpression Body { get; set; } // Block | Expression
}

internal interface IMethodOrAccessorDeclaration : IFunctionLikeDeclaration, IClassElement, IObjectLiteralElement
{
}

internal interface IAccessorDeclaration : IMethodOrAccessorDeclaration, ISignatureDeclaration, 
    ITypeElement, IObjectLiteralElementLike, IDeclarationWithTypeParameterChildren
{
}

internal interface IClassLikeDeclaration : INamedDeclaration, IObjectTypeDeclaration, IDeclarationWithTypeParameterChildren
{
    NodeArray<HeritageClause> HeritageClauses { get; }
    NodeArray<IClassElement> Members { get; }
}

internal interface IClassElement : INamedDeclaration
{
}


internal interface ITypeNode : INode
{
}

internal interface IKeywordTypeNode : ITypeNode
{
}

internal interface IDeclarationStatement : INamedDeclaration, IStatement
{
}

internal interface ISignatureDeclaration : INamedDeclaration, IDeclarationWithTypeParameterChildren
{
    NodeArray<ParameterDeclaration> Parameters { get; }
    ITypeNode Type { get; }
}

internal interface IFunctionOrConstructorTypeNode : ISignatureDeclaration, ITypeNode
{
}


internal interface IBlockOrExpression : INode
{
}

internal interface IVariableDeclarationList : INode, IForInitializer
{
    NodeArray<VariableDeclaration> Declarations { get; }
}

internal interface IIntersectsChange : INode
{
    bool InterectsChange { get; }
}


internal interface IJsxAttributeName : IDeclarationName
{
}

internal interface IBindingPattern : INode, IBindingName
{
    NodeArray<IArrayBindingElement> Elements { get; }
}

internal interface IArrayBindingElement : INode
{
}

internal interface IUnionOrIntersectionTypeNode : ITypeNode
{
    NodeArray<ITypeNode> Types { get; set; }
}

internal interface IConciseBody : INode
{
}

internal interface ITemplateLiteralLikeNode : ILiteralLikeNode
{
    string RawText { get; }
    TokenFlags? TemplateFlags { get; }
}

internal interface IEntityNameExpression : INode
{
}

internal interface IEntityNameOrEntityNameExpression : INode
{
}

internal interface ISuperProperty : INode
{
}

internal interface ICallLikeExpression : INode
{
}


internal interface IExpression : IBlockOrExpression, IForInitializer
{
}

internal interface IUnaryExpression : IExpression
{
}

internal interface IAssertionExpression : IExpression
{
}

internal interface IJsxNamespacedNameOrIdentifier : IExpression
{

}

internal interface IJsxOpeningLikeElementOrOpeningFragment : INode
{
}

internal interface IJsxElementOrSelfClosingOrFragment : IJsxChild, IPrimaryExpression, IJsxAttributeValue
{
}

internal interface IJsxOpeningLikeElement : IJsxOpeningLikeElementOrOpeningFragment, IExpression
{
}

internal interface IJsxAttributeLike : IObjectLiteralElement
{
}

internal interface IJsxAttributeValue : IExpression
{
}

internal interface IJsxTagNameExpression : IExpression
{
}

internal interface IJsxHasTagName : INode
{
    IJsxTagNameExpression TagName { get; }
}

internal interface IJsxChild : INode
{
}

internal interface IBlockLike : INode
{
    NodeArray<IStatement> Statements { get; }
}

internal interface IForInitializer : INode
{
}

internal interface IBreakOrContinueStatement : IStatement
{
    Identifier Label { get; }
}

internal interface ICaseOrDefaultClause : INode, IBlockLike
{
}

internal interface IDeclarationWithTypeParameterChildren : INode, IDeclarationWithTypeParameters
{
    NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
}

internal interface IDeclarationWithTypeParameters : INode
{
}

internal interface IModuleName : INode, IDeclarationName
{
}

internal interface IModuleBody : INode
{
}

internal interface INamespaceBody : INode, IModuleBody
{
}

internal interface IJsDocNamespaceBody : INode
{
}

internal interface IModuleReference : INode
{
}

internal interface INamedImportsOrExports : INode
{
}


internal interface INamedImportBindings : INode, INamedImportsOrExports
{
}

internal interface INamedExportBindings : INode, INamedImportsOrExports
{
}


internal interface IImportOrExportSpecifier : INamedDeclaration
{
    Identifier PropertyName { get; }
    bool IsTypeOnly { get; }
}

internal interface IJsDocTypeReferencingNode : IJSDocType
{
}

internal interface IAnyImportSyntax : INode
{
}

internal interface IDestructuringPattern : INode
{
}

internal interface IUpdateExpression : IUnaryExpression
{
}

internal interface ILeftHandSideExpression : IUpdateExpression
{
}

internal interface IMemberExpression : ILeftHandSideExpression
{
}

internal interface IPrimaryExpression : IMemberExpression
{
}

internal interface IStatement : INode
{
}