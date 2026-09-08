namespace Serenity;

public class CollectionExtensionsTests
{
    [Fact]
    public void AddRange_Params_AddsAllItems()
    {
        ICollection<int> list = new List<int>();
        list.AddRange(1, 2, 3);
        Assert.Equal([1, 2, 3], list);
    }

    [Fact]
    public void AddRange_Params_Throws_WhenListIsNull()
    {
        ICollection<int> list = null;
        Assert.Throws<ArgumentException>(() => list.AddRange(1, 2));
    }

    [Fact]
    public void AddRange_Enumerable_AddsAllItems()
    {
        ICollection<int> list = new List<int>();
        list.AddRange(new List<int> { 4, 5, 6 });
        Assert.Equal([4, 5, 6], list);
    }

    [Fact]
    public void AddRange_Enumerable_Throws_WhenListIsNull()
    {
        ICollection<int> list = null;
        Assert.Throws<ArgumentException>(() => list.AddRange(new[] { 1 }));
    }

    [Fact]
    public void Get_ReturnsValue_WhenKeyExists()
    {
        var dict = new Dictionary<string, int> { ["a"] = 1 };
        Assert.Equal(1, dict.Get("a"));
    }

    [Fact]
    public void Get_ReturnsDefault_WhenKeyMissing()
    {
        var dict = new Dictionary<string, int> { ["a"] = 1 };
        Assert.Equal(0, dict.Get("b"));
    }

    [Fact]
    public void Get_Throws_WhenDictionaryIsNull()
    {
        Dictionary<string, int> dict = null;
        Assert.Throws<ArgumentException>(() => dict.Get("a"));
    }

    [Fact]
    public void Get_WithDefault_ReturnsValue_WhenKeyExists()
    {
        var dict = new Dictionary<string, int> { ["a"] = 1 };
        Assert.Equal(1, dict.Get("a", 99));
    }

    [Fact]
    public void Get_WithDefault_ReturnsDefault_WhenKeyMissing()
    {
        var dict = new Dictionary<string, int> { ["a"] = 1 };
        Assert.Equal(99, dict.Get("b", 99));
    }

    [Fact]
    public void Get_WithDefault_Throws_WhenDictionaryIsNull()
    {
        Dictionary<string, int> dict = null;
        Assert.Throws<ArgumentException>(() => dict.Get("a", 99));
    }
}
