using System.Threading;

namespace Serenity.TestUtils;

/// <summary>
/// A stream that blocks reads until data is written, so event subscriptions
/// can be registered before the reader reaches the end of stream.
/// </summary>
public sealed class BlockingStream : System.IO.Stream
{
    private readonly Queue<byte[]> chunks = new();
    private readonly SemaphoreSlim available = new(0);
    private readonly object gate = new();
    private bool completed;
    private byte[]? current;
    private int currentOffset;

    public void Write(string text)
    {
        lock (gate)
            chunks.Enqueue(Encoding.UTF8.GetBytes(text));
        available.Release();
    }

    public void Complete()
    {
        lock (gate)
            completed = true;
        available.Release();
    }

    private int ReadSync(byte[] buffer, int offset, int count)
    {
        while (true)
        {
            if (TryCopyCurrent(buffer, offset, count, out var copied))
                return copied;

            lock (gate)
            {
                if (chunks.Count > 0)
                {
                    current = chunks.Dequeue();
                    currentOffset = 0;
                    continue;
                }

                if (completed)
                    return 0;
            }

            available.Wait();
        }
    }

    private bool TryCopyCurrent(byte[] buffer, int offset, int count, out int copied)
    {
        if (current != null && currentOffset < current.Length)
        {
            copied = Math.Min(count, current.Length - currentOffset);
            Buffer.BlockCopy(current, currentOffset, buffer, offset, copied);
            currentOffset += copied;
            if (currentOffset >= current.Length)
                current = null;
            return true;
        }

        copied = 0;
        return false;
    }

    public override int Read(byte[] buffer, int offset, int count) => ReadSync(buffer, offset, count);

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count,
        CancellationToken cancellationToken)
    {
        while (true)
        {
            if (TryCopyCurrent(buffer, offset, count, out var copied))
                return copied;

            await available.WaitAsync(cancellationToken).ConfigureAwait(false);

            lock (gate)
            {
                if (chunks.Count > 0)
                {
                    current = chunks.Dequeue();
                    currentOffset = 0;
                    continue;
                }

                if (completed)
                    return 0;
            }
        }
    }

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();
    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override void Flush()
    {
    }

    public override long Seek(long offset, System.IO.SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}
