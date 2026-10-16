namespace Serenity.Web;

public class LayoutScriptBundleScriptTests
{
    private sealed class FakeLayoutScriptFileProvider(IReadOnlyList<string> files) : ILayoutScriptFileProvider
    {
        public IReadOnlyList<string> GetFiles() => files;
    }

    private sealed class FakeBundleBuilder : IScriptBundleBuilder
    {
        public DynamicScript Result { get; set; } = new TextScript("");
        public IReadOnlyList<string>? Files { get; private set; }

        public DynamicScript BuildBundle(IEnumerable<string> sourceFiles)
        {
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
            new LayoutScriptBundleScript(null!, new FakeBundleBuilder()));
        Assert.Throws<ArgumentNullException>(() =>
            new LayoutScriptBundleScript(new FakeLayoutScriptFileProvider([]), null!));
    }

    [Fact]
    public void GetScript_Returns_Empty_When_No_Files()
    {
        var builder = new FakeBundleBuilder();
        var script = new LayoutScriptBundleScript(new FakeLayoutScriptFileProvider([]), builder);

        Assert.Equal("", script.GetScript());
        Assert.Null(builder.Files);
    }

    [Fact]
    public void GetScript_Builds_With_Provided_Files()
    {
        var builder = new FakeBundleBuilder { Result = new TextScript("BUNDLED") };
        var script = new LayoutScriptBundleScript(
            new FakeLayoutScriptFileProvider(["~/a.js", "~/b.js"]), builder);

        Assert.Equal("BUNDLED", script.GetScript());
        Assert.Equal(["~/a.js", "~/b.js"], builder.Files);
    }
}
