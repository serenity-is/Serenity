namespace Serenity.TypeScript;


public class JsDocNamespaceDeclaration : ModuleDeclaration
{
}

public class JsDocTypeExpression : Node
{
    public JsDocTypeExpression()
    {
        Kind = SyntaxKind.JSDocTypeExpression;
    }

    public IJsDocType Type { get; set; }
}

public interface IJsDocType : ITypeNode
{
}

public class JsDocType : TypeNode, IJsDocType
{
}

public class JsDocAllType : JsDocType
{
    public JsDocAllType()
    {
        Kind = SyntaxKind.JSDocAllType;
    }
}

public class JsDocUnknownType : JsDocType
{
    public JsDocUnknownType()
    {
        Kind = SyntaxKind.JSDocUnknownType;
    }
}

public class JsDocNonNullableType : JsDocType
{
    public JsDocNonNullableType()
    {
        Kind = SyntaxKind.JSDocNonNullableType;
    }

    public ITypeNode Type { get; set; }
    public bool Postfix { get; set; }
}

public class JsDocNullableType : JsDocType
{
    public JsDocNullableType()
    {
        Kind = SyntaxKind.JSDocNullableType;
    }

    public ITypeNode Type { get; set; }
    public bool Postfix { get; set; }
}

public class JsDocOptionalType : JsDocType
{
    public JsDocOptionalType()
    {
        Kind = SyntaxKind.JSDocOptionalType;
    }

    public IJsDocType Type { get; set; }
}

public class JsDocFunctionType : Node, IJsDocType, ISignatureDeclaration
{
    public JsDocFunctionType()
    {
        Kind = SyntaxKind.JSDocFunctionType;
    }

    public INode Name { get; set; }
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
    public NodeArray<ParameterDeclaration> Parameters { get; set; }
    public ITypeNode Type { get; set; }
}

public class JsDocVariadicType : JsDocType
{
    public JsDocVariadicType()
    {
        Kind = SyntaxKind.JSDocVariadicType;
    }

    public IJsDocType Type { get; set; }
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
        Kind = SyntaxKind.JSDocTag;
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
        Kind = SyntaxKind.JSDocAugmentsTag;
    }

    public JsDocTypeExpression TypeExpression { get; set; }
}

public class JsDocTemplateTag : JsDocTag
{
    public JsDocTemplateTag()
    {
        Kind = SyntaxKind.JSDocTemplateTag;
    }

    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }
}

public class JsDocReturnTag : JsDocTag
{
    public JsDocReturnTag()
    {
        Kind = SyntaxKind.JSDocReturnTag;
    }

    public JsDocTypeExpression TypeExpression { get; set; }
}

public class JsDocTypeTag : JsDocTag
{
    public JsDocTypeTag()
    {
        Kind = SyntaxKind.JSDocTypeTag;
    }

    public JsDocTypeExpression TypeExpression { get; set; }
}

public class JsDocTypedefTag : Node, IJsDocTag, IDeclaration
{
    public JsDocTypedefTag()
    {
        Kind = SyntaxKind.JSDocTypedefTag;
    }

    public INode FullName { get; set; } // JSDocNamespaceDeclaration | Identifier
    public JsDocTypeExpression TypeExpression { get; set; }
    public JsDocTypeLiteral JsDocTypeLiteral { get; set; }
    public INode Name { get; set; }
    public AtToken AtToken { get; set; }
    public Identifier TagName { get; set; }
    public string Comment { get; set; }
}

public class JsDocPropertyTag : Node, IJsDocTag, ITypeElement
{
    public JsDocPropertyTag()
    {
        Kind = SyntaxKind.JSDocPropertyTag;
    }

    public JsDocTypeExpression TypeExpression { get; set; }
    public AtToken AtToken { get; set; }
    public Identifier TagName { get; set; }
    public string Comment { get; set; }
    public INode Name { get; set; }
    public QuestionToken QuestionToken { get; set; }
}

public class JsDocTypeLiteral : JsDocType
{
    public JsDocTypeLiteral()
    {
        Kind = SyntaxKind.JSDocTypeLiteral;
    }

    public NodeArray<JsDocPropertyTag> JsDocPropertyTags { get; set; }
    public JsDocTypeTag JsDocTypeTag { get; set; }
}

public class JsDocParameterTag : JsDocTag
{
    public JsDocParameterTag()
    {
        Kind = SyntaxKind.JSDocParameterTag;
    }

    public Identifier PreParameterName { get; set; }
    public JsDocTypeExpression TypeExpression { get; set; }
    public Identifier PostParameterName { get; set; }
    public Identifier ParameterName { get; set; }
    public bool IsBracketed { get; set; }
}
