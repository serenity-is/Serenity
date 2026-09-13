namespace Serenity.TypeScript;

public class JSDocTemplateTag() : JSDocTag(SyntaxKind.JSDocTemplateTag), IDeclarationWithTypeParameterChildren
{
    public NodeArray<TypeParameterDeclaration>? TypeParameters { get; set; }

}