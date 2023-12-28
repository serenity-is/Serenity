namespace Serenity.TypeScript;

internal class SourceFile : Declaration
{
    public SourceFile() 
        : base(SyntaxKind.SourceFile)
    {
    }

    public NodeArray<IStatement> Statements { get; set; }
    public Token EndOfFileToken { get; set; }
    public string FileName { get; set; }
    public string ModuleName { get; set; }

    public LanguageVariant LanguageVariant { get; set; }
    public ScriptTarget LanguageVersion { get; set; }
    public bool IsDeclarationFile { get; set; }
    public ScriptKind ScriptKind { get; set; }
    public INode ExternalModuleIndicator { get; set; }
    internal List<Diagnostic> ParseDiagnostics { get; set; }
    public string Text { get; set; }



    public IEnumerable<CommentDirective> CommentDirectives { get; set; }
    public int NodeCount { get; set; }
    public int IdentifierCount { get; set; }
    public HashSet<string> Identifiers { get; set; }
}