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

    string GetText(string sourceText = null);
    string GetTextWithTrivia(string sourceText = null);

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

internal interface IHasChildren : INode
{
    IEnumerable<INode> Children { get; set; }
}

internal interface IHasJSDoc : INode
{
    JSDocArray JSDoc { get; set; }
}

internal interface IDeclaration : INode
{
}

internal interface IEntityName : INode
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

internal interface IHasName : INode
{
    IDeclarationName Name { get; }
}

internal interface INamedDeclaration : IDeclaration, IHasName
{
}

internal interface IVariableLikeDeclaration : INamedDeclaration
{
}

internal interface ITypeElement : INamedDeclaration
{
    QuestionToken QuestionToken { get; }
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

internal interface IFunctionLikeDeclaration : ISignatureDeclaration
{
    AsteriskToken AsteriskToken { get; set; }
    QuestionToken QuestionToken { get; set; }
    IBlockOrExpression Body { get; set; } // Block | Expression
}

internal interface IMethodOrAccessorDeclaration : IFunctionLikeDeclaration, IClassElement, IObjectLiteralElement
{
}

internal interface IAccessorDeclaration : IMethodOrAccessorDeclaration, ISignatureDeclaration, ITypeElement, IObjectLiteralElementLike
{
}

internal interface IClassLikeDeclaration : INamedDeclaration
{
    NodeArray<TypeParameterDeclaration> TypeParameters { get; }
    NodeArray<HeritageClause> HeritageClauses { get; }
    NodeArray<IClassElement> Members { get; }
}

internal interface IClassElement : IDeclaration
{
}


internal interface ITypeNode : INode
{
}

internal interface IKeywordTypeNode : ITypeNode
{
}

internal interface ISignatureDeclaration : INamedDeclaration
{
    NodeArray<TypeParameterDeclaration> TypeParameters { get; }
    NodeArray<ParameterDeclaration> Parameters { get; }
    ITypeNode Type { get; }
}

internal interface IFunctionOrConstructorTypeNode : ISignatureDeclaration, ITypeNode
{
}


internal interface IBlockOrExpression : INode
{
}

internal interface IVariableDeclarationListOrExpression : INode
{
}

internal interface IVariableDeclarationList : INode, IVariableDeclarationListOrExpression
{
    NodeArray<VariableDeclaration> Declarations { get; set; }
}

internal interface IExpression : IBlockOrExpression, IVariableDeclarationListOrExpression
{
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

internal interface ITemplateLiteral : INode
{
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

internal interface IAssertionExpression : IExpression
{
}

internal interface IJsxOpeningLikeElement : IExpression
{
}

internal interface IJsxAttributeLike : IObjectLiteralElement
{
}

internal interface IJsxTagNameExpression : IExpression
{
}

internal interface IJsxChild : INode
{
}

internal interface IBlockLike : INode
{
}

internal interface IForInitializer : INode
{
}

internal interface IBreakOrContinueStatement : IStatement
{
    Identifier Label { get; set; }
}

internal interface ICaseOrDefaultClause : INode
{
}

internal interface IDeclarationWithTypeParameters : INode
{
}

internal interface IModuleName : INode
{
}

internal interface IModuleBody : INode
{
}

internal interface INamespaceBody : INode
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