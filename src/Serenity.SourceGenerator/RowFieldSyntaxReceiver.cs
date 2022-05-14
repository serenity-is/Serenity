using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Serenity.SourceGenerator;

internal class PrivateRowFieldsSyntaxReceiver : ISyntaxContextReceiver
{
    private ITypeSymbol generatedRowAttribute;

    public List<IFieldSymbol> PrivateFields { get; } = new List<IFieldSymbol>();

    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        if (context.Node is ClassDeclarationSyntax classDeclaration)
        {
            generatedRowAttribute ??= context.SemanticModel.Compilation.GetTypeByMetadataName(typeof(GeneratedRowAttribute).FullName);

            if (context.SemanticModel.GetDeclaredSymbol(classDeclaration) is not ITypeSymbol typeSymbol)
                return;

            if (typeSymbol.GetAttributes().Any(x => 
                SymbolEqualityComparer.Default.Equals(x.AttributeClass, generatedRowAttribute)))
            {
                foreach (var fieldDeclaration in context.Node.ChildNodes().OfType<FieldDeclarationSyntax>())
                {
                    foreach (var variable in fieldDeclaration.Declaration.Variables)
                    {
                        if (context.SemanticModel.GetDeclaredSymbol(variable) is IFieldSymbol fieldSymbol)
                            PrivateFields.Add(fieldSymbol);
                    }
                }
            }
        }
    }
}