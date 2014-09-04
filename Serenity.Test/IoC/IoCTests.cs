using Serenity.Data;
using Serenity.Test.Testing;
using Serenity.Testing;
using System;
using Xunit;

namespace Serenity.Test
{
    public partial class IoCTests
    {
        /// <summary>
        /// This flag (IoCManualStartContext) should be set in app.config file to prevent errors in unit tests.
        /// As one test might start a context while other not, and depending on their order, the one that
        /// doesn't start a context might be success or not. So we decided to not create a context, unless
        /// explicitly started in a unit test environment.
        /// </summary>
        [Fact]
        public void IoCThrowsNullReferenceExceptionIfTestingModeAndNoContextStarted()
        {
            Assert.Throws<InvalidProgramException>(() => Dependency.Resolve<ISupportAttached>());
            Assert.Throws<InvalidProgramException>(() => Dependency.Resolve<IDependencyRegistrar>()
                .RegisterInstance<ISupportAttached>(new TestRow()));

            using (var context = new MunqContext())
            {
                Dependency.Resolve<IDependencyRegistrar>().RegisterInstance<ISupportAttached>(new TestRow());
            }

            Assert.Throws<InvalidProgramException>(() => Dependency.Resolve<ISupportAttached>());
            Assert.Throws<InvalidProgramException>(() => Dependency.Resolve<IDependencyRegistrar>()
                .RegisterInstance<ISupportAttached>(new TestRow()));
        }
    }
}