namespace Serenity.CodeGenerator
{
    public interface IProjectFileInfo
    {
        string ProjectFile { get; }
        IFileSystem FileSystem { get; }

        string GetAssemblyName();
        string GetOutDir();
        string GetRootNamespace();
        string GetTargetFramework();
    }
}