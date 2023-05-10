using Serenity.CodeGenerator;

namespace Serenity.Tests.CodeGenerator;

public abstract class BaseTemplateTest
{
    protected abstract string TemplateName { get; }

    protected string RenderTemplate(EntityModel model)
    {
        return Templates.Render(new MockGeneratorFileSystem(), TemplateName, model);
    }

    protected void AssertEqual(string expected, string actual)
    {
        expected = expected?.Replace("\r", "");
        actual = actual?.Replace("\r", "");
        Assert.Equal(expected, actual);
    }
}
