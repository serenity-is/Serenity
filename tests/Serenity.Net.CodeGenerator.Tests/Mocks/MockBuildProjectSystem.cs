using Serenity.CodeGenerator;

namespace Serenity.Tests
{
    public class MockBuildProjectSystem : IBuildProjectSystem
    {
        private readonly IFileSystem fileSystem;

        public MockBuildProjectSystem(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public IBuildProject LoadProject(string path)
        {
            return new MockBuildProject(fileSystem, path);
        }
    }
}