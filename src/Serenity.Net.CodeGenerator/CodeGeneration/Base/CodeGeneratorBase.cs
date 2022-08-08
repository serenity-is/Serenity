namespace Serenity.CodeGeneration
{
    public abstract class CodeGeneratorBase
    {
        protected List<GeneratedSource> generatedCode;
        protected StringBuilder sb;
        protected CodeWriter cw;

        public CodeGeneratorBase()
        {
        }

        protected virtual void Reset()
        {
            sb = new StringBuilder(4096);
            cw = new CodeWriter(sb, 4);
            generatedCode = new();
        }

        protected virtual void AddFile(string filename, bool module = false)
        {
            var text = sb.ToString();
            generatedCode.Add(new GeneratedSource(filename, text, module));
            sb.Clear();
        }

        protected abstract void GenerateAll();

        public List<GeneratedSource> Run()
        {
            Reset();
            GenerateAll();
            return generatedCode;
        }
    }
}