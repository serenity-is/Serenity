using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using Serenity.Data;

namespace Serenity
{
    public static class Log
    {
        private static LoggingLevel _level;
        private static string _file;
        private static StreamWriter _stream;
        private static DateTime _writerDate;
        private static int _flushTimeout;
        private static object _lock = new object();
        private static List<string> _queue;
        private static DateTime _lastFlush;

        static Log()
        {
            try
            {
                var settings = JSON.ParseTolerant<LogSettings>(ConfigurationManager.AppSettings["Logging"].TrimToNull() ?? "{}");
                _level = settings.Level;
                _file = settings.File;
                _queue = new List<string>();

                if (_file != null && _file.StartsWith("~\\"))
                    _file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _file.Substring(2));

                _lastFlush = DateTime.Now;
                _flushTimeout = settings.FlushTimeout;
            }
            catch (Exception ex)
            {
                ex.Log();
                _level = LoggingLevel.Off;
            }
        }

        internal static void InternalFlush()
        {
            if (_stream != null)
            {
                try
                {
                    foreach (var s in _queue)
                        _stream.Write(s);
                    _queue.Clear();
                    _stream.Flush();
                }
                catch (Exception ex)
                {
                    if (_queue != null)
                        _queue.Clear();

                    ex.Log();
                }
            }

            _lastFlush = DateTime.Now;
        }

        public static void Dispose()
        {
            lock (_lock)
            {
                try
                {
                    InternalFlush();

                    if (_stream != null)
                    {
                        _stream.Close();
                        _stream = null;
                    }
                }
                catch (Exception ex)
                {
                    _stream = null;
                    ex.Log();
                }
            }
        }

        public static bool IsLevel(LoggingLevel level)
        {
            return _level <= level;
        }

        private static string[] SeverityChar = new string[] { "[D] ", "[I] ", "[W] ", "[E] " };

        public static void Write(LoggingLevel level, string message)
        {
            if (_level <= level && level <= LoggingLevel.Error && level >= LoggingLevel.Debug)
            {
                Exception exception = null;

                lock (_lock)
                {
                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(SeverityChar[(int)level + 4]);
                        sb.Append(DateTime.Now.ToString(DateHelper.ISODateTimeFormatLocal));
                        sb.Append(' ');
                        sb.AppendLine(message);

                        if (_stream == null || _writerDate != DateTime.Today)
                        {
                            _writerDate = DateTime.Today;
                            string newFile = String.Format(_file, _writerDate.ToString("yyyyMMdd"), TemporaryFileHelper.RandomFileCode());

                            Directory.CreateDirectory(Path.GetDirectoryName(newFile));

                            if (_stream != null)
                            {
                                InternalFlush();

                                try
                                {
                                    _stream.Close();
                                }
                                catch (Exception exx)
                                {
                                    exception = exx;
                                }

                                _stream = null;
                                _stream = new StreamWriter(newFile, true, Encoding.UTF8);
                            }
                            else
                                _stream = new StreamWriter(newFile, true, Encoding.UTF8);
                        }

                        _queue.Add(sb.ToString());

                        if (_flushTimeout == 0 ||
                            _flushTimeout > 0 && (DateTime.Now - _lastFlush).TotalMilliseconds >= _flushTimeout)
                            InternalFlush();
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                }

                if (exception != null)
                    exception.Log();
            }
        }

        public static bool IsEnabled
        {
            get { return _level > LoggingLevel.Off; }
        }

        public static bool DebugLevel
        {
            get { return _level <= LoggingLevel.Debug; }
        }

        public static void Debug(string message)
        {
            Write(LoggingLevel.Debug, message);
        }

        public static bool InfoLevel
        {
            get { return _level <= LoggingLevel.Info; }
        }

        public static void Info(string message)
        {
            Write(LoggingLevel.Info, message);
        }

        public static bool WarnLevel
        {
            get { return _level <= LoggingLevel.Warn; }
        }

        public static void Warn(string message)
        {
            Write(LoggingLevel.Warn, message);
        }

        public static bool ErrorLevel
        {
            get { return _level <= LoggingLevel.Error; }
        }

        public static void Error(string message)
        {
            Write(LoggingLevel.Error, message);
        }

        private class LogSettings
        {
            public LoggingLevel Level { get; set; }
            public string File { get; set; }
            public int FlushTimeout { get; set; }
        }
    }
}