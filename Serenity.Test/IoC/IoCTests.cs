using Serenity.Data;
using Serenity.Test.Testing;
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
            Assert.Throws<NullReferenceException>(() => IoC.Resolve<ISupportAttached>());
            Assert.Throws<NullReferenceException>(() => IoC.RegisterInstance<ISupportAttached>(new TestRow()));

            using (var context = IoC.StartContext())
            {
                IoC.RegisterInstance<ISupportAttached>(new TestRow());
            }

            Assert.Throws<NullReferenceException>(() => IoC.Resolve<ISupportAttached>());
            Assert.Throws<NullReferenceException>(() => IoC.RegisterInstance<ISupportAttached>(new TestRow()));
        }
    }
}