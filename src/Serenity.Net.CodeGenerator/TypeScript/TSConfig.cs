namespace Serenity.CodeGenerator
{
    public class TSConfig
    {
        public CompilerConfig CompilerOptions { get; set; }
        public string[] Files { get; set; }
        public string[] Include { get; set; }
        public string[] Exclude { get; set; }

        public class CompilerConfig
        {
            public string Module { get; set; }
            public string[] TypeRoots { get; set; }
            public string[] Types { get; set; }
        }
    }
}