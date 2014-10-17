using FakeItEasy;
using Serenity.Abstractions;
using Serenity.Testing;
using System;
using System.Collections.Generic;
using Xunit;

namespace Serenity.Test
{
    public partial class DistributedCacheTests
    {
        [Fact]
        public void DistributedCache_Increment_ThrowsExceptionIfNoDistributedCacheProviderIsRegistered()
        {
            using (new MunqContext())
            {
                var exception = Assert.Throws<KeyNotFoundException>(() => DistributedCache.Increment("x", 1));
                Assert.Contains(typeof(IDistributedCache).Name, exception.Message);
            }
        }

        [Fact]
        public void DistributedCache_Increment_UsesRegisteredDistributedCacheProvider()
        {
            using (new MunqContext())
            {
                var registrar = Dependency.Resolve<IDependencyRegistrar>();

                var cache = A.Fake<IDistributedCache>();
                registrar.RegisterInstance(cache);

                A.CallTo(() => cache.Increment("SomeKey", 3)).Returns(7);

                var actual = DistributedCache.Increment("SomeKey", 3);
                Assert.Equal(7, actual);

                A.CallTo(() => cache.Increment("SomeKey", 3))
                    .MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Fact]
        public void DistributedCache_Set_ThrowsExceptionIfNoDistributedCacheProviderIsRegistered()
        {
            using (new MunqContext())
            {
                var exception = Assert.Throws<KeyNotFoundException>(() => DistributedCache.Set("x", 1));
                Assert.Contains(typeof(IDistributedCache).Name, exception.Message);
            }
        }

        [Fact]
        public void DistributedCache_Set_UsesRegisteredDistributedCacheProvider()
        {
            using (new MunqContext())
            {
                var registrar = Dependency.Resolve<IDependencyRegistrar>();

                var cache = A.Fake<IDistributedCache>();
                registrar.RegisterInstance(cache);

                DistributedCache.Set("SomeKey", 789);

                A.CallTo(() => cache.Set("SomeKey", 789))
                    .MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Fact]
        public void DistributedCache_SetWithExpiration_ThrowsExceptionIfNoDistributedCacheProviderIsRegistered()
        {
            using (new MunqContext())
            {
                var exception = Assert.Throws<KeyNotFoundException>(() => DistributedCache.Set("x", 1, TimeSpan.FromSeconds(1)));
                Assert.Contains(typeof(IDistributedCache).Name, exception.Message);
            }
        }

        [Fact]
        public void DistributedCache_SetWithExpiration_UsesRegisteredDistributedCacheProvider()
        {
            using (new MunqContext())
            {
                var registrar = Dependency.Resolve<IDependencyRegistrar>();

                var cache = A.Fake<IDistributedCache>();
                registrar.RegisterInstance(cache);

                var expiration = TimeSpan.FromMinutes(5);

                DistributedCache.Set("SomeKey", 789, expiration);

                A.CallTo(() => cache.Set("SomeKey", 789, expiration))
                    .MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Fact]
        public void DistributedCache_Get_ThrowsExceptionIfNoDistributedCacheProviderIsRegistered()
        {
            using (new MunqContext())
            {
                var exception = Assert.Throws<KeyNotFoundException>(() => DistributedCache.Get<object>("x"));
                Assert.Contains(typeof(IDistributedCache).Name, exception.Message);
            }
        }

        [Fact]
        public void DistributedCache_Get_UsesRegisteredDistributedCacheProvider()
        {
            using (new MunqContext())
            {
                var registrar = Dependency.Resolve<IDependencyRegistrar>();

                var cache = A.Fake<IDistributedCache>();
                registrar.RegisterInstance(cache);

                A.CallTo(() => cache.Get<int>("SomeKey"))
                    .Returns(13579);

                var actual = DistributedCache.Get<int>("SomeKey");
                Assert.Equal(13579, actual);

                A.CallTo(() => cache.Get<int>("SomeKey"))
                    .MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Fact]
        public void DistributedCache_Get_ThrowsExceptionIfValueIsNotOfRequestedType()
        {
            using (new MunqContext())
            {
                var registrar = Dependency.Resolve<IDependencyRegistrar>();

                var cache = A.Fake<IDistributedCache>();
                registrar.RegisterInstance(cache);

                A.CallTo(() => cache.Get<int>("SomeKey"))
                    .Throws<InvalidCastException>();

                Assert.Throws<InvalidCastException>(() => DistributedCache.Get<int>("SomeKey"));
            }
        }
    }
}