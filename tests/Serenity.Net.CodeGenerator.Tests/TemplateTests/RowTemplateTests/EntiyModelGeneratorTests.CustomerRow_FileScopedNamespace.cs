using System.Text.RegularExpressions;

namespace Serenity.Tests.CodeGenerator;

public partial class RowTemplateTests
{
    [Fact]
    public void CustomerRow_FileScopedNamespace()
    {
        var model = new MockEntityModel
        {
            FileScopedNamespaces = true
        };
        var actual = RenderTemplate(model);
        
        var expected = Regex.Replace(ExpectedDefaultTestRowCS,
            @"^namespace TestNamespace\.TestModule",
            "namespace TestNamespace.TestModule;\n",
            RegexOptions.Multiline);

        expected = Regex.Replace(expected,
            @"^([{}]\r?\n?|    )", "", RegexOptions.Multiline).TrimEnd();

        AssertEqual(expected, actual);
        Assert.Contains("namespace TestNamespace.TestModule;", actual);
    }
}
