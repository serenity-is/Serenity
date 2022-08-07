namespace Serenity.CodeGeneration
{
    public abstract class CodeGeneratorBase
    {
        private SortedDictionary<string, GeneratedSource> generatedCode;
        protected StringBuilder sb;
        protected CodeWriter cw;

        public CodeGeneratorBase()
        {
        }

        protected virtual void Reset()
        {
            sb = new StringBuilder(4096);
            cw = new CodeWriter(sb, 4);
            generatedCode = new SortedDictionary<string, GeneratedSource>();
        }

        protected virtual void AddFile(string filename, bool module = false)
        {
            var text = sb.ToString();
            generatedCode[filename] = new GeneratedSource(text, module);
            sb.Clear();
        }

        protected abstract void GenerateAll();

        public SortedDictionary<string, GeneratedSource> Run()
        {
            Reset();
            GenerateAll();
            return generatedCode;
        }
    }
}