namespace Serenity.Reflection;

public class VisitedGraphTests
{
    [Fact]
    public void ContainsKey_ReturnsTrue_ForNullKey()
    {
        var graph = new VisitedGraph();
        Assert.True(graph.ContainsKey(null));
    }

    [Fact]
    public void ContainsKey_ReturnsFalse_ForMissingKey()
    {
        var graph = new VisitedGraph();
        Assert.False(graph.ContainsKey("missing"));
    }

    [Fact]
    public void ContainsKey_ReturnsTrue_ForPresentKey()
    {
        var graph = new VisitedGraph();
        graph.Add("key", "value");
        Assert.True(graph.ContainsKey("key"));
    }

    [Fact]
    public void Indexer_ReturnsNull_ForNullKey()
    {
        var graph = new VisitedGraph();
        Assert.Null(graph[null]);
    }

    [Fact]
    public void Indexer_ReturnsValue_ForPresentKey()
    {
        var graph = new VisitedGraph();
        graph.Add("key", "value");
        Assert.Equal("value", graph["key"]);
    }
}
