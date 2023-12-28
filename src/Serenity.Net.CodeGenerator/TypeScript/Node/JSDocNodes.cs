namespace Serenity.TypeScript;

internal class JSDocTypeExpression : NodeBase
{
    public JSDocTypeExpression()
    {
        Kind = SyntaxKind.JSDocTypeExpression;
    }

    public ITypeNode Type { get; set; }
}

internal interface IJSDocType : ITypeNode
{
}

internal class JSDocType(SyntaxKind kind) : TypeNodeBase(kind), IJSDocType
{
}

internal class JSDocAllType() : JSDocType(SyntaxKind.JSDocAllType)
{
}

internal class JSDocUnknownType() : JSDocType(SyntaxKind.JSDocUnknownType)
{
}

internal class JSDocNonNullableType(ITypeNode type, bool postfix) : JSDocType(SyntaxKind.JSDocNonNullableType)
{
    public ITypeNode Type { get; } = type;
    public bool Postfix { get; } = postfix;
}

internal class JSDocNullableType(ITypeNode type, bool postfix) : JSDocType(SyntaxKind.JSDocNullableType)
{
    public ITypeNode Type { get; } = type;
    public bool Postfix { get; } = postfix;
}

internal class JSDocOptionalType(ITypeNode type) : JSDocType(SyntaxKind.JSDocOptionalType)
{

    public ITypeNode Type { get; } = type;
}

internal class JSDocFunctionType : NodeBase, IJSDocType, ISignatureDeclaration, IHasName
{
    public JSDocFunctionType(NodeArray<ParameterDeclaration> parameters, ITypeNode type)
    {
        Kind = SyntaxKind.JSDocFunctionType;
        Parameters = parameters;
        Type = type;
    }

    public NodeArray<ParameterDeclaration> Parameters { get; set; }
    public ITypeNode Type { get; set; }
    public IDeclarationName Name { get; set; }
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
}

internal class JSDocVariadicType(ITypeNode type) : JSDocType(SyntaxKind.JSDocVariadicType)
{
    public ITypeNode Type { get; } = type;
}

internal class JSDoc : NodeBase
{
    public NodeArray<IJSDocTag> Tags { get; set; }
    public string Comment { get; set; }
}

internal class JSDocArray : List<JSDoc>
{
    public JSDocArray()
    {
    }

    public JSDocArray(JSDoc[] elements)
        : base(elements.ToList())
    {
    }

    public List<IJSDocTag> JSDocCache { get; set; }
}

internal interface IJSDocTag : INode
{
    AtToken AtToken { get; set; }
    Identifier TagName { get; set; }
    string Comment { get; set; }
}

internal class JSDocTag : NodeBase, IJSDocTag
{
    public JSDocTag()
    {
        Kind = SyntaxKind.JSDocTag;
    }

    public AtToken AtToken { get; set; }
    public Identifier TagName { get; set; }
    public string Comment { get; set; }
}

internal class JSDocUnknownTag : JSDocTag
{
}

internal class JSDocAugmentsTag : JSDocTag
{
    public JSDocAugmentsTag()
    {
        Kind = SyntaxKind.JSDocAugmentsTag;
    }

    public JSDocTypeExpression TypeExpression { get; set; }
}

internal class JSDocTemplateTag : JSDocTag
{
    public JSDocTemplateTag()
    {
        Kind = SyntaxKind.JSDocTemplateTag;
    }

    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
}

internal class JSDocReturnTag : JSDocTag
{
    public JSDocReturnTag()
    {
        Kind = SyntaxKind.JSDocReturnTag;
    }

    public JSDocTypeExpression TypeExpression { get; set; }
}

internal class JSDocTypeTag : JSDocTag
{
    public JSDocTypeTag()
    {
        Kind = SyntaxKind.JSDocTypeTag;
    }

    public JSDocTypeExpression TypeExpression { get; set; }
}

internal class JSDocTypedefTag : NodeBase, IJSDocTag, IHasName
{
    public JSDocTypedefTag()
    {
        Kind = SyntaxKind.JSDocTypedefTag;
    }

    public INode FullName { get; set; } // JSDocNamespaceDeclaration | Identifier
    public JSDocTypeExpression TypeExpression { get; set; }
    public JSDocTypeLiteral JSDocTypeLiteral { get; set; }
    public IDeclarationName Name { get; set; }
    public AtToken AtToken { get; set; }
    public Identifier TagName { get; set; }
    public string Comment { get; set; }
}

internal class JSDocPropertyTag : NodeBase, IJSDocTag, ITypeElement, IHasName
{
    public JSDocPropertyTag()
    {
        Kind = SyntaxKind.JSDocPropertyTag;
    }

    public JSDocTypeExpression TypeExpression { get; set; }
    public AtToken AtToken { get; set; }
    public Identifier TagName { get; set; }
    public string Comment { get; set; }
    public IDeclarationName Name { get; set; }
    public QuestionToken QuestionToken { get; set; }
}

internal class JSDocTypeLiteral() : JSDocType(SyntaxKind.JSDocTypeLiteral)
{
    public NodeArray<JSDocPropertyTag> JSDocPropertyTags { get; set; }
    public JSDocTypeTag JSDocTypeTag { get; set; }
}

internal class JSDocParameterTag : JSDocTag
{
    public JSDocParameterTag()
    {
        Kind = SyntaxKind.JSDocParameterTag;
    }

    public Identifier PreParameterName { get; set; }
    public JSDocTypeExpression TypeExpression { get; set; }
    public Identifier PostParameterName { get; set; }
    public Identifier ParameterName { get; set; }
    public bool IsBracketed { get; set; }
}

internal class JSDocNamepathType(ITypeNode type) : JSDocType(SyntaxKind.JSDocNamepathType)
{
    public ITypeNode Type { get; } = type;
}
