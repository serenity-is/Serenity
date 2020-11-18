using System;

namespace Serenity.Data
{
    public class RowFieldsProvider
    {
        private static readonly object sync = new object();
        private static IRowFieldsProvider current;

        static RowFieldsProvider()
        {
            current = new DefaultRowFieldsProvider();
        }

        /// <summary>
        /// Gets current row fields provider. This instance is required
        /// as rows might have to be created in contexts where dependency injection
        /// is not possible, like deserialization.
        /// </summary>
        public static IRowFieldsProvider Current
        {
            get
            {
                lock (sync)
                    return current;
            }
        }

        /// Sets current row fields provider. This instance is required
        /// as rows might have to be created in contexts where dependency injection
        /// is not possible, like deserialization. If using a DI container,
        /// set this at startup to the same singleton service you register with DI.
        public static IRowFieldsProvider SetCurrent(IRowFieldsProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            lock (sync)
            {
                var old = current;
                current = provider;
                return old;
            }
        }

        /// <summary>
        /// Runs operation in locked context, please use only for tests
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="factory"></param>
        public static void TestScope(Action operation, IRowFieldsProvider factory)
        {
            lock (sync)
            {
                var old = SetCurrent(factory);
                try
                {
                    operation();
                }
                finally
                {
                    SetCurrent(old);
                }
            }
        }
    }
}