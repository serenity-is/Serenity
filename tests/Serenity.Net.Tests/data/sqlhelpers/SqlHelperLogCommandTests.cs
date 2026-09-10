using Microsoft.Extensions.Logging;

namespace Serenity.Data;

public class SqlHelperLogCommandTests
{
    [Fact]
    public void LogCommand_NullLogger_ThrowsArgumentNullException()
    {
        using var connection = new MockDbConnection();
        using var command = connection.CreateCommand();

        Assert.Throws<ArgumentNullException>(() => SqlHelper.LogCommand("Test", command, null!));
    }

    [Fact]
    public void LogCommand_NullCommand_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => SqlHelper.LogCommand("Test", null!, new MockLogger()));
    }

    [Fact]
    public void LogCommand_WithDebugEnabledLogger_LogsCommandText()
    {
        using var connection = new MockDbConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT 1";
        var logger = new MockLogger();

        SqlHelper.LogCommand("Test", command, logger);

        Assert.Null(logger.LastException);
    }

    [Fact]
    public void LogCommand_WithDisabledLogger_DoesNotLog()
    {
        using var connection = new MockDbConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT 1";
        var logger = new DisabledLogger();

        SqlHelper.LogCommand("Test", command, logger);

        Assert.False(logger.Logged);
    }

    [Fact]
    public void LogCommand_WhenDumperThrows_LogsError()
    {
        using var connection = new MockDbConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT 1";
        command.Parameters.Add(new ThrowingValueParameter());
        var logger = new MockLogger();

        SqlHelper.LogCommand("Test", command, logger);

        Assert.NotNull(logger.LastException);
    }

    private class DisabledLogger : ILogger
    {
        public bool Logged { get; private set; }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => false;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Logged = true;
        }
    }

    private class ThrowingValueParameter : MockDbParameter
    {
        public override object Value => throw new InvalidOperationException("dump error");
    }
}
