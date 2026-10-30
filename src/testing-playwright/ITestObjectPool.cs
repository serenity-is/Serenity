namespace Serenity.Testing.Playwright;
    public interface ITestObjectPool<T> where T : class
    {
        Func<T> ItemFactory { get; set; }
        TimeSpan LeaseTimeout { get; set; }
        int MaxSize { get; }

        void Dispose();
        T Lease(CancellationToken token);
        Task<T> LeaseAsync();
        void Return(T item);
    }
