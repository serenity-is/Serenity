namespace Serenity.Web.SpaServices;

public class EventedStreamReaderTests
{
    [Fact]
    public void Constructor_Throws_For_Null()
    {
        Assert.Throws<ArgumentNullException>(() => new EventedStreamReader(null!));
    }

    [Fact]
    public async Task WaitForMatch_Resolves_On_Matching_Line()
    {
        var stream = new BlockingStream();
        var reader = new EventedStreamReader(new System.IO.StreamReader(stream));
        var matchTask = reader.WaitForMatch(new Regex("matching"));
        stream.Write("first\nmatching line\n");
        stream.Complete();

        var match = await matchTask;

        Assert.True(match.Success);
    }

    [Fact]
    public async Task WaitForMatch_Throws_On_Stream_Close_Without_Match()
    {
        var stream = new BlockingStream();
        var reader = new EventedStreamReader(new System.IO.StreamReader(stream));
        var matchTask = reader.WaitForMatch(new Regex("notfound"));
        stream.Write("first\nsecond\n");
        stream.Complete();

        await Assert.ThrowsAsync<System.IO.EndOfStreamException>(() => matchTask);
    }

    [Fact]
    public async Task Raises_Chunk_Line_And_Closed_Events()
    {
        var stream = new BlockingStream();
        var reader = new EventedStreamReader(new System.IO.StreamReader(stream));
        var lines = new List<string>();
        var chunks = 0;
        var closed = new TaskCompletionSource();
        reader.OnReceivedLine += line => lines.Add(line);
        reader.OnReceivedChunk += _ => chunks++;
        reader.OnStreamClosed += () => closed.TrySetResult();

        stream.Write("line1\nli");
        stream.Complete();
        await closed.Task;

        Assert.Contains("line1\n", lines);
        Assert.True(chunks > 0);
    }
}
