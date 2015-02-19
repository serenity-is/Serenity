using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serenity.Abstractions;
using Serenity.ComponentModel;
using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Serenity.Logging
{
    public class SqlLogger : ILogger
    {
        private object sync = new object();

        public SqlLogger()
        {
            var settings = Config.TryGet<LogSettings>() ?? new LogSettings();
            ConnectionKey = settings.ConnectionKey;
            InsertCommand = settings.InsertCommand;
        }

        public string ConnectionKey { get; set; }
        public string InsertCommand { get; set; }

        [SettingScope("Application"), SettingKey("Logging"), Hidden]
        private class LogSettings
        {
            public string ConnectionKey { get; set; }
            public string InsertCommand { get; set; }
        }

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

            ThreadPool.QueueUserWorkItem(w =>
            {
                try
                {
                    using (var connection = SqlConnections.NewByKey(connectionKey))
                    {
                        connection.Execute(insertCommand, parameters);
                    }
                }
                catch (Exception ex)
                {
                    var internalLogger = Dependency.TryResolve<ILogger>("Internal");
                    if (internalLogger != null)
                        internalLogger.Write(LoggingLevel.Fatal, null, ex, this.GetType());
                }
            });
        }
    }
}