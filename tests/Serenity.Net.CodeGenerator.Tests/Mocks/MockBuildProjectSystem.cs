using Serenity.CodeGenerator;

namespace Serenity.Tests;

public class MockBuildProjectSystem(IGeneratorFileSystem fileSystem) : IBuildProjectSystem
{
    private readonly IGeneratorFileSystem fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

    public IBuildProject LoadProject(string path)
    {
        return new MockBuildProject(fileSystem, path);
    }
}