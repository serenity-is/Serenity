#if !NET
using Serenity.Abstractions;
using Serenity.ComponentModel;
using Serenity.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
#if !NET45
using Microsoft.Extensions.Internal;
using DateTime = System.DateTimeOffset;
#endif

namespace Serenity.Logging
{
    /// <summary>
    /// A simple file logger implementation
    /// </summary>
    /// <seealso cref="Serenity.Abstractions.ILogger" />
    /// <seealso cref="System.IDisposable" />
    public class FileLogger : ILogger, IDisposable
    {
        private string file;
        private Queue<string> queue;
        private DateTime lastFlush;
        private DateTime writerDate;
        private TimeSpan flushTimeout;
        private object sync = new object();
        private StreamWriter stream;

#if NET45
        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogger"/> class.
        /// </summary>
        /// <param name="log">The log.</param>
        public FileLogger(LogSettings log = null)
        {
#else
        private IExceptionLogger exceptionLogger;
        private ISystemClock clock;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogger"/> class.
        /// </summary>
        /// <param name="clock">The system clock</param>
        /// <param name="logSettings">The log settings.</param>
        /// <param name="exceptionLogger">Exception logger</param>
        public FileLogger(ISystemClock clock, LogSettings logSettings = null, IExceptionLogger exceptionLogger = null)
        {
            this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
            this.exceptionLogger = exceptionLogger;
#endif
            queue = new Queue<string>();
#if NET45
            lastFlush = DateTimeProvider.Now;
#else
            lastFlush = clock.UtcNow;
#endif

#if NET45
            var settings = log ?? Config.TryGet<LogSettings>() ?? new LogSettings();
#else
            var settings = logSettings ?? throw new ArgumentNullException(nameof(logSettings));
#endif
            File = string.IsNullOrEmpty(settings.File) ? null : settings.File;
            FlushTimeout = TimeSpan.FromSeconds(settings.FlushTimeout);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="FileLogger"/> class.
        /// </summary>
        ~FileLogger()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets or sets the flush timeout.
        /// </summary>
        /// <value>
        /// The flush timeout.
        /// </value>
        public TimeSpan FlushTimeout
        {
            get { return flushTimeout; }
            set { flushTimeout = value; }
        }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>
        /// The file.
        /// </value>
        public string File
        {
            get { return file; }
            set 
            {
                if (file != value)
                {
                    file = value;

#if !NET45
                    string baseDirectory = AppContext.BaseDirectory;
#else
                    string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
#endif

                    if (file != null && (file.StartsWith("~\\") || file.StartsWith("~/")))
                        file = Path.Combine(baseDirectory, file.Substring(2));
                }
            }
        }

        /// <summary>
        /// Generates a random file code.
        /// </summary>
        /// <returns></returns>
        public static string RandomFileCode()
        {
            Guid guid = Guid.NewGuid();
            var guidBytes = guid.ToByteArray();
            var eightBytes = new byte[8];
            for (int i = 0; i < 8; i++)
                eightBytes[i] = (byte)(guidBytes[i] ^ guidBytes[i + 8]);
            return Base32.Encode(eightBytes);
        }

        /// <summary>
        /// File logger settings
        /// </summary>
        [SettingScope("Application"), SettingKey("Logging"), Ignore]
        public class LogSettings
        {
            /// <summary>
            /// Gets or sets the file name.
            /// </summary>
            /// <value>
            /// The file.
            /// </value>
            public string File { get; set; }

            /// <summary>
            /// Gets or sets the flush timeout.
            /// </summary>
            /// <value>
            /// The flush timeout.
            /// </value>
            public int FlushTimeout { get; set; }
        }

        /// <summary>
        /// Flushes this instance.
        /// </summary>
        public void Flush()
        {
            InternalFlush();
        }

        internal void InternalFlush()
        {
            if (stream != null)
            {
                try
                {
                    while (queue.Count > 0)
                        stream.Write(queue.Dequeue());

                    queue.Clear();
                    stream.Flush();
                }
                catch (Exception ex)
                {
                    if (queue != null)
                        queue.Clear();

#if NET45
                    ex.Log();
#else
                    ex.Log(exceptionLogger);
#endif
                }
            }

#if NET45
            lastFlush = DateTimeProvider.Now;
#else
            lastFlush = clock.UtcNow;
#endif
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
                try
                {
                    InternalFlush();

                    if (stream != null)
                    {
                        stream.Dispose();
                        stream = null;
                    }
                }
                catch (Exception ex)
                {
                    stream = null;
#if NET45
                    ex.Log();
#else
                    ex.Log(exceptionLogger);
#endif
                }
            }

            if (disposing)
                GC.SuppressFinalize(this);
        }

        private static string[] SeverityChar = new string[] { "[V] ", "[D] ", "[I] ", "[W] ", "[E] ", "[F] " };

        /// <summary>
        /// Writes the specified level log message.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="source">The source.</param>
        public void Write(LoggingLevel level, string message, Exception exception, Type source)
        {
            lock (sync)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    if (level >= LoggingLevel.Verbose && level <= LoggingLevel.Fatal)
                        sb.Append(SeverityChar[(int)level + 6]);
                    else
                        sb.Append("[?]");

#if NET45
                    sb.Append(DateTimeProvider.Now.ToString(DateHelper.ISODateTimeFormatLocal));
#else
                    sb.Append(clock.UtcNow.ToLocalTime().ToString(DateHelper.ISODateTimeFormatLocal));

#endif
                    sb.Append(' ');

                    if (source != null)
                    {
                        sb.Append(source.FullName);
                        sb.Append(' ');
                    }

                    if (message != null)
                        sb.AppendLine(message);

                    if (exception != null)
                        sb.AppendLine(exception.ToString());

#if NET45
                    var currentDate = DateTimeProvider.Now.Date;
#else
                    var currentDate = clock.UtcNow.ToLocalTime().Date;
#endif
                    if (stream == null || writerDate != currentDate)
                    {
                        writerDate = currentDate;
                        string newFile = String.Format(file, writerDate.ToString("yyyyMMdd"), RandomFileCode());

                        Directory.CreateDirectory(Path.GetDirectoryName(newFile));

                        if (stream != null)
                        {
                            InternalFlush();

                            try
                            {
                                stream.Dispose();
                            }
#if !NET45
                            catch
                            {
                            }
#else
                            catch (Exception ex1)
                            {
                                var internalLogger = Dependency.TryResolve<ILogger>("Internal");
                                if (internalLogger != null)
                                    internalLogger.Write(LoggingLevel.Fatal, null, ex1, this.GetType());
                            }
#endif

                            stream = null;
                            stream = new StreamWriter(System.IO.File.Open(newFile, FileMode.OpenOrCreate, 
                                FileAccess.Write, FileShare.Read), Encoding.UTF8);
                        }
                        else
                            stream = new StreamWriter(System.IO.File.Open(newFile, FileMode.OpenOrCreate, 
                                FileAccess.Write, FileShare.Read), Encoding.UTF8);
                    }

                    queue.Enqueue(sb.ToString());

#if NET45
                    if (flushTimeout <= TimeSpan.Zero ||
                        flushTimeout > TimeSpan.Zero && (DateTimeProvider.Now - lastFlush) >= flushTimeout)
#else
                    if (flushTimeout <= TimeSpan.Zero ||
                        flushTimeout > TimeSpan.Zero && (clock.UtcNow - lastFlush) >= flushTimeout)
#endif
                        InternalFlush();
                }
#if !NET45
                catch
                {
                }
#else
                catch (Exception ex2)
                {
                    var internalLogger = Dependency.TryResolve<ILogger>("Internal");
                    if (internalLogger != null)
                        internalLogger.Write(LoggingLevel.Fatal, null, ex2, this.GetType());
                }
#endif
            }
        }
    }
}
#endif