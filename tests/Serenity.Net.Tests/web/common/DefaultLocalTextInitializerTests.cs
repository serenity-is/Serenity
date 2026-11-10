namespace Serenity.Web;

public class DefaultLocalTextInitializerTests
{
    [Fact]
    public void Initialize_Throws_When_Registry_Is_Null()
    {
        var initializer = new DefaultLocalTextInitializer(new MockTypeSource());
        Assert.Throws<ArgumentNullException>(() => initializer.Initialize(null));
    }

    [Fact]
    public void Initialize_Registers_Base_Texts_Without_WebHostEnvironment()
    {
        var registry = new MockLocalTextRegistry();
        var initializer = new DefaultLocalTextInitializer(new MockTypeSource());

        initializer.Initialize(registry);

        Assert.NotNull(registry);
    }

    [Fact]
    public void Initialize_Registers_Json_Texts_From_Content_Root()
    {
        var env = new MockHostEnvironment();
        var textsPath = env.Path.Combine(env.ContentRootPath, "App_Data/texts");
        env.Directory.CreateDirectory(textsPath);
        env.File.WriteAllText(env.Path.Combine(textsPath, "en.json"),
            "{\"My.Text.Key\": \"translated\"}");

        var registry = new MockLocalTextRegistry();
        var initializer = new DefaultLocalTextInitializer(new MockTypeSource(), null, env);

        initializer.Initialize(registry);

        Assert.Contains(registry.AddedList, x => x.key == "My.Text.Key" && x.text == "translated");
    }

    private sealed class RecordingSource(Action<ILocalTextRegistry> add) : IDynamicLocalTextSource
    {
        public int Calls { get; private set; }

        public void AddTexts(ILocalTextRegistry registry)
        {
            Calls++;
            add(registry);
        }
    }

    [Fact]
    public void Initialize_Adds_The_Texts_Of_Dynamic_Sources_After_The_Json_Texts()
    {
        var env = new MockHostEnvironment();
        var textsPath = env.Path.Combine(env.ContentRootPath, "App_Data/texts");
        env.Directory.CreateDirectory(textsPath);
        env.File.WriteAllText(env.Path.Combine(textsPath, "en.json"),
            "{\"My.Text.Key\": \"translated\"}");

        var registry = new MockLocalTextRegistry();
        var seenJsonTexts = false;
        var source = new RecordingSource(r =>
        {
            // what is already added can be told apart from what is missing
            seenJsonTexts = registry.AddedList.Any(x => x.key == "My.Text.Key");
            r.Add("tr", "Navigation.Reports/Sales", "Satis");
        });
        var initializer = new DefaultLocalTextInitializer(new MockTypeSource(), null, env, [source]);

        initializer.Initialize(registry);

        Assert.True(seenJsonTexts);
        Assert.Equal(1, source.Calls);
        Assert.Contains(registry.AddedList, x => x.key == "Navigation.Reports/Sales" && x.text == "Satis");
    }

    [Fact]
    public void Initialize_Adds_The_Texts_Of_Dynamic_Sources_Without_WebHostEnvironment()
    {
        var registry = new MockLocalTextRegistry();
        var first = new RecordingSource(r => r.Add("", "A.Key", "A"));
        var second = new RecordingSource(r => r.Add("", "B.Key", "B"));

        new DefaultLocalTextInitializer(new MockTypeSource(), dynamicSources: [first, second]).Initialize(registry);

        Assert.Contains(registry.AddedList, x => x.key == "A.Key");
        Assert.Contains(registry.AddedList, x => x.key == "B.Key");
    }

    [Fact]
    public void AddJsonTexts_Skips_When_ContentRootFileProvider_Is_Null()
    {
        var env = new MockHostEnvironment
        {
            ContentRootFileProvider = null!
        };
        var registry = new MockLocalTextRegistry();
        var initializer = new DefaultLocalTextInitializer(new MockTypeSource(), null, env);

        initializer.Initialize(registry);

        Assert.DoesNotContain(registry.AddedList, x => x.key == "My.Text.Key");
    }
}
