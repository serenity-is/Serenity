namespace Serenity.TypeScript;

internal class JSDocTemplateTag : JSDocTag, IDeclarationWithTypeParameterChildren
{
    public JSDocTemplateTag()
    {
        Kind = SyntaxKind.JSDocTemplateTag;
    }

    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }

}