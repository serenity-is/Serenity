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
