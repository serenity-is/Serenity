using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Serenity.SourceGenerator
{
    [Generator]
    public class RowGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context) 
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            context.AddSource("myGeneratedFile.cs", SourceText.From(@"
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
    }
}