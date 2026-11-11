namespace Serenity.CodeGenerator
{
    public interface IProjectFileInfo
    {
        string ProjectFile { get; }
        IFileSystem FileSystem { get; }

        string[]? GetAssemblyList(string[]? configured);
        string? GetAssemblyName();
        string? GetDotNetSergen();
        string? GetEsmAssetBasePath();
        IDictionary<string, string?> GetGlobalUsings();
        string? GetNullable();
        string? GetOutDir();
        string? GetRootNamespace();
        string? GetTargetFramework();
        bool UsesSourceGenerators();
    }
}