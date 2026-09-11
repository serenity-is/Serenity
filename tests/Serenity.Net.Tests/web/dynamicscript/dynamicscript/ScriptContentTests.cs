using System.IO.Compression;

namespace Serenity.Web;

public class ScriptContentTests
{
    [Fact]
    public void Constructor_Throws_When_Content_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ScriptContent(null!, DateTime.UtcNow, CompressionLevel.Optimal));
    }

    [Fact]
    public void Content_And_Time_Are_Set()
    {
        var content = new byte[] { 1, 2, 3 };
        var time = DateTime.UtcNow;
        var script = new ScriptContent(content, time, CompressionLevel.Optimal);

        Assert.Same(content, script.Content);
        Assert.Equal(time, script.Time);
    }

    [Fact]
    public void Hash_Is_Deterministic_And_Cached()
    {
        var script = new ScriptContent([1, 2, 3], DateTime.UtcNow, CompressionLevel.Optimal);

        Assert.Equal(script.Hash, script.Hash);
        Assert.DoesNotContain("+", script.Hash);
        Assert.DoesNotContain("/", script.Hash);
    }

    [Fact]
    public void CanCompress_Is_False_For_NoCompression()
    {
        var script = new ScriptContent([1], DateTime.UtcNow, CompressionLevel.NoCompression);
        Assert.False(script.CanCompress);
    }

    [Fact]
    public void CompressedContent_Throws_When_Compression_Not_Allowed()
    {
        var script = new ScriptContent([1], DateTime.UtcNow, CompressionLevel.NoCompression);
        Assert.Throws<InvalidOperationException>(() => script.CompressedContent);
        Assert.Throws<InvalidOperationException>(() => script.BrotliContent);
    }

    [Fact]
    public void CompressedContent_Returns_Gzip_Bytes_And_Caches()
    {
        var content = System.Text.Encoding.UTF8.GetBytes(new string('a', 1000));
        var script = new ScriptContent(content, DateTime.UtcNow, CompressionLevel.Optimal);

        var first = script.CompressedContent;
        var second = script.CompressedContent;

        Assert.NotNull(first);
        Assert.Same(first, second);
        Assert.True(first.Length > 0);
    }

    [Fact]
    public void BrotliContent_Returns_Bytes_And_Caches()
    {
        var content = System.Text.Encoding.UTF8.GetBytes(new string('a', 1000));
        var script = new ScriptContent(content, DateTime.UtcNow, CompressionLevel.SmallestSize);

        var first = script.BrotliContent;
        var second = script.BrotliContent;

        Assert.NotNull(first);
        Assert.Same(first, second);
        Assert.True(first.Length > 0);
    }

    [Fact]
    public void BrotliContent_With_Fastest_Level()
    {
        var content = System.Text.Encoding.UTF8.GetBytes(new string('a', 1000));
        var script = new ScriptContent(content, DateTime.UtcNow, CompressionLevel.Fastest);

        Assert.True(script.BrotliContent.Length > 0);
        Assert.True(script.CompressedContent.Length > 0);
    }
}
