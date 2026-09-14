namespace Serenity.CodeGenerator;

public partial class TSConfigHelperTests
{
    private static void AddFile(MockFileSystem fileSystem, string path, string content)
    {
        var directory = fileSystem.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
            fileSystem.CreateDirectory(directory);
        fileSystem.WriteAllText(path, content);
    }

    private static bool EndsWithPath(string path, string suffix)
    {
        return path.Replace('\\', '/').EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
    }
}
