using Serenity.Abstractions;
using Serenity.ComponentModel;
using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Threading;
#if !NET45
using DynamicParameters = Dapper.DynamicParameters;
#endif
namespace Serenity.Logging
{
    /// <summary>
    /// A simple logger that logs to an SQL table.
    /// </summary>
    /// <seealso cref="Serenity.Abstractions.ILogger" />
    /// <seealso cref="System.IDisposable" />
    public class SqlLogger : ILogger, IDisposable
    {
        private object sync = new object();
        private int inProgress;
        private Queue<DynamicParameters> queue = new Queue<DynamicParameters>();
        private AutoResetEvent signal = new AutoResetEvent(false);

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLogger"/> class.
        /// </summary>
        public SqlLogger()
        {
            var settings = Config.TryGet<LogSettings>() ?? new LogSettings();
            ConnectionKey = settings.ConnectionKey;
            InsertCommand = settings.InsertCommand;
            new Thread(Worker).Start();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SqlLogger"/> class.
        /// </summary>
        ~SqlLogger()
        {
            Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            lock (sync)
            {
                queue = null;
            }

            if (disposing)
                GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Flushes this instance.
        /// </summary>
        public void Flush()
        {
            while (true)
            {
                lock (sync)
                {
                    if (inProgress == 0 && (queue == null || queue.Count == 0))
                        return;
                }
                Thread.Sleep(1);
            }
        }

        private void Worker()
        {
            while (queue != null)
            {
                signal.WaitOne();

                if (queue == null)
                    break;

                while (true)
                try
                {
                    DynamicParameters parameters;
                    lock (sync)
                    {
                        if (queue == null || queue.Count == 0)
                            break;

                        parameters = queue.Dequeue();
                        inProgress++;
                    }

                    try
                    {
                        using (var connection = SqlConnections.NewByKey(ConnectionKey))
                        {
                            connection.Execute(InsertCommand, parameters);
                        }
                    }
                    finally
                    {
                        inProgress--;
                    }
                }
#if !NET45
                catch
                {

                }
#else
                catch (Exception ex)
                {
                    var internalLogger = Dependency.TryResolve<ILogger>("Internal");
                    if (internalLogger != null)
                        internalLogger.Write(LoggingLevel.Fatal, null, ex, this.GetType());
                }
#endif
            }
        }

        /// <summary>
        /// Gets or sets the connection key.
        /// </summary>
        /// <value>
        /// The connection key.
        /// </value>
        public string ConnectionKey { get; set; }

        /// <summary>
        /// Gets or sets the insert command.
        /// </summary>
        /// <value>
        /// The insert command.
        /// </value>
        public string InsertCommand { get; set; }

        [SettingScope("Application"), SettingKey("Logging"), Ignore]
        private class LogSettings
        {
            public string ConnectionKey { get; set; }
            public string InsertCommand { get; set; }
        }

        /// <summary>
        /// Writes the specified level log message.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="source">The source.</param>
        public void Write(LoggingLevel level, string message, Exception exception, Type source)
        {
            var connectionKey = ConnectionKey;
            var insertCommand = InsertCommand;
            var parameters = new DynamicParameters(new Dictionary<string, object>()
            {
                { "@date", DateTime.Now },
                { "@utcdate", DateTime.UtcNow },
                { "@level", EnumMapper.GetName(level) },
                { "@message", message },
                { "@exception", exception != null ? exception.ToString() : null },
                { "@source", source != null ? source.FullName : null },
                { "@thread", Thread.CurrentThread.ManagedThreadId }
            });

            lock (sync)
                queue.Enqueue(parameters);

            signal.Set();
        }
    }
}