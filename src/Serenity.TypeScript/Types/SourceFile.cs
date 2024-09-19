namespace Serenity.TypeScript;

public class SourceFile : Node, IDeclaration, IBlockLike, IGetRestChildren
{
    public SourceFile()
        : base(SyntaxKind.SourceFile)
    {
    }

    internal SourceFile(string sourceText, string fileName, ScriptTarget languageVersion, ScriptKind scriptKind, bool isDeclarationFile,
        NodeArray<IStatement> statements, EndOfFileToken endOfFileToken, NodeFlags flags) : this()
    {
        // code from createNode is inlined here so createNode won't have to deal with special case of creating source files
        // this is quite rare comparing to other nodes and createNode should be as fast as possible
        Statements = statements;
        EndOfFileToken = endOfFileToken;
        Flags = flags;
        Utilities.SetTextRangePosWidth(this, 0, sourceText.Length);

        Text = sourceText;
        LanguageVersion = languageVersion;
        FileName = fileName;
        LanguageVariant = Utilities.GetLanguageVariant(scriptKind);
        IsDeclarationFile = isDeclarationFile;
        ScriptKind = scriptKind;
    }

    public NodeArray<IStatement> Statements { get; set; }
    public Token EndOfFileToken { get; set; }
    public string FileName { get; set; }
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
    public JSDocParsingMode JsDocParsingMode { get; set; }

    public IEnumerable<INode> GetRestChildren()
    {
        if (Statements != null) foreach (var x in Statements) yield return x;
        yield return EndOfFileToken;
    }
}