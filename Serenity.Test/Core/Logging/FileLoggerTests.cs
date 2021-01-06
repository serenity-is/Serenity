using FakeItEasy;
using Serenity.Abstractions;
using Serenity.Logging;
using Serenity.Testing;
using Xunit;

namespace Serenity.Test.Logging
{
    [Collection("AvoidParallel")]
    public partial class FileLoggerTests
    {
        [Fact]
        public void ItIsDisabledIfNoConfigProvided()
        {
            var logger = new FileLogger();
            logger.Write(LoggingLevel.Fatal, "test", null, null);
            logger.Flush();
        }

        [Fact]
        public void ItIsDisabledIfNullFileSettingIsProvided()
        {
            var logger = new FileLogger(new FileLogger.LogSettings { File = null });
            logger.Write(LoggingLevel.Fatal, "test", null, null);
            logger.Flush();
        }

        [Fact]
        public void ItIsDisabledIfEmptyFileSettingIsProvided()
        {
            var logger = new FileLogger(new FileLogger.LogSettings { File = "" });
            logger.Write(LoggingLevel.Fatal, "test", null, null);
            logger.Flush();
        }

        [Fact]
        public void ItUsesLogSettingsIfPassedInConstructor()
        {
            var tempFile = System.IO.Path.GetTempFileName();
            try
            {
                using (var logger = new FileLogger(new FileLogger.LogSettings { File = tempFile }))
                {
                    logger.Write(LoggingLevel.Fatal, "test13579", null, null);
                }

                Assert.Contains("test13579", System.IO.File.ReadAllText(tempFile));
            }
            finally
            {
                System.IO.File.Delete(tempFile);
            }
        }

        [Fact]
        public void ItUsesConfigIfNullPassedInConstructor()
        {
            var tempFile = System.IO.Path.GetTempFileName();
            try
            {
                using (new MunqContext())
                {
                    var registrar = Dependency.Resolve<IDependencyRegistrar>();
                    var config = A.Fake<IConfigurationRepository>();
                    A.CallTo(() => config.Load(typeof(FileLogger.LogSettings))).Returns(new FileLogger.LogSettings
                    {
                        File = tempFile
                    });

                    registrar.RegisterInstance<IConfigurationRepository>(config);

                    using (var logger = new FileLogger(new FileLogger.LogSettings { File = tempFile }))
                    {
                        logger.Write(LoggingLevel.Fatal, "test24680", null, null);
                    }

                    Assert.Contains("test24680", System.IO.File.ReadAllText(tempFile));
                }
            }
            finally
            {
                System.IO.File.Delete(tempFile);
            }
        }
    }
}