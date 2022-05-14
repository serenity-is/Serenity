using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Serenity.ComponentModel;
using System.Collections.Immutable;
using System.IO;

namespace Serenity.Tests.SourceGenerator;

public static class CSharpSourceGeneratorVerifier<TSourceGenerator>
    where TSourceGenerator : ISourceGenerator, new()
{
    public class Test : CSharpSourceGeneratorTest<TSourceGenerator, XUnitVerifier>
    {
        public Test()
        {
            var asmLocations = new[]
            {
                typeof(int),
                typeof(Uri),
                typeof(Dictionary<string, string>),
                typeof(GeneratedRowAttribute)
            }.Select(x => Path.ChangeExtension(x.Assembly.Location, null)).Distinct().ToArray();

            ReferenceAssemblies = Microsoft.CodeAnalysis.Testing.ReferenceAssemblies.Default.AddAssemblies(
                ImmutableArray.Create(asmLocations));
        }

        protected override CompilationOptions CreateCompilationOptions()
        {
            var compilationOptions = base.CreateCompilationOptions();
            return compilationOptions
                .WithSpecificDiagnosticOptions(
                    compilationOptions.SpecificDiagnosticOptions.SetItems(GetNullableWarningsFromCompiler()));
        }

        protected override Project ApplyCompilationOptions(Project project)
        {
            return base.ApplyCompilationOptions(project);
        }

        public LanguageVersion LanguageVersion { get; set; } = LanguageVersion.Default;

        private static ImmutableDictionary<string, ReportDiagnostic> GetNullableWarningsFromCompiler()
        {
            string[] args = { "/warnaserror:nullable" };
            var commandLineArguments = CSharpCommandLineParser.Default.Parse(args, baseDirectory: Environment.CurrentDirectory, sdkDirectory: Environment.CurrentDirectory);
            var nullableWarnings = commandLineArguments.CompilationOptions.SpecificDiagnosticOptions;

            return nullableWarnings;
        }

        protected override ParseOptions CreateParseOptions()
        {
            return ((CSharpParseOptions)base.CreateParseOptions()).WithLanguageVersion(LanguageVersion);
        }
    }
}