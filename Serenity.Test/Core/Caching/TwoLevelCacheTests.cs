using FakeItEasy;
using Serenity.Abstractions;
using Serenity.Caching;
using Serenity.Testing;
using System;
using System.Collections.Generic;
using Xunit;

namespace Serenity.Test
{
    [Collection("AvoidParallel")]
    public partial class TwoLevelCacheTests
    {
        [Fact]
        public void TwoLevelCache_Get_ThrowsExceptionIfNoLocalCacheProviderIsRegistered()
        {
            using (new MunqContext())
            {
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance<IDistributedCache>(new DistributedCacheEmulator());

                var exception = Assert.Throws<KeyNotFoundException>(() => 
                    TwoLevelCache.Get("x", TimeSpan.Zero, "gen", () => "a"));

                Assert.Contains(typeof(ILocalCache).Name, exception.Message);
            }
        }

        [Fact]
        public void TwoLevelCache_Get_ThrowsExceptionIfNoDistributedCacheProviderIsRegistered()
        {
            using (new MunqContext())
            {
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance<ILocalCache>(new HttpRuntimeCache());

                var exception = Assert.Throws<KeyNotFoundException>(() =>
                    TwoLevelCache.Get("x", TimeSpan.Zero, "gen", () => "a"));

                Assert.Contains(typeof(IDistributedCache).Name, exception.Message);
            }
        }

