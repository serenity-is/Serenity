using Serenity.Data;
using Serenity.Test.Testing;
using Serenity.Testing;
using System;
using Xunit;

namespace Serenity.Test
{
    public partial class IoCTests
    {
        [Fact]
        public void IoCThrowsInvalidProgramExceptionIfNoContextStarted()
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