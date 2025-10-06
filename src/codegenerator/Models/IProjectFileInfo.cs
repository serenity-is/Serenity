namespace Serenity.CodeGenerator
{
    public interface IProjectFileInfo
    {
        string ProjectFile { get; }
        IFileSystem FileSystem { get; }

        string[] GetAssemblyList(string[] configured);
        string GetAssemblyName();
        string GetEsmAssetBasePath();
        IDictionary<string, string> GetGlobalUsings();
        string GetOutDir();
        string GetRootNamespace();
        string GetTargetFramework();
    }
}