        [Fact]
        public void TwoLevelCache_Get_UsesRegisteredCacheProviders()
        {
            using (new MunqContext())
            {
                var localCache = A.Fake<ILocalCache>();
                var distributedCache = A.Fake<IDistributedCache>();
                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                registrar.RegisterInstance<ILocalCache>(localCache);
                registrar.RegisterInstance<IDistributedCache>(distributedCache);

                ulong? generationKey = null;

                A.CallTo(() => localCache.Get<object>("SomeKey"))
                    .Returns(null);

                A.CallTo(() => distributedCache.Set<ulong>("SomeGenerationKey", A<ulong>.Ignored))
                    .Invokes((string x, ulong y) =>
                    {
                        generationKey = y;
                    });

                var value = TwoLevelCache.Get("SomeKey", TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(30),
                    "SomeGenerationKey", () => "xyz");

                Assert.Equal("xyz", value);
                Assert.NotNull(generationKey);

                A.CallTo(() => localCache.Get<object>("SomeKey"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Add("SomeKey", "xyz", TimeSpan.FromMinutes(5)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Add("SomeKey" + TwoLevelCache.GenerationSuffix, generationKey.Value, TimeSpan.FromMinutes(5)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Add("SomeGenerationKey", generationKey.Value, TwoLevelCache.GenerationCacheExpiration))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Set("SomeGenerationKey", generationKey.Value))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Set("SomeKey", "xyz", TimeSpan.FromMinutes(30)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Set("SomeKey" + TwoLevelCache.GenerationSuffix, generationKey.Value, TimeSpan.FromMinutes(30)))
                    .MustHaveHappened(1, Times.Exactly);
            }
        }

        [Fact]
        public void TwoLevelCache_Get_GetsGenerationFromDistributedCacheIfLocalCachedGenerationDoesntExist()
        {
            using (new MunqContext())
            {
                var localCache = A.Fake<ILocalCache>();
                var distributedCache = A.Fake<IDistributedCache>();

                A.CallTo(() => localCache.Get<object>("SomeKey"))
                    .Returns("LocalValue");

                A.CallTo(() => localCache.Get<object>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .Returns(987UL);

                A.CallTo(() => localCache.Get<object>("SomeGenerationKey"))
                    .Returns(null);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeGenerationKey"))
                     .Returns(987UL);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeKey" + TwoLevelCache.GenerationSuffix))
                     .Returns(null);

                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                registrar.RegisterInstance<ILocalCache>(localCache);
                registrar.RegisterInstance<IDistributedCache>(distributedCache);

                var value = TwoLevelCache.Get("SomeKey", TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(30),
                    "SomeGenerationKey", () => "AnotherValue");

                Assert.Equal("LocalValue", value);

                A.CallTo(() => localCache.Get<object>("SomeKey"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Get<object>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Get<object>("SomeGenerationKey"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Add(A<String>.Ignored, A<String>.Ignored, A<TimeSpan>.Ignored))
                    .MustNotHaveHappened();

                A.CallTo(() => localCache.Add("SomeKey" + TwoLevelCache.GenerationSuffix, A<ulong>.Ignored, A<TimeSpan>.Ignored))
                    .MustNotHaveHappened();

                A.CallTo(() => localCache.Add("SomeGenerationKey", 987UL, TwoLevelCache.GenerationCacheExpiration))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeGenerationKey"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Get<string>(A<string>.Ignored))
                    .MustNotHaveHappened();

                A.CallTo(() => distributedCache.Set<ulong>(A<string>.Ignored, A<ulong>.Ignored))
                    .MustNotHaveHappened();

                A.CallTo(() => distributedCache.Set<ulong>(A<string>.Ignored, A<ulong>.Ignored, A<TimeSpan>.Ignored))
                    .MustNotHaveHappened();
            }
        }

        [Fact]
        public void TwoLevelCache_Get_ReturnsLocalCachedItemIfItExistsAndGenerationIsSame()
        {
            using (new MunqContext())
            {
                var localCache = A.Fake<ILocalCache>();
                var distributedCache = A.Fake<IDistributedCache>();

                A.CallTo(() => localCache.Get<object>("SomeKey"))
                    .Returns("SomeValue");

                A.CallTo(() => localCache.Get<object>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .Returns(789UL);

                A.CallTo(() => localCache.Get<object>("SomeGenerationKey"))
                    .Returns(789UL);

                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                registrar.RegisterInstance<ILocalCache>(localCache);
                registrar.RegisterInstance<IDistributedCache>(distributedCache);

                var value = TwoLevelCache.Get("SomeKey", TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(30),
                    "SomeGenerationKey", () => "AnotherValue");

                Assert.Equal("SomeValue", value);

                A.CallTo(() => localCache.Get<object>("SomeKey"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Get<object>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Get<object>("SomeGenerationKey"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Get<ulong?>(A<string>.Ignored))
                    .MustNotHaveHappened();

                A.CallTo(() => distributedCache.Get<string>(A<string>.Ignored))
                    .MustNotHaveHappened();

                A.CallTo(() => distributedCache.Set<ulong>(A<string>.Ignored, A<ulong>.Ignored))
                    .MustNotHaveHappened();

                A.CallTo(() => distributedCache.Set<ulong>(A<string>.Ignored, A<ulong>.Ignored, A<TimeSpan>.Ignored))
                    .MustNotHaveHappened();

                A.CallTo(() => localCache.Add(A<string>.Ignored, A<string>.Ignored, A<TimeSpan>.Ignored))
                    .MustNotHaveHappened();
            }
        }

        [Fact]
        public void TwoLevelCache_Get_TriesDistributedCacheIfLocalCachedItemDoesntExist()
        {
            using (new MunqContext())
            {
                var localCache = A.Fake<ILocalCache>();
                var distributedCache = A.Fake<IDistributedCache>();

                A.CallTo(() => localCache.Get<object>("SomeKey"))
                    .Returns(null);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeGenerationKey"))
                    .Returns(987UL);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .Returns(987UL);

                A.CallTo(() => distributedCache.Get<string>("SomeKey"))
                    .Returns("DistributedCacheValue");

                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                registrar.RegisterInstance<ILocalCache>(localCache);
                registrar.RegisterInstance<IDistributedCache>(distributedCache);

                var value = TwoLevelCache.Get("SomeKey", TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(30),
                    "SomeGenerationKey", () => "AnotherValue");

                Assert.Equal("DistributedCacheValue", value);

                A.CallTo(() => localCache.Get<object>("SomeKey"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Get<object>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .MustNotHaveHappened();

                A.CallTo(() => localCache.Add("SomeKey", "DistributedCacheValue", TimeSpan.FromMinutes(5)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Add("SomeKey" + TwoLevelCache.GenerationSuffix, 987UL, TimeSpan.FromMinutes(5)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Add("SomeGenerationKey", 987UL, TwoLevelCache.GenerationCacheExpiration))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeGenerationKey"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Get<string>("SomeKey"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Set<ulong>(A<string>.Ignored, A<ulong>.Ignored))
                    .MustNotHaveHappened();

                A.CallTo(() => distributedCache.Set<ulong>(A<string>.Ignored, A<ulong>.Ignored, A<TimeSpan>.Ignored))
                    .MustNotHaveHappened();
            }
        }

        [Fact]
        public void TwoLevelCache_Get_TriesDistributedCacheIfLocalCachedItemExistsButGenerationIsDifferent()
        {
            using (new MunqContext())
            {
                var localCache = A.Fake<ILocalCache>();
                var distributedCache = A.Fake<IDistributedCache>();

                A.CallTo(() => localCache.Get<object>("SomeKey"))
                    .Returns("SomeValue");

                A.CallTo(() => localCache.Get<object>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .Returns(789UL);

                A.CallTo(() => localCache.Get<object>("SomeGenerationKey"))
                    .Returns(135UL);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeGenerationKey"))
                    .Returns(987UL);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .Returns(987UL);

                A.CallTo(() => distributedCache.Get<string>("SomeKey"))
                    .Returns("DistributedCacheValue");

                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                registrar.RegisterInstance<ILocalCache>(localCache);
                registrar.RegisterInstance<IDistributedCache>(distributedCache);

                var value = TwoLevelCache.Get("SomeKey", TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(30),
                    "SomeGenerationKey", () => "AnotherValue");

                Assert.Equal("DistributedCacheValue", value);

                A.CallTo(() => localCache.Get<object>("SomeKey"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Get<object>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Add("SomeKey", "DistributedCacheValue", TimeSpan.FromMinutes(5)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Add("SomeKey" + TwoLevelCache.GenerationSuffix, 987UL, TimeSpan.FromMinutes(5)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Add("SomeGenerationKey", 987UL, TwoLevelCache.GenerationCacheExpiration))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeGenerationKey"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Get<string>("SomeKey"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Set<ulong>(A<string>.Ignored, A<ulong>.Ignored))
                    .MustNotHaveHappened();

                A.CallTo(() => distributedCache.Set<ulong>(A<string>.Ignored, A<ulong>.Ignored, A<TimeSpan>.Ignored))
                    .MustNotHaveHappened();
            }
        }

        [Fact]
        public void TwoLevelCache_Get_CallsLoaderFunctionIfDistributedItemCacheExistsButGenerationDifferent()
        {
            using (new MunqContext())
            {
                var localCache = A.Fake<ILocalCache>();
                var distributedCache = A.Fake<IDistributedCache>();

                A.CallTo(() => localCache.Get<object>("SomeKey"))
                    .Returns("SomeValue");

                A.CallTo(() => localCache.Get<object>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .Returns(789UL);

                A.CallTo(() => localCache.Get<object>("SomeGenerationKey"))
                    .Returns(135UL);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeGenerationKey"))
                    .Returns(987UL);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .Returns(886UL);

                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                registrar.RegisterInstance<ILocalCache>(localCache);
                registrar.RegisterInstance<IDistributedCache>(distributedCache);

                var value = TwoLevelCache.Get("SomeKey", TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(30),
                    "SomeGenerationKey", () => "AnotherValue");

                Assert.Equal("AnotherValue", value);

                A.CallTo(() => localCache.Get<object>("SomeKey"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Get<object>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Add("SomeKey", "AnotherValue", TimeSpan.FromMinutes(5)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Add("SomeKey" + TwoLevelCache.GenerationSuffix, 987UL, TimeSpan.FromMinutes(5)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Add("SomeGenerationKey", 987UL, TwoLevelCache.GenerationCacheExpiration))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeGenerationKey"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Get<string>("SomeKey"))
                    .MustNotHaveHappened();

                A.CallTo(() => distributedCache.Set<ulong>("SomeGenerationKey", A<ulong>.Ignored))
                    .MustNotHaveHappened();

                A.CallTo(() => distributedCache.Set("SomeKey", "AnotherValue", TimeSpan.FromMinutes(30)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Set("SomeKey" + TwoLevelCache.GenerationSuffix, 987UL, TimeSpan.FromMinutes(30)))
                    .MustHaveHappened(1, Times.Exactly);
            }
        }

        [Fact]
        public void TwoLevelCache_Get_CallsLoaderFunctionIfLocalAndDistributedCachedItemDoesntExist()
        {
            using (new MunqContext())
            {
                var localCache = A.Fake<ILocalCache>();
                var distributedCache = A.Fake<IDistributedCache>();

                A.CallTo(() => localCache.Get<object>("SomeKey"))
                    .Returns(null);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .Returns(null);

                ulong? generationKey = null;

                A.CallTo(() => distributedCache.Set<ulong>("SomeGenerationKey", A<ulong>.Ignored))
                    .Invokes((string x, ulong y) =>
                    {
                        generationKey = y;
                    });

                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                registrar.RegisterInstance<ILocalCache>(localCache);
                registrar.RegisterInstance<IDistributedCache>(distributedCache);

                var value = TwoLevelCache.Get("SomeKey", TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(30),
                    "SomeGenerationKey", () => "AnotherValue");

                Assert.NotNull(generationKey);
                Assert.Equal("AnotherValue", value);

                A.CallTo(() => localCache.Get<object>("SomeKey"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Get<object>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .MustNotHaveHappened();

                A.CallTo(() => localCache.Add("SomeKey", "AnotherValue", TimeSpan.FromMinutes(5)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Add("SomeKey" + TwoLevelCache.GenerationSuffix, generationKey.Value, TimeSpan.FromMinutes(5)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Add("SomeGenerationKey", generationKey.Value, TwoLevelCache.GenerationCacheExpiration))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeGenerationKey"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Get<string>("SomeKey"))
                    .MustNotHaveHappened();

                A.CallTo(() => distributedCache.Set("SomeGenerationKey", generationKey.Value))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Set("SomeKey", "AnotherValue", TimeSpan.FromMinutes(30)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Set("SomeKey" + TwoLevelCache.GenerationSuffix, generationKey.Value, TimeSpan.FromMinutes(30)))
                    .MustHaveHappened(1, Times.Exactly);
            }
        }

        [Fact]
        public void TwoLevelCache_GetSingleExpirationOverload_WorksProperly()
        {
            using (new MunqContext())
            {
                var localCache = A.Fake<ILocalCache>();
                var distributedCache = A.Fake<IDistributedCache>();

                A.CallTo(() => localCache.Get<object>("SomeKey"))
                    .Returns(null);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .Returns(null);

                ulong? generationKey = null;

                A.CallTo(() => distributedCache.Set<ulong>("SomeGenerationKey", A<ulong>.Ignored))
                    .Invokes((string x, ulong y) =>
                    {
                        generationKey = y;
                    });

                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                registrar.RegisterInstance<ILocalCache>(localCache);
                registrar.RegisterInstance<IDistributedCache>(distributedCache);

                var value = TwoLevelCache.Get("SomeKey", TimeSpan.FromMinutes(5),
                    "SomeGenerationKey", () => "AnotherValue");

                Assert.NotNull(generationKey);
                Assert.Equal("AnotherValue", value);

                A.CallTo(() => localCache.Get<object>("SomeKey"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Get<object>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .MustNotHaveHappened();

                A.CallTo(() => localCache.Add("SomeKey", "AnotherValue", TimeSpan.FromMinutes(5)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Add("SomeKey" + TwoLevelCache.GenerationSuffix, generationKey.Value, TimeSpan.FromMinutes(5)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Add("SomeGenerationKey", generationKey.Value, TwoLevelCache.GenerationCacheExpiration))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeGenerationKey"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Get<string>("SomeKey"))
                    .MustNotHaveHappened();

                A.CallTo(() => distributedCache.Set("SomeGenerationKey", generationKey.Value))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Set("SomeKey", "AnotherValue", TimeSpan.FromMinutes(5)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Set("SomeKey" + TwoLevelCache.GenerationSuffix, generationKey.Value, TimeSpan.FromMinutes(5)))
                    .MustHaveHappened(1, Times.Exactly);
            }
        }

        [Fact]
        public void TwoLevelCache_GetWithCustomSerializer_ThrowsExceptionIfNoLocalCacheProviderIsRegistered()
        {
            using (new MunqContext())
            {
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance<IDistributedCache>(new DistributedCacheEmulator());

                var exception = Assert.Throws<KeyNotFoundException>(() =>
                    TwoLevelCache.GetWithCustomSerializer("x", TimeSpan.Zero, TimeSpan.Zero, "gen", () => "a",
                        x => x, y => y));

                Assert.Contains(typeof(ILocalCache).Name, exception.Message);
            }
        }

        [Fact]
        public void TwoLevelCache_GetWithCustomSerializer_ThrowsExceptionIfNoDistributedCacheProviderIsRegistered()
        {
            using (new MunqContext())
            {
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance<ILocalCache>(new HttpRuntimeCache());

                var exception = Assert.Throws<KeyNotFoundException>(() =>
                    TwoLevelCache.GetWithCustomSerializer("x", TimeSpan.Zero, TimeSpan.Zero, "gen", () => "a",
                        x => x, y => y));

                Assert.Contains(typeof(IDistributedCache).Name, exception.Message);
            }
        }

        [Fact]
        public void TwoLevelCache_GetWithCustomSerializer_UsesRegisteredCacheProviders()
        {
            using (new MunqContext())
            {
                var localCache = A.Fake<ILocalCache>();
                var distributedCache = A.Fake<IDistributedCache>();
                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                registrar.RegisterInstance<ILocalCache>(localCache);
                registrar.RegisterInstance<IDistributedCache>(distributedCache);

                ulong? generationKey = null;

                A.CallTo(() => localCache.Get<object>("SomeKey"))
                    .Returns(null);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .Returns(null);

                A.CallTo(() => distributedCache.Set<ulong>("SomeGenerationKey", A<ulong>.Ignored))
                    .Invokes((string x, ulong y) =>
                    {
                        generationKey = y;
                    });

                var value = TwoLevelCache.GetWithCustomSerializer("SomeKey", TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(30),
                    "SomeGenerationKey", () => "xyz", x => "serialized:" + x, y => y == null ? null : y.Substring("serialized:".Length));

                Assert.Equal("xyz", value);
                Assert.NotNull(generationKey);

                A.CallTo(() => localCache.Get<object>("SomeKey"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Add("SomeKey", "xyz", TimeSpan.FromMinutes(5)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Add("SomeKey" + TwoLevelCache.GenerationSuffix, generationKey.Value, TimeSpan.FromMinutes(5)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Add("SomeGenerationKey", generationKey.Value, TwoLevelCache.GenerationCacheExpiration))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Set("SomeGenerationKey", generationKey.Value))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Set("SomeKey", "serialized:xyz", TimeSpan.FromMinutes(30)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Set("SomeKey" + TwoLevelCache.GenerationSuffix, generationKey.Value, TimeSpan.FromMinutes(30)))
                    .MustHaveHappened(1, Times.Exactly);
            }
        }

        [Fact]
        public void TwoLevelCache_GetWithCustomSerializer_UsesSerializerProperly()
        {
            using (new MunqContext())
            {
                var localCache = A.Fake<ILocalCache>();
                var distributedCache = A.Fake<IDistributedCache>();
                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                registrar.RegisterInstance<ILocalCache>(localCache);
                registrar.RegisterInstance<IDistributedCache>(distributedCache);

                A.CallTo(() => localCache.Get<object>(A<String>.Ignored))
                    .Returns(null);

                A.CallTo(() => distributedCache.Get<ulong?>(A<String>.Ignored))
                    .Returns(null);

                var value = TwoLevelCache.GetWithCustomSerializer("SomeKey", TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(30),
                    "SomeGenerationKey", () => "xyz", x => "serialized:" + x, y => y == null ? null : y.Substring("serialized:".Length));

                Assert.Equal("xyz", value);

                A.CallTo(() => localCache.Add("SomeKey", "xyz", TimeSpan.FromMinutes(5)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Set("SomeKey", "serialized:xyz", TimeSpan.FromMinutes(30)))
                    .MustHaveHappened(1, Times.Exactly);
            }
        }

        [Fact]
        public void TwoLevelCache_GetWithCustomSerializer_UsesDeserializerProperly()
        {
            using (new MunqContext())
            {
                var localCache = A.Fake<ILocalCache>();
                var distributedCache = A.Fake<IDistributedCache>();
                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                registrar.RegisterInstance<ILocalCache>(localCache);
                registrar.RegisterInstance<IDistributedCache>(distributedCache);

                A.CallTo(() => localCache.Get<object>(A<String>.Ignored))
                    .Returns(null);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeGenerationKey"))
                    .Returns(123UL);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .Returns(123UL);

                A.CallTo(() => distributedCache.Get<string>("SomeKey"))
                    .Returns("serialized:SerializedValue");

                var value = TwoLevelCache.GetWithCustomSerializer("SomeKey", TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(30),
                    "SomeGenerationKey", () => "ValueIfNotFound", x => "serialized:" + x, y => y == null ? null : y.Substring("serialized:".Length));

                Assert.Equal("SerializedValue", value);
            }
        }

        [Fact]
        public void TwoLevelCache_GetLocalStoreOnly_ThrowsExceptionIfNoLocalCacheProviderIsRegistered()
        {
            using (new MunqContext())
            {
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance<IDistributedCache>(new DistributedCacheEmulator());

                var exception = Assert.Throws<KeyNotFoundException>(() =>
                    TwoLevelCache.GetLocalStoreOnly<object>("x", TimeSpan.Zero, "gen", () => "a"));

                Assert.Contains(typeof(ILocalCache).Name, exception.Message);
            }
        }

        [Fact]
        public void TwoLevelCache_GetLocalStoreOnly_ThrowsExceptionIfNoDistributedCacheProviderIsRegistered()
        {
            using (new MunqContext())
            {
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance<ILocalCache>(new HttpRuntimeCache());

                var exception = Assert.Throws<KeyNotFoundException>(() =>
                    TwoLevelCache.GetLocalStoreOnly<object>("x", TimeSpan.Zero, "gen", () => "a"));

                Assert.Contains(typeof(IDistributedCache).Name, exception.Message);
            }
        }

        [Fact]
        public void TwoLevelCache_GetLocalStoreOnly_UsesRegisteredCacheProviders()
        {
            using (new MunqContext())
            {
                var localCache = A.Fake<ILocalCache>();
                var distributedCache = A.Fake<IDistributedCache>();
                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                registrar.RegisterInstance<ILocalCache>(localCache);
                registrar.RegisterInstance<IDistributedCache>(distributedCache);

                ulong? generationKey = null;

                A.CallTo(() => localCache.Get<object>("SomeKey"))
                    .Returns(null);

                A.CallTo(() => distributedCache.Set<ulong>("SomeGenerationKey", A<ulong>.Ignored))
                    .Invokes((string x, ulong y) =>
                    {
                        generationKey = y;
                    });

                var value = TwoLevelCache.GetLocalStoreOnly("SomeKey", TimeSpan.FromMinutes(5),
                    "SomeGenerationKey", () => "xyz");

                Assert.Equal("xyz", value);
                Assert.NotNull(generationKey);

                A.CallTo(() => localCache.Get<object>("SomeKey"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Add("SomeKey", "xyz", TimeSpan.FromMinutes(5)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Add("SomeKey" + TwoLevelCache.GenerationSuffix, generationKey.Value, TimeSpan.FromMinutes(5)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Add("SomeGenerationKey", generationKey.Value, TwoLevelCache.GenerationCacheExpiration))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .MustNotHaveHappened();

                A.CallTo(() => distributedCache.Set("SomeGenerationKey", generationKey.Value))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Set(A<string>.Ignored, A<string>.Ignored, A<TimeSpan>.Ignored))
                    .MustNotHaveHappened();
            }
        }

        [Fact]
        public void TwoLevelCache_GetLocalStoreOnly_CallsLoaderFunctionIfLocalCachedItemGenerationIsDifferent()
        {
            using (new MunqContext())
            {
                var localCache = A.Fake<ILocalCache>();
                var distributedCache = A.Fake<IDistributedCache>();

                A.CallTo(() => localCache.Get<object>("SomeKey"))
                    .Returns("SomeValue");

                A.CallTo(() => localCache.Get<object>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .Returns(123UL);

                A.CallTo(() => localCache.Get<object>("SomeGenerationKey"))
                    .Returns(456UL);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeGenerationKey"))
                     .Returns(987UL);

                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                registrar.RegisterInstance<ILocalCache>(localCache);
                registrar.RegisterInstance<IDistributedCache>(distributedCache);

                var value = TwoLevelCache.GetLocalStoreOnly("SomeKey", TimeSpan.FromMinutes(5),
                    "SomeGenerationKey", () => "AnotherValue");

                Assert.Equal("AnotherValue", value);

                A.CallTo(() => localCache.Get<object>("SomeKey"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Get<object>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Get<object>("SomeGenerationKey"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Add("SomeKey", "AnotherValue", TimeSpan.FromMinutes(5)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Add("SomeKey" + TwoLevelCache.GenerationSuffix, 987UL, TimeSpan.FromMinutes(5)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Add("SomeGenerationKey", 987UL, TwoLevelCache.GenerationCacheExpiration))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Get<ulong?>(A<string>.Ignored))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Get<string>(A<string>.Ignored))
                    .MustNotHaveHappened();

                A.CallTo(() => distributedCache.Set<ulong>(A<string>.Ignored, A<ulong>.Ignored))
                    .MustNotHaveHappened();

                A.CallTo(() => distributedCache.Set<ulong>(A<string>.Ignored, A<ulong>.Ignored, A<TimeSpan>.Ignored))
                    .MustNotHaveHappened();
            }

        }

        [Fact]
        public void TwoLevelCache_GetLocalStoreOnly_CallsLoaderFunctionIfLocalCachedItemDoesntExist()
        {
            using (new MunqContext())
            {
                var localCache = A.Fake<ILocalCache>();
                var distributedCache = A.Fake<IDistributedCache>();

                A.CallTo(() => localCache.Get<object>("SomeKey"))
                    .Returns(null);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeGenerationKey"))
                     .Returns(987UL);

                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                registrar.RegisterInstance<ILocalCache>(localCache);
                registrar.RegisterInstance<IDistributedCache>(distributedCache);

                var value = TwoLevelCache.GetLocalStoreOnly("SomeKey", TimeSpan.FromMinutes(5),
                    "SomeGenerationKey", () => "AnotherValue");

                Assert.Equal("AnotherValue", value);

                A.CallTo(() => localCache.Get<object>("SomeKey"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Get<object>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .MustNotHaveHappened();

                A.CallTo(() => localCache.Get<object>("SomeGenerationKey"))
                    .MustNotHaveHappened();

                A.CallTo(() => localCache.Add("SomeKey", "AnotherValue", TimeSpan.FromMinutes(5)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Add("SomeKey" + TwoLevelCache.GenerationSuffix, 987UL, TimeSpan.FromMinutes(5)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Add("SomeGenerationKey", 987UL, TwoLevelCache.GenerationCacheExpiration))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Get<ulong?>(A<string>.Ignored))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Get<string>(A<string>.Ignored))
                    .MustNotHaveHappened();

                A.CallTo(() => distributedCache.Set<ulong>(A<string>.Ignored, A<ulong>.Ignored))
                    .MustNotHaveHappened();

                A.CallTo(() => distributedCache.Set<ulong>(A<string>.Ignored, A<ulong>.Ignored, A<TimeSpan>.Ignored))
                    .MustNotHaveHappened();
            }
        }

        [Fact]
        public void TwoLevelCache_GetLocalStoreOnly_UsesLoaderFunctionIfLocalItemGenerationDoesntExist()
        {
            using (new MunqContext())
            {
                var localCache = A.Fake<ILocalCache>();
                var distributedCache = A.Fake<IDistributedCache>();

                A.CallTo(() => localCache.Get<object>("SomeKey"))
                    .Returns("Dummy");

                A.CallTo(() => localCache.Get<ulong?>("SomeGenerationKey" + TwoLevelCache.GenerationSuffix))
                    .Returns(null);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeGenerationKey"))
                     .Returns(987UL);

                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                registrar.RegisterInstance<ILocalCache>(localCache);
                registrar.RegisterInstance<IDistributedCache>(distributedCache);

                var value = TwoLevelCache.GetLocalStoreOnly("SomeKey", TimeSpan.FromMinutes(5),
                    "SomeGenerationKey", () => "AnotherValue");

                Assert.Equal("AnotherValue", value);

                A.CallTo(() => localCache.Get<object>("SomeKey"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Get<object>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Get<object>("SomeGenerationKey"))
                    .MustNotHaveHappened();

                A.CallTo(() => localCache.Add("SomeKey", "AnotherValue", TimeSpan.FromMinutes(5)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Add("SomeKey" + TwoLevelCache.GenerationSuffix, 987UL, TimeSpan.FromMinutes(5)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Add("SomeGenerationKey", 987UL, TwoLevelCache.GenerationCacheExpiration))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Get<ulong?>(A<string>.Ignored))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Get<string>(A<string>.Ignored))
                    .MustNotHaveHappened();

                A.CallTo(() => distributedCache.Set<ulong>(A<string>.Ignored, A<ulong>.Ignored))
                    .MustNotHaveHappened();

                A.CallTo(() => distributedCache.Set<ulong>(A<string>.Ignored, A<ulong>.Ignored, A<TimeSpan>.Ignored))
                    .MustNotHaveHappened();
            }
        }

        [Fact]
        public void TwoLevelCache_GetLocalStoreOnly_GetsGenerationFromDistributedCacheIfLocalCachedGenerationDoesntExist()
        {
            using (new MunqContext())
            {
                var localCache = A.Fake<ILocalCache>();
                var distributedCache = A.Fake<IDistributedCache>();

                A.CallTo(() => localCache.Get<object>("SomeKey"))
                    .Returns("LocalValue");

                A.CallTo(() => localCache.Get<object>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .Returns(987UL);

                A.CallTo(() => localCache.Get<object>("SomeGenerationKey"))
                    .Returns(null);

                A.CallTo(() => distributedCache.Get<ulong?>("SomeGenerationKey"))
                     .Returns(987UL);

                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                registrar.RegisterInstance<ILocalCache>(localCache);
                registrar.RegisterInstance<IDistributedCache>(distributedCache);

                var value = TwoLevelCache.GetLocalStoreOnly("SomeKey", TimeSpan.FromMinutes(5),
                    "SomeGenerationKey", () => "AnotherValue");

                Assert.Equal("LocalValue", value);

                A.CallTo(() => localCache.Get<object>("SomeKey"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Get<object>("SomeKey" + TwoLevelCache.GenerationSuffix))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Get<object>("SomeGenerationKey"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => localCache.Add(A<String>.Ignored, A<String>.Ignored, A<TimeSpan>.Ignored))
                    .MustNotHaveHappened();

                A.CallTo(() => localCache.Add("SomeKey" + TwoLevelCache.GenerationSuffix, A<ulong>.Ignored, A<TimeSpan>.Ignored))
                    .MustNotHaveHappened();

                A.CallTo(() => localCache.Add("SomeGenerationKey", 987UL, TwoLevelCache.GenerationCacheExpiration))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Get<ulong?>(A<string>.Ignored))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => distributedCache.Get<string>(A<string>.Ignored))
                    .MustNotHaveHappened();

                A.CallTo(() => distributedCache.Set<ulong>(A<string>.Ignored, A<ulong>.Ignored))
                    .MustNotHaveHappened();

                A.CallTo(() => distributedCache.Set<ulong>(A<string>.Ignored, A<ulong>.Ignored, A<TimeSpan>.Ignored))
                    .MustNotHaveHappened();
            }
        }

        [Fact]
        public static void TwoLevelCache_ChangeGlobalGeneration_ExpiresItems()
        {
            using (new MunqContext())
            {
                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                registrar.RegisterInstance<ILocalCache>(new LocalCacheEmulator());
                registrar.RegisterInstance<IDistributedCache>(new DistributedCacheEmulator());

                var value = TwoLevelCache.Get("SomeKey", TimeSpan.FromDays(5), "GroupKey", () => "FirstValue");
                Assert.Equal("FirstValue", value);
                Assert.Equal("FirstValue", LocalCache.TryGet<string>("SomeKey"));
                Assert.Equal("FirstValue", DistributedCache.Get<string>("SomeKey"));
                var generation = DistributedCache.Get<ulong?>("GroupKey");
                Assert.Equal(generation, DistributedCache.Get<ulong>("SomeKey" + TwoLevelCache.GenerationSuffix));
                Assert.Equal(generation, LocalCache.TryGet<object>("GroupKey") as ulong?);
                Assert.Equal(generation, LocalCache.TryGet<object>("SomeKey" + TwoLevelCache.GenerationSuffix));

                value = TwoLevelCache.Get("SomeKey", TimeSpan.FromDays(5), "GroupKey", () => "SecondValue");
                Assert.Equal("FirstValue", value);

                #pragma warning disable 618
                TwoLevelCache.ChangeGlobalGeneration("GroupKey");
                #pragma warning restore 618
                Assert.Null(DistributedCache.Get<ulong?>("GroupKey"));
                Assert.Null(LocalCache.TryGet<object>("GroupKey"));

                value = TwoLevelCache.Get("SomeKey", TimeSpan.FromDays(5), "GroupKey", () => "ThirdValue");
                Assert.Equal("ThirdValue", value);
            }
        }

        [Fact]
        public static void TwoLevelCache_ExpireGroupItems_ExpiresItems()
        {
            using (new MunqContext())
            {
                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                registrar.RegisterInstance<ILocalCache>(new LocalCacheEmulator());
                registrar.RegisterInstance<IDistributedCache>(new DistributedCacheEmulator());

                var value = TwoLevelCache.Get("SomeKey", TimeSpan.FromDays(5), "GroupKey", () => "FirstValue");
                Assert.Equal("FirstValue", value);
                Assert.Equal("FirstValue", LocalCache.TryGet<string>("SomeKey"));
                Assert.Equal("FirstValue", DistributedCache.Get<string>("SomeKey"));
                var generation = DistributedCache.Get<ulong?>("GroupKey");
                Assert.Equal(generation, DistributedCache.Get<ulong>("SomeKey" + TwoLevelCache.GenerationSuffix));
                Assert.Equal(generation, LocalCache.TryGet<object>("GroupKey") as ulong?);
                Assert.Equal(generation, LocalCache.TryGet<object>("SomeKey" + TwoLevelCache.GenerationSuffix));

                value = TwoLevelCache.Get("SomeKey", TimeSpan.FromDays(5), "GroupKey", () => "SecondValue");
                Assert.Equal("FirstValue", value);

                TwoLevelCache.ExpireGroupItems("GroupKey");
                Assert.Null(DistributedCache.Get<ulong?>("GroupKey"));
                Assert.Null(LocalCache.TryGet<object>("GroupKey"));

                value = TwoLevelCache.Get("SomeKey", TimeSpan.FromDays(5), "GroupKey", () => "ThirdValue");
                Assert.Equal("ThirdValue", value);
            }
        }

        [Fact]
        public static void TwoLevelCache_Remove_RemovesCachedItemsFromLocalAndDistributedCaches()
        {
            using (new MunqContext())
            {
                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                registrar.RegisterInstance<ILocalCache>(new LocalCacheEmulator());
                registrar.RegisterInstance<IDistributedCache>(new DistributedCacheEmulator());

                var value = TwoLevelCache.Get("SomeKey", TimeSpan.FromDays(5), "GroupKey", () => "FirstValue");
                Assert.Equal("FirstValue", value);
                Assert.Equal("FirstValue", LocalCache.TryGet<string>("SomeKey"));
                Assert.Equal("FirstValue", DistributedCache.Get<string>("SomeKey"));
                var generation = DistributedCache.Get<ulong?>("GroupKey");
                Assert.Equal(generation, DistributedCache.Get<ulong>("SomeKey" + TwoLevelCache.GenerationSuffix));
                Assert.Equal(generation, LocalCache.TryGet<object>("GroupKey") as ulong?);
                Assert.Equal(generation, LocalCache.TryGet<object>("SomeKey" + TwoLevelCache.GenerationSuffix));

                TwoLevelCache.Remove("SomeKey");
                Assert.Null(LocalCache.TryGet<string>("SomeKey"));
                Assert.Null(LocalCache.TryGet<object>("SomeKey" + TwoLevelCache.GenerationSuffix));
                Assert.Null(DistributedCache.Get<string>("SomeKey"));
                Assert.Null(DistributedCache.Get<ulong?>("SomeKey" + TwoLevelCache.GenerationSuffix));
            }
        }
    }
}