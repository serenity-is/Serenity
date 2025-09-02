namespace Serenity.CodeGenerator;

public class TSConfig
{
    public CompilerConfig CompilerOptions { get; set; }
    public string Extends { get; set; }
    public string[] Files { get; set; }
    public string[] Include { get; set; }
    public string[] Exclude { get; set; }
    public string RootDir { get; set; }

    public class CompilerConfig
    {
        public string BaseUrl { get; set; }
        public string Module { get; set; }
        public string[] TypeRoots { get; set; }
        public string[] Types { get; set; }
        public Dictionary<string, string[]> Paths { get; set; }
    }
}