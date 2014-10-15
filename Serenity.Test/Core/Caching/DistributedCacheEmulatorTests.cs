using Serenity.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Serenity.Test
{
    public partial class DistributedCacheEmulatorTests
    {
        [Fact]
        public void DistributedCacheEmulator_Increment_SetsValueToOneIfKeyNotInCache()
        {
            var cache = new DistributedCacheEmulator();
            var actual = cache.Increment("NotExistingValue", 7);
            Assert.Equal(1, actual);

            var inCache = cache.Get<long>("NotExistingValue");
            Assert.Equal(1, inCache);
        }

        [Fact]
        public void DistributedCacheEmulator_Increment_ProperlyIncrementsValueInCache()
        {
            var cache = new DistributedCacheEmulator();
            cache.Set("ExistingValue", 100);

            var actual = cache.Increment("ExistingValue", 3);
            Assert.Equal(103, actual);

            var inCache = cache.Get<long>("ExistingValue");
            Assert.Equal(103, inCache);
        }

        [Fact]
        public void DistributedCacheEmulator_Increment_WorksMultiThreaded()
        {
            var cache = new DistributedCacheEmulator();
            cache.Set("MultiThreadedValue", 12345);
            var values = new HashSet<long>();

            Parallel.ForEach(new int[100], x =>
            {
                var result = cache.Increment("MultiThreadedValue");
                lock (values)
                    values.Add(result);
            });

            Assert.Equal(100, values.Count);
            var sorted = new long[100];
            values.CopyTo(sorted);
            Array.Sort(sorted);
            for (var i = 0; i < sorted.Length; i++)
                Assert.Equal(i + 1 + 12345, sorted[i]);
        }

        [Fact]
        public void DistributedCacheEmulator_Get_ReturnsDefaultIfValueNotInCache()
        {
            var cache = new DistributedCacheEmulator();
            
            var actualInt = cache.Get<int>("NonExistingInt");
            Assert.Equal(0, actualInt);

            var actualStr = cache.Get<string>("NonExistingString");
            Assert.Equal(null, actualStr);

            var actualBool = cache.Get<bool>("NonExistingBoolean");
            Assert.Equal(false, actualBool);
        }

        [Fact]
        public void DistributedCacheEmulator_Get_WorksMultiThreaded()
        {
            var cache = new DistributedCacheEmulator();
            cache.Set("MultiThreadedValue", 12345);
            var values = new HashSet<int>();

            Parallel.ForEach(new int[100], x =>
            {
                var result = cache.Get<int>("MultiThreadedValue");
                lock (values)
                    values.Add(result);
            });

            Assert.Equal(12345, values.First());
        }

        [Fact]
        public void DistributedCacheEmulator_Set_WorksProperly()
        {
            var cache = new DistributedCacheEmulator();

            cache.Set("SomeInt", 13579);
            var actualInt = cache.Get<int>("SomeInt");
            Assert.Equal(13579, actualInt);

            cache.Set("SomeStr", "abc");
            var actualStr = cache.Get<string>("SomeStr");
            Assert.Equal("abc", actualStr);

            cache.Set("SomeBool", true);
            var actualBool = cache.Get<bool>("SomeBool");
            Assert.Equal(true, actualBool);
        }

        [Fact]
        public void DistributedCacheEmulator_Set_WorksMultiThreaded()
        {
            var cache = new DistributedCacheEmulator();

            var threads = new int[100];
            for (var i = 0; i < threads.Length; i++)
                threads[i] = i;

            Parallel.ForEach(threads, x =>
            {
                cache.Set("MultiThreadedValue" + x, x * 7);
            });

            for (var i = 0; i < threads.Length; i++)
            {
                var actual = cache.Get<int>("MultiThreadedValue" + threads[i]);
                Assert.Equal(threads[i] * 7, actual);
            }
        }

        [Fact]
        public void DistributedCacheEmulator_SetWithExpiration_WorksProperly()
        {
            var cache = new DistributedCacheEmulator();

            cache.Set("SomeInt", 13579, DateTime.Now.Add(TimeSpan.FromMilliseconds(100)));
            var actualInt = cache.Get<int>("SomeInt");
            Assert.Equal(13579, actualInt);

            Thread.Sleep(1);

            var notExpiredInt = cache.Get<int>("SomeInt");
            Assert.Equal(13579, notExpiredInt);

            Thread.Sleep(110);

            var expiredInt = cache.Get<int>("SomeInt");
            Assert.Equal(0, expiredInt);
        }

        [Fact]
        public void DistributedCacheEmulator_SetWithExpiration_WorksMultiThreaded()
        {
            var cache = new DistributedCacheEmulator();

            var threads = new int[100];
            for (var i = 0; i < threads.Length; i++)
                threads[i] = i;

            Parallel.ForEach(threads, x =>
            {
                cache.Set("MultiThreadedValue" + x, x * 7, DateTime.Now.Add(TimeSpan.FromMilliseconds(100)));
            });

            Thread.Sleep(1);

            for (var i = 0; i < threads.Length; i++)
            {
                var actual = cache.Get<int>("MultiThreadedValue" + threads[i]);
                Assert.Equal(threads[i] * 7, actual);
            }

            Thread.Sleep(110);

            for (var i = 0; i < threads.Length; i++)
            {
                var actual = cache.Get<int>("MultiThreadedValue" + threads[i]);
                Assert.Equal(0, actual);
            }
        }
    }
}