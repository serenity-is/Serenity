using FakeItEasy;
using Serenity.Abstractions;
using Serenity.Testing;
using System;
using System.Collections.Generic;
using Xunit;

namespace Serenity.Test
{
    public partial class LocalCacheTests
    {
        [Fact]
        public void LocalCache_AddThrowsExceptionIfNoLocalCacheProviderIsRegistered()
        {
            using (new MunqContext())
            {
                var exception = Assert.Throws<KeyNotFoundException>(() => LocalCache.Add("x", "y", TimeSpan.Zero));
                Assert.Contains(typeof(ILocalCache).Name, exception.Message);
            }
        }

        [Fact]
        public void LocalCache_AddUsesRegisteredLocalCacheProvider()
        {
            using (new MunqContext())
            {
                var registrar = Dependency.Resolve<IDependencyRegistrar>();

                var cache = A.Fake<ILocalCache>();
                registrar.RegisterInstance(cache);

                LocalCache.Add("SomeKey", 1, TimeSpan.FromHours(1));

                A.CallTo(() => cache.Add("SomeKey", 1, TimeSpan.FromHours(1)))
                    .MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Fact]
        public void LocalCache_GetThrowsExceptionIfNoLocalCacheProviderIsRegistered()
        {
            using (new MunqContext())
            {
                var exception = Assert.Throws<KeyNotFoundException>(() => 
                    LocalCache.Get<object>("x", TimeSpan.Zero, () => "a"));

                Assert.Contains(typeof(ILocalCache).Name, exception.Message);
            }
        }

        [Fact]
        public void LocalCache_GetUsesRegisteredLocalCacheProvider()
        {
            using (new MunqContext())
            {
                var registrar = Dependency.Resolve<IDependencyRegistrar>();

                var cache = A.Fake<ILocalCache>();
                registrar.RegisterInstance(cache);

                LocalCache.Get<object>("KeyToGet", TimeSpan.Zero, () => "a");

                A.CallTo(() => cache.Get<object>("KeyToGet"))
                    .MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Fact]
        public void LocalCache_GetReturnsNullIfValueInCacheIsDbNull()
        {
            using (new MunqContext())
            {
                var registrar = Dependency.Resolve<IDependencyRegistrar>();

                var cache = A.Fake<ILocalCache>();
                registrar.RegisterInstance(cache);

                A.CallTo(() => cache.Get<object>("NullKey"))
                    .Returns(DBNull.Value);

                var actual = LocalCache.Get<object>("NullKey", TimeSpan.Zero, () => null);
                Assert.Equal(null, actual);

                A.CallTo(() => cache.Get<object>("NullKey"))
                    .MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Fact]
        public void LocalCache_GetCallsLoaderFunctionIfValueNotInCache()
        {
            using (new MunqContext())
            {
                var registrar = Dependency.Resolve<IDependencyRegistrar>();

                var cache = A.Fake<ILocalCache>();
                registrar.RegisterInstance(cache);

                A.CallTo(() => cache.Get<object>(null))
                    .WithAnyArguments()
                    .Returns(null);

                var actual = LocalCache.Get<string>("NotFound", TimeSpan.FromMinutes(1), () => "LoaderReturn");
                Assert.Equal("LoaderReturn", actual);

                A.CallTo(() => cache.Add("NotFound", "LoaderReturn", TimeSpan.FromMinutes(1)))
                    .MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Fact]
        public void LocalCache_GetAddsDbNullToCacheIfLoaderFunctionReturnsNull()
        {
            using (new MunqContext())
            {
                var registrar = Dependency.Resolve<IDependencyRegistrar>();

                var cache = A.Fake<ILocalCache>();
                registrar.RegisterInstance(cache);

                A.CallTo(() => cache.Get<object>("NotFound"))
                    .Returns(null);

                var actual = LocalCache.Get<string>("NotFound", TimeSpan.FromMinutes(1), () => null);
                Assert.Equal(null, actual);

                A.CallTo(() => cache.Add("NotFound", DBNull.Value, TimeSpan.FromMinutes(1)))
                    .MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Fact]
        public void LocalCache_GetThrowsExceptionIfValueIsNotOfRequestedType()
        {
            using (new MunqContext())
            {
                var registrar = Dependency.Resolve<IDependencyRegistrar>();

                var cache = A.Fake<ILocalCache>();
                registrar.RegisterInstance(cache);

                A.CallTo(() => cache.Get<object>("IntegerValue"))
                    .Returns(1);

                Assert.Throws<InvalidCastException>(() => 
                    LocalCache.Get<string>("Integer", TimeSpan.FromMinutes(1), () => null));
            }
        }

        [Fact]
        public void LocalCache_TryGetThrowsExceptionIfNoLocalCacheProviderIsRegistered()
        {
            using (new MunqContext())
            {
                var exception = Assert.Throws<KeyNotFoundException>(() =>
                    LocalCache.TryGet<object>("x"));

                Assert.Contains(typeof(ILocalCache).Name, exception.Message);
            }
        }

        [Fact]
        public void LocalCache_TryGetUsesRegisteredLocalCacheProvider()
        {
            using (new MunqContext())
            {
                var registrar = Dependency.Resolve<IDependencyRegistrar>();

                var cache = A.Fake<ILocalCache>();
                registrar.RegisterInstance(cache);

                LocalCache.TryGet<string>("KeyToGet");

                A.CallTo(() => cache.Get<object>("KeyToGet"))
                    .MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Fact]
        public void LocalCache_TryGetReturnsNullIfValueNotInCache()
        {
            using (new MunqContext())
            {
                var registrar = Dependency.Resolve<IDependencyRegistrar>();

                var cache = A.Fake<ILocalCache>();
                registrar.RegisterInstance(cache);

                A.CallTo(() => cache.Get<object>("NotInCache"))
                    .Returns(null);

                var actual = LocalCache.TryGet<string>("NotInCache");
                Assert.Equal(null, actual);

                A.CallTo(() => cache.Get<object>("NotInCache"))
                    .MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Fact]
        public void LocalCache_TryGetReturnsDbNullIfValueInCacheIsDbNull()
        {
            using (new MunqContext())
            {
                var registrar = Dependency.Resolve<IDependencyRegistrar>();

                var cache = A.Fake<ILocalCache>();
                registrar.RegisterInstance(cache);

                A.CallTo(() => cache.Get<object>("DbNullValue"))
                    .Returns(DBNull.Value);

                var actual = LocalCache.TryGet<object>("DbNullValue");
                Assert.Equal(DBNull.Value, actual);

                A.CallTo(() => cache.Get<object>("DbNullValue"))
                    .MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Fact]
        public void LocalCache_TryGetReturnsNullIfValueIsNotOfRequestedType()
        {
            using (new MunqContext())
            {
                var registrar = Dependency.Resolve<IDependencyRegistrar>();

                var cache = A.Fake<ILocalCache>();
                registrar.RegisterInstance(cache);

                A.CallTo(() => cache.Get<object>("IntegerValue"))
                    .Returns(1);

                var actual = LocalCache.TryGet<string>("IntegerValue");
                Assert.Equal(null, actual);
            }
        }

        [Fact]
        public void LocalCache_RemoveThrowsExceptionIfNoLocalCacheProviderIsRegistered()
        {
            using (new MunqContext())
            {
                var exception = Assert.Throws<KeyNotFoundException>(() =>
                    LocalCache.Remove("x"));

                Assert.Contains(typeof(ILocalCache).Name, exception.Message);
            }
        }

        [Fact]
        public void LocalCache_RemoveUsesRegisteredLocalCacheProvider()
        {
            using (new MunqContext())
            {
                var registrar = Dependency.Resolve<IDependencyRegistrar>();

                var cache = A.Fake<ILocalCache>();
                registrar.RegisterInstance(cache);

                LocalCache.Remove("KeyToRemove");

                A.CallTo(() => cache.Remove("KeyToRemove"))
                    .MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Fact]
        public void LocalCache_RemoveReturnsRemovedValue()
        {
            using (new MunqContext())
            {
                var registrar = Dependency.Resolve<IDependencyRegistrar>();

                var cache = A.Fake<ILocalCache>();
                registrar.RegisterInstance(cache);

                A.CallTo(() => cache.Remove("OldValue"))
                    .Returns("Old");

                var actual = LocalCache.Remove("OldValue");
                Assert.Equal("Old", actual);

                A.CallTo(() => cache.Remove("OldValue"))
                    .MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Fact]
        public void LocalCache_RemoveReturnsNullIfValueWasntInCache()
        {
            using (new MunqContext())
            {
                var registrar = Dependency.Resolve<IDependencyRegistrar>();

                var cache = A.Fake<ILocalCache>();
                registrar.RegisterInstance(cache);

                A.CallTo(() => cache.Remove("OldValue"))
                    .Returns(null);

                var actual = LocalCache.Remove("OldValue");
                Assert.Equal(null, actual);

                A.CallTo(() => cache.Remove("OldValue"))
                    .MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Fact]
        public void LocalCache_RemoveAllThrowsExceptionIfNoLocalCacheProviderIsRegistered()
        {
            using (new MunqContext())
            {
                var exception = Assert.Throws<KeyNotFoundException>(() =>
                    LocalCache.RemoveAll());

                Assert.Contains(typeof(ILocalCache).Name, exception.Message);
            }
        }

        [Fact]
        public void LocalCache_RemoveAllUsesRegisteredLocalCacheProvider()
        {
            using (new MunqContext())
            {
                var registrar = Dependency.Resolve<IDependencyRegistrar>();

                var cache = A.Fake<ILocalCache>();
                registrar.RegisterInstance(cache);

                LocalCache.RemoveAll();

                A.CallTo(() => cache.RemoveAll())
                    .MustHaveHappened(Repeated.Exactly.Once);
            }
        }
    }
}