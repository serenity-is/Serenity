namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    const string root = "/root/";

    static TSModuleResolver CreateResolver(MockFileSystem fileSystem,
        TSConfig tsConfig = null, string tsConfigDir = root)
    {
        return new TSModuleResolver(fileSystem, tsConfigDir, tsConfig);
    }

    static string FullPath(MockFileSystem fileSystem, string path)
    {
        return PathHelper.ToPath(fileSystem.GetFullPath(path));
    }
}
