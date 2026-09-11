namespace Serenity.Web;

public class FileWatcherTests
{
    [Fact]
    public void Constructor_Throws_When_Path_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(() => new FileWatcher(null, "*.txt"));
    }

    [Fact]
    public void Constructor_Throws_When_Filter_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(() => new FileWatcher("some/path", null));
    }

    [Fact]
    public void Constructor_Does_Not_Create_Watcher_For_NonExisting_Directory()
    {
        var watcher = new FileWatcher("/non/existing/path/that/should/never/exist", "*.txt");
        Assert.Equal("/non/existing/path/that/should/never/exist", watcher.Path);
        Assert.Equal("*.txt", watcher.Filter);
    }

    [Fact]
    public void RaiseChanged_Raises_Changed_Event()
    {
        var watcher = new FileWatcher("/non/existing/path", "*.txt");
        string? raised = null;
        watcher.Changed += name => raised = name;

        watcher.RaiseChanged("test.txt");

        Assert.Equal("test.txt", raised);
    }

    [Fact]
    public void Changed_Event_Can_Be_Removed()
    {
        var watcher = new FileWatcher("/non/existing/path", "*.txt");
        int calls = 0;
        Action<string> handler = _ => calls++;

        watcher.Changed += handler;
        watcher.Changed -= handler;
        watcher.RaiseChanged("test.txt");

        Assert.Equal(0, calls);
    }

    [Fact]
    public void RaiseChanged_Does_Nothing_Without_Handlers()
    {
        var watcher = new FileWatcher("/non/existing/path", "*.txt");
        watcher.RaiseChanged("test.txt");
    }

    [Fact]
    public void Constructor_Creates_Watcher_For_Existing_Directory_And_Can_Dispose()
    {
        var path = System.IO.Path.GetTempPath();
        var watcher = new FileWatcher(path, "*.txt");

        Assert.Equal(path, watcher.Path);
        Assert.Equal("*.txt", watcher.Filter);

        ((IDisposable)watcher).Dispose();
        ((IDisposable)watcher).Dispose();
    }
}
