using Serenity.CodeGenerator;

namespace Serenity.Tests;

public class MockBuildProjectSystem(IFileSystem fileSystem) : IBuildProjectSystem
{
    private readonly IFileSystem fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

    public IBuildProject LoadProject(string path)
    {
        return new MockBuildProject(fileSystem, path);
    }
}