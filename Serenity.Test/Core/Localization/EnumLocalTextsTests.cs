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
    public partial class EnumLocalTextsTests
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
                    EnumLocalTexts.Initialize(new[] { this.GetType().Assembly }));

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

                EnumLocalTexts.Initialize(new[] { this.GetType().Assembly });

                A.CallTo(() => registry.Add(A<string>._, A<string>._, A<string>._))
                    .MustHaveHappened(Repeated.AtLeast.Twice);
            }
        }

        [Fact]
        public void EnumLocalTexts_Initialize_SearchesOnlyGivenAssemblies()
        {
        }
    }
}