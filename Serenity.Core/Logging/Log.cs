using Serenity.Abstractions;
using Serenity.ComponentModel;
using Serenity.Logging;
using System;

namespace Serenity
{
    public static class Log
    {
        private static LoggingLevel minimumLevel;

        static Log()
        {
            var settings = Config.TryGet<LogSettings>() ?? new LogSettings();
            minimumLevel = settings.Level;
        }

        public static LoggingLevel MinimumLevel
        {
            get { return minimumLevel; }
            set { minimumLevel = value; }
        }

        public static bool IsLevelEnabled(LoggingLevel level)
        {
            return minimumLevel <= level;
        }

        public static bool IsEnabled
        {
            get { return minimumLevel < LoggingLevel.Off; }
        }

        public static bool IsFatalEnabled
        {
            get { return minimumLevel <= LoggingLevel.Fatal; }
        }

        public static void Fatal(string message)
        {
            Write(LoggingLevel.Fatal, message, null, null);
        }

        public static void Fatal(string message, Type source)
        {
            Write(LoggingLevel.Fatal, message, null, source);
        }

        public static void Fatal(string message, Exception exception, Type source)
        {
            Write(LoggingLevel.Fatal, message, exception, source);
        }

        public static bool IsErrorEnabled
        {
            get { return minimumLevel <= LoggingLevel.Error; }
        }

        public static void Error(string message)
        {
            Write(LoggingLevel.Error, message, null, null);
        }

        public static void Error(string message, Type source)
        {
            Write(LoggingLevel.Error, message, null, source);
        }

        public static void Error(string message, Exception exception, Type source)
        {
            Write(LoggingLevel.Error, message, exception, source);
        }

        public static bool IsWarnEnabled
        {
            get { return minimumLevel <= LoggingLevel.Warn; }
        }

        public static void Warn(string message)
        {
            Write(LoggingLevel.Warn, message, null, null);
        }

        public static void Warn(string message, Type source)
        {
            Write(LoggingLevel.Warn, message, null, source);
        }

        public static void Warn(string message, Exception exception, Type source)
        {
            Write(LoggingLevel.Warn, message, exception, source);
        }

        public static bool IsInfoEnabled
        {
            get { return minimumLevel <= LoggingLevel.Info; }
        }

        public static void Info(string message)
        {
            Write(LoggingLevel.Info, message, null, null);
        }

        public static void Info(string message, Type source)
        {
            Write(LoggingLevel.Info, message, null, source);
        }

        public static void Info(string message, Exception exception, Type source)
        {
            Write(LoggingLevel.Info, message, exception, source);
        }

        public static bool IsDebugEnabled
        {
            get { return minimumLevel <= LoggingLevel.Debug; }
        }

        [Obsolete("Use IsDebugEnabled")]
        public static bool DebugLevel
        {
            get { return minimumLevel <= LoggingLevel.Debug; }
        }

        public static void Debug(string message)
        {
            Write(LoggingLevel.Debug, message, null, null);
        }

        public static void Debug(string message, Type source)
        {
            Write(LoggingLevel.Debug, message, null, source);
        }

        public static void Debug(string message, Exception exception, Type source)
        {
            Write(LoggingLevel.Debug, message, exception, source);
        }

        public static bool IsVerboseEnabled
        {
            get { return minimumLevel <= LoggingLevel.Verbose; }
        }

        public static void Verbose(string message)
        {
            Write(LoggingLevel.Verbose, message, null, null);
        }

        public static void Verbose(string message, Type source)
        {
            Write(LoggingLevel.Verbose, message, null, source);
        }

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
#if ASPNETCORE
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