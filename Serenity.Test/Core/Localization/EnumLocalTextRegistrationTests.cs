using FakeItEasy;
using Serenity.Abstractions;
using Serenity.ComponentModel;
using Serenity.Localization;
using Serenity.Testing;
using System.Collections.Generic;
using System.ComponentModel;
using Xunit;

namespace Serenity.Test
{
#if !ASPNETCORE
    [Collection("AvoidParallel")]
    public class EnumLocalTextRegistrationTests
    {
        [EnumKey("My.CoolEnumKey")]
        public enum EnumWithKey
        {
            NoDescriptionKey,
            [Description("Description for WithDescriptionKey")]
            WithDescriptionKey
        }

        public enum EnumWithoutKey
        {
            [Description("Description for WithDescriptionNoKey")]
            WithDescriptionNoKey,
            NoDescriptionNoKey,
        }

        [Fact]
        public void EnumLocalTexts_Initialize_ThrowsException_IfNoLocalTextRegistryIsRegistered()
        {
            using (new MunqContext())
            {
                var exception = Assert.Throws<KeyNotFoundException>(() => 
                    EnumLocalTextRegistration.Initialize(new[] { this.GetType().Assembly }));

                Assert.Contains(typeof(ILocalTextRegistry).Name, exception.Message);
            }
        }

        [Fact]
        public void EnumLocalTexts_Initialize_UsesRegisteredLocalTextRegistry()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();

                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance(registry);

                EnumLocalTextRegistration.Initialize(new[] { this.GetType().Assembly });

                A.CallTo(() => registry.Add(A<string>._, A<string>._, A<string>._))
                    .MustHaveHappened(Repeated.AtLeast.Twice);
            }
        }

        [Fact]
        public void EnumLocalTexts_Initialize_SearchesOnlyGivenAssemblies()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();

                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance(registry);

                EnumLocalTextRegistration.Initialize(new[] { typeof(LocalText).Assembly });

                A.CallTo(() => registry.Add(A<string>._, A<string>.That.Contains("EnumWithoutKey"), A<string>._))
                    .MustNotHaveHappened();
            }
        }

        [Fact]
        public void EnumLocalTexts_Initialize_UsesFullName_IfEnumKeyAttributeIsNotPresent()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();

                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance(registry);

                EnumLocalTextRegistration.Initialize(new[] { this.GetType().Assembly });

                string expectedKey = "Enums." + typeof(EnumWithoutKey).FullName + "." + 
                    EnumWithoutKey.WithDescriptionNoKey.GetName();

                A.CallTo(() => registry.Add(A<string>._, expectedKey, "Description for WithDescriptionNoKey"))
                    .MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Fact]
        public void EnumLocalTexts_Initialize_UsesKey_IfKeyAttributeIsPresent()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();

                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance(registry);

                EnumLocalTextRegistration.Initialize(new[] { this.GetType().Assembly });

                string expectedKey = "Enums.My.CoolEnumKey." +
                    EnumWithKey.WithDescriptionKey.GetName();

                A.CallTo(() => registry.Add(A<string>._, expectedKey, "Description for WithDescriptionKey"))
                    .MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Fact]
        public void EnumLocalTexts_Initialize_SkipsEnumValuesWithoutDescriptionAttribute()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();

                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance(registry);

                EnumLocalTextRegistration.Initialize(new[] { this.GetType().Assembly });

                string unexpectedKey1 = "Enums.My.CoolEnumKey." +
                    EnumWithKey.NoDescriptionKey.GetName();

                string unexpectedKey2 = "Enums." + typeof(EnumWithoutKey).FullName + "." +
                    EnumWithoutKey.NoDescriptionNoKey.GetName();

                A.CallTo(() => registry.Add(A<string>._, unexpectedKey1, A<string>._))
                    .MustNotHaveHappened();

                A.CallTo(() => registry.Add(A<string>._, unexpectedKey2, A<string>._))
                    .MustNotHaveHappened();
            }
        }

        [Fact]
        public void EnumLocalTexts_Initialize_UsesInvariantLanguageIdByDefault()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();

                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance(registry);

                EnumLocalTextRegistration.Initialize(new[] { this.GetType().Assembly });

                A.CallTo(() => registry.Add("", A<string>._, A<string>._))
                    .MustHaveHappened();

                A.CallTo(() => registry.Add(A<string>.That.Not.IsEqualTo(""), A<string>._, A<string>._))
                    .MustNotHaveHappened();
            }
        }

        [Fact]
        public void EnumLocalTexts_Initialize_UsesLanguageSpecified()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();

                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance(registry);

                EnumLocalTextRegistration.Initialize(new[] { this.GetType().Assembly }, "es");

                A.CallTo(() => registry.Add("es", A<string>._, A<string>._))
                    .MustHaveHappened();

                A.CallTo(() => registry.Add(A<string>.That.Not.IsEqualTo("es"), A<string>._, A<string>._))
                    .MustNotHaveHappened();
            }
        }

    }
#endif
}