using Serenity.CodeGenerator;

namespace Serenity.CodeGenerator;

public abstract class BaseTemplateTest
{
    protected abstract string TemplateName { get; }

    protected string RenderTemplate(EntityModel model)
    {
        return Templates.Render(new MockFileSystem(), TemplateName, model);
    }

    protected void AssertEqual(string expected, string actual)
    {
        expected = expected?.Replace("\r", "");
        actual = actual?.Replace("\r", "");
        Assert.Equal(expected, actual);
    }
}
