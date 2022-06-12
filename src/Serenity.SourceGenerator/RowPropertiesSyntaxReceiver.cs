using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Serenity.SourceGenerator;

internal class RowPropertiesSyntaxReceiver : ISyntaxContextReceiver
{
    private ITypeSymbol generateRowFieldsAttr;

    public List<IPropertySymbol> Properties { get; } = new();

    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        if (context.Node is ClassDeclarationSyntax classDeclaration)
        {
            generateRowFieldsAttr ??= context.SemanticModel.Compilation.GetTypeByMetadataName("Serenity.ComponentModel.GenerateRowFieldsAttribute");

            if (context.SemanticModel.GetDeclaredSymbol(classDeclaration) is not ITypeSymbol typeSymbol)
                return;

            if (typeSymbol.GetAttributes().Any(x => 
                SymbolEqualityComparer.Default.Equals(x.AttributeClass, generateRowFieldsAttr)))
            {
                foreach (var propertyDeclaration in context.Node.ChildNodes().OfType<PropertyDeclarationSyntax>())
                {
                    if (context.SemanticModel.GetDeclaredSymbol(propertyDeclaration) is IPropertySymbol propertySymbol)
                        Properties.Add(propertySymbol);
                }
            }
        }
    }
}