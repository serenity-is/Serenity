namespace Serenity.TypeScript;

internal interface ITypeScriptAST
{
    string SourceStr { get; set; }
    INode RootNode { get; set; }
    void MakeAST(string source, string fileName = "fileName.ts", bool setChildren = true);
}

internal class TypeScriptAST : ITypeScriptAST
{
    public string SourceStr { get; set; }
    public INode RootNode { get; set; }

    public TypeScriptAST(string source = null, string fileName = "fileName.ts", bool setChildren = true, bool optimized = false)
    {
        if (source != null)
        {
            MakeAST(source, fileName, setChildren, optimized);
        }
    }

    public void MakeAST(string source, string fileName = "fileName.ts", bool setChildren = true)
    {
        MakeAST(source, fileName, setChildren, false);
    }

    public void MakeAST(string source, string fileName = "fileName.ts", bool setParentNodes = true, bool optimized = false)
    {
        SourceStr = source;
        
        var parser = new Parser
        {
            Optimized = optimized
        };

        RootNode = parser.ParseSourceFile(fileName, source, ScriptTarget.Latest, 
            syntaxCursor: null, setParentNodes);
    }
}