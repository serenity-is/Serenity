namespace Serenity.CodeGenerator;

public abstract class BaseTemplateTest
{
    protected abstract string TemplateName { get; }

    protected Templates templates = new();

    protected string RenderTemplate(EntityModel model)
    {
        return templates.Render(new MockFileSystem(), TemplateName, model);
    }

    protected void AssertEqual(string expected, string actual)
    {
        expected = expected?.Replace("\r", "");
        actual = actual?.Replace("\r", "");
        Assert.Equal(expected, actual);
    }
}
