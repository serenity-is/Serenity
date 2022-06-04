using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

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
            if (!(context.SyntaxReceiver is PrivateRowFieldsSyntaxReceiver receiver))
                return;

            foreach (IGrouping<INamedTypeSymbol, IFieldSymbol> group in receiver.PrivateFields.GroupBy(x => x.ContainingType,
                SymbolEqualityComparer.Default))
            {
                string.Join(".", 
                    group.Key.ContainingNamespace.ToDisplayString()
            }

            context.AddSource("myGeneratedFile", SourceText.From(@"
namespace GeneratedNamespace
{
    public class GeneratedClass
    {
        public static void GeneratedMethod()
        {
            // generated code
        }
    }
}", Encoding.UTF8));
        }

        private string ProcessRow(INamedTypeSymbol rowSymbol, 
            List<IFieldSymbol> fields, ISymbol attributeSymbol, ISymbol notifySymbol, GeneratorExecutionContext context)
        {
        }
    }
}