using Serenity.Abstractions;
using Serenity.ComponentModel;
using Serenity.Logging;
using System;

namespace Serenity
{
    /// <summary>
    /// Static class with log helper functions
    /// </summary>
    public static class Log
    {
        private static LoggingLevel minimumLevel;

        static Log()
        {
            var settings = Config.TryGet<LogSettings>() ?? new LogSettings();
            minimumLevel = settings.Level;
        }

        /// <summary>
        /// Gets or sets the minimum level for logging.
        /// </summary>
        /// <value>
        /// The minimum level.
        /// </value>
        public static LoggingLevel MinimumLevel
        {
            get { return minimumLevel; }
            set { minimumLevel = value; }
        }

        /// <summary>
        /// Determines whether logging is enabled for specified level
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsLevelEnabled(LoggingLevel level)
        {
            return minimumLevel <= level;
        }

        /// <summary>
        /// Gets a value indicating whether this logging is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public static bool IsEnabled
        {
            get { return minimumLevel < LoggingLevel.Off; }
        }

        /// <summary>
        /// Gets a value indicating whether fatal level is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public static bool IsFatalEnabled
        {
            get { return minimumLevel <= LoggingLevel.Fatal; }
        }

        /// <summary>
        /// Logs a fatal level message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Fatal(string message)
        {
            Write(LoggingLevel.Fatal, message, null, null);
        }

        /// <summary>
        /// Logs a fatal level message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="source">The source type.</param>
        public static void Fatal(string message, Type source)
        {
            Write(LoggingLevel.Fatal, message, null, source);
        }

        /// <summary>
        /// Logs a fatal level message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="source">The source type.</param>
        public static void Fatal(string message, Exception exception, Type source)
        {
            Write(LoggingLevel.Fatal, message, exception, source);
        }


        /// <summary>
        /// Gets a value indicating whether error level logging is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public static bool IsErrorEnabled
        {
            get { return minimumLevel <= LoggingLevel.Error; }
        }

        /// <summary>
        /// Logs an error level message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Error(string message)
        {
            Write(LoggingLevel.Error, message, null, null);
        }

        /// <summary>
        /// Logs an error level message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="source">The source.</param>
        public static void Error(string message, Type source)
        {
            Write(LoggingLevel.Error, message, null, source);
        }

        /// <summary>
        /// Logs an error level message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="source">The source.</param>
        public static void Error(string message, Exception exception, Type source)
        {
            Write(LoggingLevel.Error, message, exception, source);
        }

        /// <summary>
        /// Gets a value indicating whether warning level logging is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public static bool IsWarnEnabled
        {
            get { return minimumLevel <= LoggingLevel.Warn; }
        }

        /// <summary>
        /// Logs a warning level message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Warn(string message)
        {
            Write(LoggingLevel.Warn, message, null, null);
        }

        /// <summary>
        /// Logs a warning level message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="source">The source.</param>
        public static void Warn(string message, Type source)
        {
            Write(LoggingLevel.Warn, message, null, source);
        }

        /// <summary>
        /// Logs a warning level message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="source">The source.</param>
        public static void Warn(string message, Exception exception, Type source)
        {
            Write(LoggingLevel.Warn, message, exception, source);
        }

        /// <summary>
        /// Gets a value indicating whether info level logging is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public static bool IsInfoEnabled
        {
            get { return minimumLevel <= LoggingLevel.Info; }
        }

        /// <summary>
        /// Logs an information level message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Info(string message)
        {
            Write(LoggingLevel.Info, message, null, null);
        }

        /// <summary>
        /// Logs an information level message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="source">The source.</param>
        public static void Info(string message, Type source)
        {
            Write(LoggingLevel.Info, message, null, source);
        }

        /// <summary>
        /// Logs an information level message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="source">The source.</param>
        public static void Info(string message, Exception exception, Type source)
        {
            Write(LoggingLevel.Info, message, exception, source);
        }

        /// <summary>
        /// Gets a value indicating whether debug level logging is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public static bool IsDebugEnabled
        {
            get { return minimumLevel <= LoggingLevel.Debug; }
        }

        /// <summary>
        /// Gets a value indicating whether debug level logging is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        [Obsolete("Use IsDebugEnabled")]
        public static bool DebugLevel
        {
            get { return minimumLevel <= LoggingLevel.Debug; }
        }

        /// <summary>
        /// Logs a debug level message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Debug(string message)
        {
            Write(LoggingLevel.Debug, message, null, null);
        }

        /// <summary>
        /// Logs a debug level message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="source">The source.</param>
        public static void Debug(string message, Type source)
        {
            Write(LoggingLevel.Debug, message, null, source);
        }

        /// <summary>
        /// Logs a debug level message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="source">The source.</param>
        public static void Debug(string message, Exception exception, Type source)
        {
            Write(LoggingLevel.Debug, message, exception, source);
        }

        /// <summary>
        /// Gets a value indicating whether verbose level logging is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public static bool IsVerboseEnabled
        {
            get { return minimumLevel <= LoggingLevel.Verbose; }
        }

        /// <summary>
        /// Logs a verbose level message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Verbose(string message)
        {
            Write(LoggingLevel.Verbose, message, null, null);
        }

        /// <summary>
        /// Logs a verbose level message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="source">The source.</param>
        public static void Verbose(string message, Type source)
        {
            Write(LoggingLevel.Verbose, message, null, source);
        }

        /// <summary>
        /// Logs a verbose level message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="source">The source.</param>
        public static void Verbose(string message, Exception exception, Type source)
        {
            Write(LoggingLevel.Verbose, message, exception, source);
        }

        private static void Write(LoggingLevel level, string message, Exception exception, Type source)
        {
            if (minimumLevel > level)
                return;

            var logger = Dependency.TryResolve<ILogger>();
            if (logger == null)
                return;

            try
            {
                logger.Write(level, message, exception, source);
            }
#if COREFX
            catch
            {
            }
#else
            catch (Exception ex)
            {
                var internalLogger = Dependency.TryResolve<ILogger>("Internal");
                if (internalLogger != null)
                    internalLogger.Write(LoggingLevel.Fatal, null, ex, typeof(Log));
            }
#endif
        }

        /// <summary>
        /// Generates a random file code.
        /// </summary>
        /// <returns>Random file code</returns>
        [Obsolete("Use FileLogger.RandomFileCode")]
        public static string RandomFileCode()
        {
            return FileLogger.RandomFileCode();
        }

        [SettingScope("Application"), SettingKey("Logging")]
        private class LogSettings
        {
            public LoggingLevel Level { get; set; }
        }
    }
}