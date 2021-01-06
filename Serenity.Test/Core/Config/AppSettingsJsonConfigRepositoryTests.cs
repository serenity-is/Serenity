using Serenity.Abstractions;
using Serenity.ComponentModel;
using Serenity.Configuration;
using Serenity.Testing;
using System;
using Xunit;

namespace Serenity.Test
{
    [Collection("AvoidParallel")]
    public partial class AppSettingsJsonConfigRepositoryTests
    {
        [Fact]
        public void AppSettingsJsonConfigRepository_SaveThrowsException()
        {
            using (new MunqContext())
            {
                Assert.Throws<NotImplementedException>(() => new AppSettingsJsonConfigRepository()
                    .Save(typeof(DummySettings), new DummySettings()));
            }
        }

        [Fact]
        public void AppSettingsJsonConfigRepository_UsesClassNameIfSettingKeyAttributeIsNotPresent()
        {
            using (new MunqContext())
            {
                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                registrar.RegisterInstance<ILocalCache>(new LocalCacheEmulator());
                registrar.RegisterInstance<IConfigurationManager>(new WebConfigurationWrapper());

                var actual = new AppSettingsJsonConfigRepository().Load(typeof(TestDbSettings)) as TestDbSettings;
                Assert.NotNull(actual);
                Assert.NotNull(actual.Provider);
                Assert.NotNull(actual.Server);
                Assert.NotNull(actual.RootPath);
            }
        }

        [Fact]
        public void AppSettingsJsonConfigRepository_UsesSettingKeyAttributeIfPresent()
        {
            using (new MunqContext())
            {
                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                registrar.RegisterInstance<ILocalCache>(new LocalCacheEmulator());
                registrar.RegisterInstance<IConfigurationManager>(new WebConfigurationWrapper());

                var actual = new AppSettingsJsonConfigRepository().Load(typeof(MyTestDbSettings)) as MyTestDbSettings;
                Assert.NotNull(actual);
                Assert.NotNull(actual.Provider);
                Assert.NotNull(actual.Server);
                Assert.NotNull(actual.RootPath);
            }
        }

        [Fact]
        public void AppSettingsJsonConfigRepository_ReturnsADefaultInstanceIfSettingIsNotFound()
        {
            using (new MunqContext())
            {
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance<ILocalCache>(new LocalCacheEmulator());

                var actual = new AppSettingsJsonConfigRepository().Load(typeof(DummySettings)) as DummySettings;
                Assert.NotNull(actual);
                Assert.Equal("-1", actual.Default);
            }
        }

        private class DummySettings
        {
            public DummySettings()
            {
                Default = "-1";
            }

            public string Default { get; set; }
        }

        private class TestDbSettings
        {
            public string Server { get; set; }
            public string Provider { get; set; }
            public string RootPath { get; set; }
        }

        [SettingKey("TestDbSettings")]
        private class MyTestDbSettings : TestDbSettings
        {
        }
    }
}