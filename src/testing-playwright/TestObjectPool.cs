namespace Serenity.Testing.Playwright;

using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

public class TestObjectPool<T> : IDisposable, ITestObjectPool<T> where T : class
{
    private readonly ConcurrentQueue<T> pool = new();
    private readonly ConcurrentDictionary<T, byte> leased = new();
    private readonly Lock sync = new();
    private SemaphoreSlim semaphore;
    private volatile bool disposed;
    private int totalItems;

    public int MaxSize { get; set; } = int.MaxValue;
    public TimeSpan LeaseTimeout { get; set; } = TimeSpan.Zero;
    public Func<T> ItemFactory { get; set; } = () => Activator.CreateInstance<T>();

    public void Dispose()
    {
        if (disposed) return;
        disposed = true;
        semaphore?.Dispose();
        foreach (var item in pool)
        {
            if (item is IDisposable d) d.Dispose();
        }
        pool.Clear();
        foreach (var kvp in leased)
        {
            if (kvp.Key is IDisposable d) d.Dispose();
        }
        leased.Clear();
        GC.SuppressFinalize(this);
    }

    public T Lease(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(disposed, typeof(TestObjectPool<>));

        semaphore ??= new SemaphoreSlim(0, MaxSize);

        lock (sync)
        {
            if (totalItems < MaxSize)
            {
                totalItems++;
                var item = ItemFactory();
                leased.TryAdd(item, 0);
                return item;
            }
        }

        try
        {
            if (LeaseTimeout == TimeSpan.Zero)
                semaphore.Wait(cancellationToken);
            else if (!semaphore.Wait(LeaseTimeout, cancellationToken))
                throw new TimeoutException("Lease operation timed out.");

            if (!pool.TryDequeue(out var item))
                throw new InvalidOperationException("Pool is empty but semaphore allowed access.");

            leased.TryAdd(item, 0);
            return item;
        }
        catch (OperationCanceledException)
        {
            throw new TimeoutException("Lease operation timed out.");
        }
    }

    public async Task<T> LeaseAsync()
    {
        ObjectDisposedException.ThrowIf(disposed, typeof(TestObjectPool<>));

        semaphore ??= new SemaphoreSlim(0, MaxSize);

        lock (sync)
        {
            if (totalItems < MaxSize)
            {
                totalItems++;
                var item = ItemFactory();
                leased.TryAdd(item, 0);
                return item;
            }
        }

        try
        {
            if (LeaseTimeout == TimeSpan.Zero)
            {
                await semaphore.WaitAsync();
            }
            else
            {
                using var cts = new CancellationTokenSource(LeaseTimeout);
                semaphore.Wait(cts.Token);
            }
            if (!pool.TryDequeue(out var item))
            {
                throw new InvalidOperationException("Pool is empty but semaphore allowed access.");
            }
            leased.TryAdd(item, 0);
            return item;
        }
        catch (OperationCanceledException)
        {
            throw new TimeoutException("Lease operation timed out.");
        }
    }

    public void Return(T item)
    {
        if (disposed || item == null || !leased.TryRemove(item, out _)) return;
        pool.Enqueue(item);
        semaphore?.Release();
    }
}