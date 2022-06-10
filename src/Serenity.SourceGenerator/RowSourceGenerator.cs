using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Serenity.Reflection;

namespace Serenity.SourceGenerator
{
    [Generator]
    public class RowSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context) 
        {
            context.RegisterForSyntaxNotifications(() => new PrivateRowFieldsSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxContextReceiver is not PrivateRowFieldsSyntaxReceiver receiver)
                return;

            foreach (var group in receiver.PrivateFields.GroupBy<IFieldSymbol, INamedTypeSymbol>(
                x => x.ContainingType,
                SymbolEqualityComparer.Default))
            {
                ProcessRow(group.Key, group, context);
            }

        }

        private void ProcessRow(INamedTypeSymbol rowSymbol, 
            IEnumerable<IFieldSymbol> fields, GeneratorExecutionContext context)
        {
            var ns = rowSymbol.ContainingNamespace.ToDisplayString();
            var fullName = StringHelper.JoinNonEmpty(".", ns, rowSymbol.Name);

            var cw = new CodeWriter(4);
            cw.InNamespace(ns, () =>
            {
                cw.Builder.Append($"partial class {rowSymbol.Name}");
                cw.InBrace(() =>
                {
                });
            });

            context.AddSource(fullName + ".generated", SourceText.From(cw.ToString(), Encoding.UTF8));
        }
    }
}