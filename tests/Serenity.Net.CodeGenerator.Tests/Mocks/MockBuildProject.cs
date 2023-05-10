using Serenity.CodeGenerator;
using System.Xml.Linq;

namespace Serenity.Tests;

public class MockBuildProject : IBuildProject
{
    private readonly XElement project;

    public MockBuildProject(IGeneratorFileSystem fileSystem, string path)
    {
        if (fileSystem == null)
            throw new ArgumentNullException(nameof(fileSystem));

        if (path == null)
            throw new ArgumentNullException(nameof(path));

        project = XElement.Load(fileSystem.ReadAllText(path));
    }

    public IEnumerable<IBuildProjectItem> AllEvaluatedItems =>
        project.Elements("ItemGroup").SelectMany(x => x.Elements())
            .Select(x => new MockBuildProjectItem(x));
}