using Serenity.CodeGenerator;
using System.Xml.Linq;

namespace Serenity.Tests;

public class MockBuildProjectItem(XElement projectItem) : IBuildProjectItem
{
    private readonly XElement projectItem = projectItem ?? throw new ArgumentNullException(nameof(projectItem));

    public string EvaluatedInclude => projectItem.Attribute("Include")?.Value ?? "";
    public string GetMetadataValue(string name) => projectItem.Attribute(name)?.Value ?? projectItem.Element(name)?.Value;
    public string ItemType => projectItem.Name?.ToString();
}