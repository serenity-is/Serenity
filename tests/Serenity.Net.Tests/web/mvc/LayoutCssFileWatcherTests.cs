using Microsoft.Extensions.Primitives;
using System.Threading;

namespace Serenity.Web;

public class LayoutCssFileWatcherTests
{
    private sealed class MockWebRootFileWatcher : IWebRootFileWatcher
    {
        public CancellationTokenSource? Current { get; private set; }
        public int Calls { get; private set; }

        public IChangeToken? GetChangeToken(string webRelativePath)
        {
            Calls++;
            Current = new CancellationTokenSource();
            return new CancellationChangeToken(Current.Token);
        }

        public void RaiseChanged() => Current!.Cancel();
    }

    private sealed class NullWebRootFileWatcher : IWebRootFileWatcher
    {
        public IChangeToken? GetChangeToken(string webRelativePath) => null;
    }

    [Fact]
    public void Constructor_Throws_For_Null_Arguments()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new LayoutCssFileWatcher(null!, new MockDynamicScriptManager()));
        Assert.Throws<ArgumentNullException>(() =>
            new LayoutCssFileWatcher(new NullWebRootFileWatcher(), null!));
    }

    [Fact]
    public void Watch_Invalidates_Script_When_File_Changes()
    {
        var fileWatcher = new MockWebRootFileWatcher();
        var scripts = new MockDynamicScriptManager();
        using var watcher = new LayoutCssFileWatcher(fileWatcher, scripts);

        watcher.Watch("LayoutCssFiles.X", ["~/a.css"]);
        fileWatcher.RaiseChanged();

        Assert.Contains("LayoutCssFiles.X", scripts.ChangedNames);
    }

    [Fact]
    public void Watch_Does_Not_Register_Duplicate_For_Same_Name()
    {
        var fileWatcher = new MockWebRootFileWatcher();
        var scripts = new MockDynamicScriptManager();
        using var watcher = new LayoutCssFileWatcher(fileWatcher, scripts);

        watcher.Watch("LayoutCssFiles.X", ["~/a.css"]);
        var callsAfterFirst = fileWatcher.Calls;
        watcher.Watch("LayoutCssFiles.X", ["~/a.css"]);

        Assert.Equal(callsAfterFirst, fileWatcher.Calls);
        Assert.Empty(scripts.ChangedNames);
    }

    [Fact]
    public void Watch_Does_Nothing_When_No_Change_Token()
    {
        var scripts = new MockDynamicScriptManager();
        using var watcher = new LayoutCssFileWatcher(new NullWebRootFileWatcher(), scripts);

        watcher.Watch("LayoutCssFiles.X", ["~/a.css"]);

        Assert.Empty(scripts.ChangedNames);
    }

    [Fact]
    public void Watch_Ignores_Empty_Name_Or_Files()
    {
        var fileWatcher = new MockWebRootFileWatcher();
        var scripts = new MockDynamicScriptManager();
        using var watcher = new LayoutCssFileWatcher(fileWatcher, scripts);

        watcher.Watch("", ["~/a.css"]);
        watcher.Watch("LayoutCssFiles.X", []);
        watcher.Watch("LayoutCssFiles.X", null!);

        Assert.Empty(scripts.ChangedNames);
        Assert.Equal(0, fileWatcher.Calls);
    }
}
