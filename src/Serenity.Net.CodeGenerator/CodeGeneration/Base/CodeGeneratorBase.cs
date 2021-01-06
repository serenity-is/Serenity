using Serenity.Reflection;
using System.Collections.Generic;
using System.Text;

namespace Serenity.CodeGeneration
{
    public abstract class CodeGeneratorBase
    {
        private SortedDictionary<string, string> generatedCode;
        protected StringBuilder sb;
        protected CodeWriter cw;

        public CodeGeneratorBase()
        {
        }

        protected virtual void Reset()
        {
            sb = new StringBuilder(4096);
            cw = new CodeWriter(sb, 4);
            generatedCode = new SortedDictionary<string, string>();
        }

        protected void AddFile(string filename)
        {
            generatedCode[filename] = sb.ToString();
            sb.Clear();
        }

        protected abstract void GenerateAll();

        public SortedDictionary<string, string> Run()
        {
            Reset();
            GenerateAll();
            return generatedCode;
        }
    }
}