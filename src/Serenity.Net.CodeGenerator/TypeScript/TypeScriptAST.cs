using Serenity.TypeScript.TsParser;
using Serenity.TypeScript.TsTypes;

namespace Serenity.TypeScript;

public interface ITypeScriptAST
{
    string SourceStr { get; set; }
    Node RootNode { get; set; }
    void MakeAST(string source, string fileName = "fileName.ts", bool setChildren = true);
}
public class TypeScriptAST: ITypeScriptAST
{
    public string SourceStr { get; set; }
    public Node RootNode { get; set; }

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

    public void MakeAST(string source, string fileName = "fileName.ts", bool setChildren = true, bool optimized = false)
    {
        SourceStr = source;
        var parser = new Parser
        {
            Optimized = optimized
        };
        var sourceFile = parser.ParseSourceFile(fileName, source, null, false, ScriptKind.Ts);
        RootNode = sourceFile;
        RootNode.SourceStr = SourceStr;
        if (setChildren)
        {
            if (optimized)
                RootNode.MakeChildrenOptimized(SourceStr);
            else
                RootNode.MakeChildren(SourceStr);
        }
    }
}