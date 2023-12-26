namespace Serenity.TypeScript;

public class SourceFile : Declaration
{
    public SourceFile()
    {
        Kind = SyntaxKind.SourceFile;
    }

    public NodeArray<IStatement> Statements { get; set; }
    public Token EndOfFileToken { get; set; }
    public string FileName { get; set; }
    public string ModuleName { get; set; }
    public LanguageVariant LanguageVariant { get; set; }
    public bool IsDeclarationFile { get; set; }
    public ScriptKind ScriptKind { get; set; }
    public INode ExternalModuleIndicator { get; set; }
    internal List<Diagnostic> ParseDiagnostics { get; set; }
    public string Text { get; set; }
}