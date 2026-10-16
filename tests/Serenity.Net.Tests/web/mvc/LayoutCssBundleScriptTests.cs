namespace Serenity.Web;

public class LayoutCssBundleScriptTests
{
    private sealed class FakeLayoutCssFileProvider(IReadOnlyList<string> files) : ILayoutCssFileProvider
    {
        public IReadOnlyList<string> GetFiles() => files;
    }

    private sealed class FakeBundleBuilder : ICssBundleBuilder
    {
        public DynamicScript Result { get; set; } = new TextScript("");
        public string? RootPrefix { get; private set; }
        public IReadOnlyList<string>? Files { get; private set; }

        public DynamicScript BuildBundle(string rootPrefix, IEnumerable<string> sourceFiles)
        {
            RootPrefix = rootPrefix;
            Files = sourceFiles.ToList();
            return Result;
        }
    }

    private sealed class TextScript(string text) : DynamicScript
    {
        public override string GetScript() => text;
    }

    [Fact]
    public void Constructor_Throws_For_Null_Arguments()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new LayoutCssBundleScript(null!, new FakeBundleBuilder()));
        Assert.Throws<ArgumentNullException>(() =>
            new LayoutCssBundleScript(new FakeLayoutCssFileProvider([]), null!));
    }

    [Fact]
    public void GetScript_Returns_Empty_When_No_Files()
    {
        var builder = new FakeBundleBuilder();
        var script = new LayoutCssBundleScript(new FakeLayoutCssFileProvider([]), builder);

        Assert.Equal("", script.GetScript());
        Assert.Null(builder.RootPrefix);
    }

    [Fact]
    public void GetScript_Builds_With_Provided_Files()
    {
        var builder = new FakeBundleBuilder { Result = new TextScript("BUNDLED") };
        var script = new LayoutCssBundleScript(
            new FakeLayoutCssFileProvider(["~/a.css", "~/b.css"]), builder);

        Assert.Equal("BUNDLED", script.GetScript());
        Assert.Equal("../", builder.RootPrefix);
        Assert.Equal(["~/a.css", "~/b.css"], builder.Files);
    }
}
