using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Serenity.Web.SpaServices;

public class LoggerFinderTests
{
    [Fact]
    public void GetOrCreateLogger_Uses_LoggerFactory_When_Available()
    {
        var loggerFactory = new MockLoggerFactory();
        var app = new ApplicationBuilder(new ServiceCollection()
            .AddSingleton<ILoggerFactory>(loggerFactory)
            .BuildServiceProvider());

        var logger = LoggerFinder.GetOrCreateLogger(app, "Test.Category");

        Assert.NotNull(logger);
        Assert.Equal("Test.Category", loggerFactory.LastCategoryName);
    }

    [Fact]
    public void GetOrCreateLogger_Returns_NullLogger_When_Not_Available()
    {
        var app = new ApplicationBuilder(new ServiceCollection().BuildServiceProvider());

        var logger = LoggerFinder.GetOrCreateLogger(app, "Test.Category");

        Assert.Same(NullLogger.Instance, logger);
    }

    private class MockLoggerFactory : ILoggerFactory
    {
        public string? LastCategoryName { get; private set; }

        public void AddProvider(ILoggerProvider provider)
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            LastCategoryName = categoryName;
            return NullLogger.Instance;
        }

        public void Dispose()
        {
        }
    }
}
