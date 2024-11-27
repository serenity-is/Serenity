namespace Serenity.CodeGeneration;

public abstract class CodeGeneratorBase
{
    protected List<GeneratedSource> generatedCode;
    protected StringBuilder sb;
    protected CodeWriter cw;

    public CodeGeneratorBase()
    {
        sb = new StringBuilder(4096);
        cw = new CodeWriter(sb, 4)
        {
            GlobalUsings = GlobalUsings
        };
    }

    public bool FileScopedNamespaces
    {
        get => cw.FileScopedNamespaces;
        set => cw.FileScopedNamespaces = value;
    }

    public readonly HashSet<string> GlobalUsings = [];

    protected virtual void Reset()
    {
        sb.Clear();
        generatedCode = [];
    }

    protected virtual void AddFile(string filename)
    {
        var text = cw.ToString();
        generatedCode.Add(new GeneratedSource(filename, text));
        sb.Clear();
        cw.LocalUsings?.Clear();
        cw.CurrentNamespace = null;
    }

    protected abstract void GenerateAll();

    public List<GeneratedSource> Run()
    {
        Reset();
        GenerateAll();
        return generatedCode;
    }
}