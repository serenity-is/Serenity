using Serenity.Abstractions;
using Serenity.ComponentModel;
using Serenity.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Serenity.Logging
{
    public class FileLogger : ILogger, IDisposable
    {
        private string file;
        private Queue<string> queue;
        private DateTime lastFlush;
        private DateTime writerDate;
        private TimeSpan flushTimeout;
        private object sync = new object();
        private StreamWriter stream;

        public FileLogger(LogSettings log = null)
        {
            queue = new Queue<string>();
            lastFlush = DateTimeProvider.Now;
            
            var settings = Config.TryGet<LogSettings>() ?? new LogSettings();
            File = string.IsNullOrEmpty(settings.File) ? null : settings.File;
            FlushTimeout = TimeSpan.FromSeconds(settings.FlushTimeout);
        }

        ~FileLogger()
        {
            Dispose(false);
        }

        public TimeSpan FlushTimeout
        {
            get { return flushTimeout; }
            set { flushTimeout = value; }
        }

        public string File
        {
            get { return file; }
            set 
            {
                if (file != value)
                {
                    file = value;

#if COREFX
                    string baseDirectory = AppContext.BaseDirectory;
#else
                    string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
#endif

                    if (file != null && (file.StartsWith("~\\") || file.StartsWith("~/")))
                        file = Path.Combine(baseDirectory, file.Substring(2));
                }
            }
        }

        public static string RandomFileCode()
        {
            Guid guid = Guid.NewGuid();
            var guidBytes = guid.ToByteArray();
            var eightBytes = new byte[8];
            for (int i = 0; i < 8; i++)
                eightBytes[i] = (byte)(guidBytes[i] ^ guidBytes[i + 8]);
            return Base32.Encode(eightBytes);
        }

        [SettingScope("Application"), SettingKey("Logging"), Ignore]
        public class LogSettings
        {
            public string File { get; set; }
            public int FlushTimeout { get; set; }
        }

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

                    ex.Log();
                }
            }

            lastFlush = DateTimeProvider.Now;
        }

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
                    ex.Log();
                }
            }

            if (disposing)
                GC.SuppressFinalize(this);
        }

        private static string[] SeverityChar = new string[] { "[V] ", "[D] ", "[I] ", "[W] ", "[E] ", "[F] " };

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

                    sb.Append(DateTimeProvider.Now.ToString(DateHelper.ISODateTimeFormatLocal));
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

                    var currentDate = DateTimeProvider.Now.Date;
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
#if COREFX
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
                            stream = new StreamWriter(System.IO.File.OpenWrite(newFile), Encoding.UTF8);
                        }
                        else
                            stream = new StreamWriter(System.IO.File.OpenWrite(newFile), Encoding.UTF8);
                    }

                    queue.Enqueue(sb.ToString());

                    if (flushTimeout <= TimeSpan.Zero ||
                        flushTimeout > TimeSpan.Zero && (DateTimeProvider.Now - lastFlush) >= flushTimeout)
                        InternalFlush();
                }
#if COREFX
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