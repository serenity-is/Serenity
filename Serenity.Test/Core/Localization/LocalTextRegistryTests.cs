using FakeItEasy;
using Serenity.Localization;
using Serenity.Testing;
using System;
using Xunit;

namespace Serenity.Test
{
    public class LocalTextRegistryTests
    {
        [Fact]
        public void LocalTextRegistry_Add_ThrowsArgumentNull_IfLanguageIDIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new LocalTextRegistry().Add(null, "a", "b"));
        }

        [Fact]
        public void LocalTextRegistry_Add_ThrowsArgumentNull_IfKeyIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new LocalTextRegistry().Add("en", null, "b"));
        }

        [Fact]
        public void LocalTextRegistry_Add_WorksProperly()
        {
            var registry = new LocalTextRegistry();
            registry.Add("es", "key", "translation");

            var actual = registry.TryGet("es", "key");
            Assert.Equal("translation", actual);
        }

        [Fact]
        public void LocalTextRegistry_Add_DoesntTrimAny()
        {
            var registry = new LocalTextRegistry();
            registry.Add("  es  ", " key ", " translation ");

            var actual = registry.TryGet("  es  ", " key ");
            Assert.Equal(" translation ", actual);
        }

        [Fact]
        public void LocalTextRegistry_Add_OverridesExisting()
        {
            var registry = new LocalTextRegistry();
            registry.Add("es", "key", "oldTranslation");

            Assert.Equal("oldTranslation", registry.TryGet("es", "key"));

            registry.Add("es", "key", "newTranslation");

            Assert.Equal("newTranslation", registry.TryGet("es", "key"));
        }

        [Fact]
        public void LocalTextRegistry_AddPending_ThrowsArgumentNull_IfLanguageIDIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new LocalTextRegistry().AddPending(null, "a", "b"));
        }

        [Fact]
        public void LocalTextRegistry_AddPending_ThrowsArgumentNull_IfKeyIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new LocalTextRegistry().AddPending("en", null, "b"));
        }

        [Fact]
        public void LocalTextRegistry_AddPending_WorksProperly()
        {
            using (new MunqContext())
            {
                bool pending = true;
                
                var ctx = A.Fake<ILocalTextContext>();
                A.CallTo(() => ctx.IsApprovalMode).ReturnsLazily(() => pending);

                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance<ILocalTextContext>(ctx);

                var registry = new LocalTextRegistry();
                registry.AddPending("es", "key", "translation");

                Assert.Equal("translation", registry.TryGet("es", "key"));

                pending = false;
                Assert.Null(registry.TryGet("es", "key"));
            }
        }

        [Fact]
        public void LocalTextRegistry_AddPending_DoesntTrimAny()
        {
            using (new MunqContext())
            {
                bool pending = true;

                var ctx = A.Fake<ILocalTextContext>();
                A.CallTo(() => ctx.IsApprovalMode).ReturnsLazily(() => pending);

                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance<ILocalTextContext>(ctx);

                var registry = new LocalTextRegistry();
                registry.Add("  es  ", " key ", " translation ");

                var actual = registry.TryGet("  es  ", " key ");
                Assert.Equal(" translation ", actual);
            }
        }

        [Fact]
        public void LocalTextRegistry_AddPending_OverridesExisting()
        {
            using (new MunqContext())
            {
                bool pending = true;

                var ctx = A.Fake<ILocalTextContext>();
                A.CallTo(() => ctx.IsApprovalMode).ReturnsLazily(() => pending);

                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance<ILocalTextContext>(ctx);

                var registry = new LocalTextRegistry();
                registry.AddPending("es", "key", "oldTranslation");

                Assert.Equal("oldTranslation", registry.TryGet("es", "key"));

                registry.AddPending("es", "key", "newTranslation");

                Assert.Equal("newTranslation", registry.TryGet("es", "key"));
            }
        }

        [Fact]
        public void LocalTextRegistry_AddPending_DoesntOverrideApprovedText()
        {
            using (new MunqContext())
            {
                bool pending = true;

                var ctx = A.Fake<ILocalTextContext>();
                A.CallTo(() => ctx.IsApprovalMode).ReturnsLazily(() => pending);

                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance<ILocalTextContext>(ctx);

                var registry = new LocalTextRegistry();
                registry.Add("es", "key", "approvedTranslation");
                Assert.Equal("approvedTranslation", registry.TryGet("es", "key"));

                registry.AddPending("es", "key", "pendingTranslation1");
                Assert.Equal("pendingTranslation1", registry.TryGet("es", "key"));

                pending = false;

                Assert.Equal("approvedTranslation", registry.TryGet("es", "key"));

                pending = true;

                registry.AddPending("es", "key", "pendingTranslation2");
                Assert.Equal("pendingTranslation2", registry.TryGet("es", "key"));

                pending = false;

                Assert.Equal("approvedTranslation", registry.TryGet("es", "key"));
            }
        }
    }
}