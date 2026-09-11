namespace Serenity.Web;

public class DefaultFileWatcherFactoryTests
{
    [Fact]
    public void Create_Returns_FileWatcher_With_Path_And_Filter()
    {
        var factory = new DefaultFileWatcherFactory();
        var watcher = factory.Create("/some/path", "*.txt");

        Assert.IsType<FileWatcher>(watcher);
        Assert.Equal("/some/path", watcher.Path);
        Assert.Equal("*.txt", watcher.Filter);
    }

    [Fact]
    public void KeepAlive_Adds_Watcher_Only_Once()
    {
        var factory = new DefaultFileWatcherFactory();
        var watcher = factory.Create("/some/path", "*.txt");

        factory.KeepAlive(watcher);
        factory.KeepAlive(watcher);

        Assert.Single(factory.Watchers);
        Assert.Contains(watcher, factory.Watchers);
    }

    [Fact]
    public void Watchers_Is_Empty_Initially()
    {
        Assert.Empty(new DefaultFileWatcherFactory().Watchers);
    }

    [Fact]
    public void KeepAlive_Keeps_Different_Watchers()
    {
        var factory = new DefaultFileWatcherFactory();
        var first = factory.Create("/first", "*.txt");
        var second = factory.Create("/second", "*.txt");

        factory.KeepAlive(first);
        factory.KeepAlive(second);

        Assert.Equal(2, factory.Watchers.Count());
    }
}
