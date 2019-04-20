using FakeItEasy;
using Serenity.Abstractions;
using Serenity.ComponentModel;
using Serenity.Testing;
using System;
using System.Collections.Generic;
using Xunit;

namespace Serenity.Test
{
    [Collection("AvoidParallel")]
    public partial class ConfigTests
    {
        private class ApplicationSettings
        {
            public int Dummy { get; set; }
        }

        [SettingScope("Server")]
        private class ServerSettings
        {
            public int Dummy { get; set; }
        }

        [Fact]
        public void Config_GetWithType_ThrowsExceptionIfNoConfigurationRepositoryIsRegistered()
        {
            using (new MunqContext())
            {
                var exception = Assert.Throws<KeyNotFoundException>(() => Config.Get(typeof(ApplicationSettings)));
                Assert.Contains(typeof(IConfigurationRepository).Name, exception.Message);
            }
        }

        [Fact]
        public void Config_GetWithType_ThrowsExceptionIfNoConfigurationRepositoryIsRegisteredForSettingScope()
        {
            using (new MunqContext())
            {
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance<IConfigurationRepository>("Server", A.Fake<IConfigurationRepository>());

                Config.Get(typeof(ServerSettings));

                var exception = Assert.Throws<KeyNotFoundException>(() => Config.Get(typeof(ApplicationSettings)));
                Assert.Contains(typeof(IConfigurationRepository).Name, exception.Message);
            }
        }

