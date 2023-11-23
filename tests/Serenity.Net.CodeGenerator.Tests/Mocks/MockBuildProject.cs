using Serenity.CodeGenerator;
using System.Xml.Linq;

namespace Serenity.Tests;

public class MockBuildProject : IBuildProject
{
    private readonly XElement project;

    public MockBuildProject(IFileSystem fileSystem, string path)
    {
        ArgumentNullException.ThrowIfNull(fileSystem);

        ArgumentNullException.ThrowIfNull(path);

        project = XElement.Load(fileSystem.ReadAllText(path));
    }

    public IEnumerable<IBuildProjectItem> AllEvaluatedItems =>
        project.Elements("ItemGroup").SelectMany(x => x.Elements())
            .Select(x => new MockBuildProjectItem(x));
}