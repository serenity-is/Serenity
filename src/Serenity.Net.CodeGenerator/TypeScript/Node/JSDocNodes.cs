namespace Serenity.TypeScript;

internal class JSDocTypeExpression : Node, IGetRestChildren
{
    public JSDocTypeExpression()
    {
        Kind = SyntaxKind.JSDocTypeExpression;
    }

    public ITypeNode Type { get; set; }

    public IEnumerable<INode> GetRestChildren()
    {
        return [Type];
    }
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

internal class JSDocNonNullableType(ITypeNode type, bool postfix) 
    : JSDocType(SyntaxKind.JSDocNonNullableType), IGetRestChildren
{
    public ITypeNode Type { get; } = type;
    public bool Postfix { get; } = postfix;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Type];
    }
}

internal class JSDocNullableType(ITypeNode type, bool postfix) 
    : JSDocType(SyntaxKind.JSDocNullableType), IGetRestChildren
{
    public ITypeNode Type { get; } = type;
    public bool Postfix { get; } = postfix;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Type];
    }
}

internal class JSDocOptionalType(ITypeNode type) 
    : JSDocType(SyntaxKind.JSDocOptionalType), IGetRestChildren
{
    public ITypeNode Type { get; } = type;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Type];
    }
}

internal class JSDocFunctionType : Node, IJSDocType, ISignatureDeclaration, IHasNameProperty, IGetRestChildren

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

    public IEnumerable<INode> GetRestChildren()
    {
        if (Parameters != null) foreach (var x in Parameters) yield return x;
        yield return Type;
        yield return Name;
        if (TypeParameters != null) foreach (var x in TypeParameters) yield return x;
    }
}

internal class JSDocVariadicType(ITypeNode type) 
    : JSDocType(SyntaxKind.JSDocVariadicType), IGetRestChildren
{
    public ITypeNode Type { get; } = type;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Type];
    }
}

internal class JSDoc : Node, IGetRestChildren
{
    public NodeArray<IJSDocTag> Tags { get; set; }
    public string Comment { get; set; }

    public IEnumerable<INode> GetRestChildren()
    {
        if (Tags != null) foreach (var x in Tags) yield return x;
    }
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

internal class JSDocTag : Node, IJSDocTag, IGetRestChildren
{
    public JSDocTag()
    {
        Kind = SyntaxKind.JSDocTag;
    }

    public AtToken AtToken { get; set; }
    public Identifier TagName { get; set; }
    public string Comment { get; set; }

    public virtual IEnumerable<INode> GetRestChildren()
    {
        return [AtToken, TagName];
    }
}

internal class JSDocUnknownTag : JSDocTag
{
}

internal class JSDocAugmentsTag : JSDocTag, IGetRestChildren
{
    public JSDocAugmentsTag()
    {
        Kind = SyntaxKind.JSDocAugmentsTag;
    }

    public JSDocTypeExpression TypeExpression { get; set; }

    public override IEnumerable<INode> GetRestChildren()
    {
        foreach (var x in base.GetRestChildren())
            yield return x;
        yield return TypeExpression;
    }
}

internal class JSDocTemplateTag : JSDocTag, IDeclarationWithTypeParameterChildren
{
    public JSDocTemplateTag()
    {
        Kind = SyntaxKind.JSDocTemplateTag;
    }

    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }

}

internal class JSDocReturnTag : JSDocTag, IGetRestChildren
{
    public JSDocReturnTag()
    {
        Kind = SyntaxKind.JSDocReturnTag;
    }

    public JSDocTypeExpression TypeExpression { get; set; }

    public override IEnumerable<INode> GetRestChildren()
    {
        foreach (var x in base.GetRestChildren())
            yield return x;

        yield return TypeExpression;
    }
}

internal class JSDocTypeTag : JSDocTag, IGetRestChildren
{
    public JSDocTypeTag()
    {
        Kind = SyntaxKind.JSDocTypeTag;
    }

    public JSDocTypeExpression TypeExpression { get; set; }

    public override IEnumerable<INode> GetRestChildren()
    {
        foreach (var x in base.GetRestChildren())
            yield return x;

        yield return TypeExpression;
    }
}

internal class JSDocTypedefTag : Node, IJSDocTag, IHasNameProperty, IGetRestChildren, IDeclarationWithTypeParameters
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

    public IEnumerable<INode> GetRestChildren()
    {
        return [FullName, TypeExpression, JSDocTypeLiteral, Name, AtToken, TagName];
    }
}

internal class JSDocPropertyTag : Node, IJSDocTag, ITypeElement, IHasNameProperty, IGetRestChildren
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

    public IEnumerable<INode> GetRestChildren()
    {
        return [TypeExpression, AtToken, TagName, AtToken, Name, QuestionToken];
    }
}

internal class JSDocTypeLiteral() : JSDocType(SyntaxKind.JSDocTypeLiteral), IGetRestChildren
{
    public NodeArray<JSDocPropertyTag> JSDocPropertyTags { get; set; }
    public JSDocTypeTag JSDocTypeTag { get; set; }

    public IEnumerable<INode> GetRestChildren()
    {
        if (JSDocPropertyTags != null) foreach (var x in JSDocPropertyTags) yield return x;
        yield return JSDocTypeTag;
    }
}

internal class JSDocParameterTag : JSDocTag, IGetRestChildren
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

    public override IEnumerable<INode> GetRestChildren()
    {
        foreach (var x in base.GetRestChildren())
            yield return x;

        yield return PreParameterName;
        yield return TypeExpression;
        yield return PostParameterName;
        yield return ParameterName;
    }
}

internal class JSDocNamepathType(ITypeNode type)
    : JSDocType(SyntaxKind.JSDocNamepathType), IGetRestChildren
{
    public ITypeNode Type { get; } = type;

    public IEnumerable<INode> GetRestChildren()
    {
        yield return Type;
    }
}