        [Fact]
        public void Config_GetWithType_UsesRegisteredConfigurationProviderUsingScopeAttribute()
        {
            using (new MunqContext())
            {
                var repository = A.Fake<IConfigurationRepository>();
                A.CallTo(() => repository.Load(A<Type>.Ignored))
                    .ReturnsLazily((Type x) => Activator.CreateInstance(x));

                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance<IConfigurationRepository>("Server", repository);

                Config.Get(typeof(ServerSettings));

                A.CallTo(() => repository.Load(typeof(ServerSettings)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => repository.Load(A<Type>.That.Not.Matches(x => x == typeof(ServerSettings))))
                    .MustNotHaveHappened();
            }
        }

        [Fact]
        public void Config_GetWithType_UsesApplicationScopeIfScopeIsNotSpecified()
        {
            using (new MunqContext())
            {
                var repository = A.Fake<IConfigurationRepository>();
                A.CallTo(() => repository.Load(A<Type>.Ignored))
                    .ReturnsLazily((Type x) => Activator.CreateInstance(x));

                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance<IConfigurationRepository>("Application", repository);

                Config.Get(typeof(ApplicationSettings));

                A.CallTo(() => repository.Load(typeof(ApplicationSettings)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => repository.Load(A<Type>.That.Not.Matches(x => x == typeof(ApplicationSettings))))
                    .MustNotHaveHappened();
            }
        }

        [Fact]
        public void Config_GetGeneric_ThrowsExceptionIfNoConfigurationRepositoryIsRegistered()
        {
            using (new MunqContext())
            {
                var exception = Assert.Throws<KeyNotFoundException>(() => Config.Get<ApplicationSettings>());
                Assert.Contains(typeof(IConfigurationRepository).Name, exception.Message);
            }
        }

        [Fact]
        public void Config_GetGeneric_ThrowsExceptionIfNoConfigurationRepositoryIsRegisteredForSettingScope()
        {
            using (new MunqContext())
            {
                var repository = A.Fake<IConfigurationRepository>();
                A.CallTo(() => repository.Load(A<Type>.Ignored))
                    .ReturnsLazily((Type x) => Activator.CreateInstance(x));

                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance<IConfigurationRepository>("Server", repository);

                Config.Get<ServerSettings>();

                var exception = Assert.Throws<KeyNotFoundException>(() => Config.Get<ApplicationSettings>());
                Assert.Contains(typeof(IConfigurationRepository).Name, exception.Message);
            }
        }

        [Fact]
        public void Config_GetGeneric_UsesRegisteredConfigurationProviderUsingScopeAttribute()
        {
            using (new MunqContext())
            {
                var repository = A.Fake<IConfigurationRepository>();
                A.CallTo(() => repository.Load(A<Type>.Ignored))
                    .ReturnsLazily((Type x) => Activator.CreateInstance(x));

                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance<IConfigurationRepository>("Server", repository);

                Config.Get<ServerSettings>();

                A.CallTo(() => repository.Load(typeof(ServerSettings)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => repository.Load(A<Type>.That.Not.Matches(x => x == typeof(ServerSettings))))
                    .MustNotHaveHappened();
            }
        }

        [Fact]
        public void Config_GetGeneric_UsesApplicationScopeIfScopeIsNotSpecified()
        {
            using (new MunqContext())
            {
                var repository = A.Fake<IConfigurationRepository>();
                A.CallTo(() => repository.Load(A<Type>.Ignored))
                    .ReturnsLazily((Type x) => Activator.CreateInstance(x));

                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance<IConfigurationRepository>("Application", repository);

                Config.Get<ApplicationSettings>();

                A.CallTo(() => repository.Load(typeof(ApplicationSettings)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => repository.Load(A<Type>.That.Not.Matches(x => x == typeof(ApplicationSettings))))
                    .MustNotHaveHappened();
            }
        }

        [Fact]
        public void Config_TryGetWithType_ReturnsNullIfNoConfigurationRepositoryIsRegistered()
        {
            using (new MunqContext())
            {
                Assert.Null(Config.TryGet(typeof(ApplicationSettings)));
            }
        }

        [Fact]
        public void Config_TryGetWithType_ReturnsNullIfNoConfigurationRepositoryIsRegisteredForSettingScope()
        {
            using (new MunqContext())
            {
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance<IConfigurationRepository>("Server", A.Fake<IConfigurationRepository>());

                Assert.NotNull(Config.TryGet(typeof(ServerSettings)));
                Assert.Null(Config.TryGet(typeof(ApplicationSettings)));
            }
        }

        [Fact]
        public void Config_TryGetWithType_UsesRegisteredConfigurationProviderUsingScopeAttribute()
        {
            using (new MunqContext())
            {
                var repository = A.Fake<IConfigurationRepository>();
                A.CallTo(() => repository.Load(A<Type>.Ignored))
                    .ReturnsLazily((Type x) => Activator.CreateInstance(x));

                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance<IConfigurationRepository>("Server", repository);

                Config.TryGet(typeof(ServerSettings));

                A.CallTo(() => repository.Load(typeof(ServerSettings)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => repository.Load(A<Type>.That.Not.Matches(x => x == typeof(ServerSettings))))
                    .MustNotHaveHappened();
            }
        }

        [Fact]
        public void Config_TryGetWithType_UsesApplicationScopeIfScopeIsNotSpecified()
        {
            using (new MunqContext())
            {
                var repository = A.Fake<IConfigurationRepository>();
                A.CallTo(() => repository.Load(A<Type>.Ignored))
                    .ReturnsLazily((Type x) => Activator.CreateInstance(x));

                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance<IConfigurationRepository>("Application", repository);

                Config.TryGet(typeof(ApplicationSettings));

                A.CallTo(() => repository.Load(typeof(ApplicationSettings)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => repository.Load(A<Type>.That.Not.Matches(x => x == typeof(ApplicationSettings))))
                    .MustNotHaveHappened();
            }
        }

        [Fact]
        public void Config_TryGetGeneric_ReturnsNullIfNoConfigurationRepositoryIsRegistered()
        {
            using (new MunqContext())
            {
                Assert.Null(Config.TryGet<ApplicationSettings>());
            }
        }

        [Fact]
        public void Config_TryGetGeneric_ReturnsNullIfNoConfigurationRepositoryIsRegisteredForSettingScope()
        {
            using (new MunqContext())
            {
                var repository = A.Fake<IConfigurationRepository>();
                A.CallTo(() => repository.Load(A<Type>.Ignored))
                    .ReturnsLazily((Type x) => Activator.CreateInstance(x));

                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance<IConfigurationRepository>("Server", repository);

                Assert.NotNull(Config.TryGet<ServerSettings>());

                Assert.Null(Config.TryGet<ApplicationSettings>());
            }
        }

        [Fact]
        public void Config_TryGetGeneric_UsesRegisteredConfigurationProviderUsingScopeAttribute()
        {
            using (new MunqContext())
            {
                var repository = A.Fake<IConfigurationRepository>();
                A.CallTo(() => repository.Load(A<Type>.Ignored))
                    .ReturnsLazily((Type x) => Activator.CreateInstance(x));

                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance<IConfigurationRepository>("Server", repository);

                Config.Get(typeof(ServerSettings));

                A.CallTo(() => repository.Load(typeof(ServerSettings)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => repository.Load(A<Type>.That.Not.Matches(x => x == typeof(ServerSettings))))
                    .MustNotHaveHappened();
            }
        }

        [Fact]
        public void Config_TryGetGeneric_UsesApplicationScopeIfScopeIsNotSpecified()
        {
            using (new MunqContext())
            {
                var repository = A.Fake<IConfigurationRepository>();
                A.CallTo(() => repository.Load(A<Type>.Ignored))
                    .ReturnsLazily((Type x) => Activator.CreateInstance(x));

                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance<IConfigurationRepository>("Application", repository);

                Config.Get<ApplicationSettings>();

                A.CallTo(() => repository.Load(typeof(ApplicationSettings)))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => repository.Load(A<Type>.That.Not.Matches(x => x == typeof(ApplicationSettings))))
                    .MustNotHaveHappened();
            }
        }
    }
}