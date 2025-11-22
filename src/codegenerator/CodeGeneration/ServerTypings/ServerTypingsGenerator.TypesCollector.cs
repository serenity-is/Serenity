#if ISSOURCEGENERATOR
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Threading;

namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator
{
    internal class ExportedTypesCollector(CancellationToken cancellation) : SymbolVisitor
    {
        private readonly HashSet<INamedTypeSymbol> _exportedTypes = new(SymbolEqualityComparer.Default);
        public ImmutableArray<INamedTypeSymbol> GetPublicTypes() => _exportedTypes.ToImmutableArray();

        public override void VisitAssembly(IAssemblySymbol symbol)
        {
            cancellation.ThrowIfCancellationRequested();
            symbol.GlobalNamespace.Accept(this);
        }

        public override void VisitNamespace(INamespaceSymbol symbol)
        {
            foreach (INamespaceOrTypeSymbol namespaceOrType in symbol.GetMembers())
            {
                cancellation.ThrowIfCancellationRequested();
                namespaceOrType.Accept(this);
            }
        }

        public static bool IsAccessibleOutsideOfAssembly(ISymbol symbol) =>
            symbol.DeclaredAccessibility switch
            {
                Accessibility.Protected => true,
                Accessibility.ProtectedOrInternal => true,
                Accessibility.Public => true,
                _ => false
            };

        public override void VisitNamedType(INamedTypeSymbol type)
        {
            cancellation.ThrowIfCancellationRequested();

            if (!IsAccessibleOutsideOfAssembly(type) || !_exportedTypes.Add(type))
                return;

            var nestedTypes = type.GetTypeMembers();

            if (nestedTypes.IsDefaultOrEmpty)
                return;

            foreach (INamedTypeSymbol nestedType in nestedTypes)
            {
                cancellation.ThrowIfCancellationRequested();
                nestedType.Accept(this);
            }
        }
    }

}
#endif