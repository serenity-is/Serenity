namespace Serenity.CodeGeneration;

public abstract class CodeGeneratorBase
{
    protected List<GeneratedSource> generatedCode;
    protected StringBuilder sb;
    protected CodeWriter cw;

    public CodeGeneratorBase()
    {
        sb = new StringBuilder(4096);
        cw = new CodeWriter(sb, 4);
    }

    public bool FileScopedNamespaces
    {
        get => cw.FileScopedNamespaces;
        set => cw.FileScopedNamespaces = value;
    }

    protected virtual void Reset()
    {
        sb.Clear();
        generatedCode = new();
    }

    protected virtual void AddFile(string filename, bool module = false)
    {
        var text = cw.ToString();
        generatedCode.Add(new GeneratedSource(filename, text, module));
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