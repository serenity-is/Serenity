using System;
using System.Text;
using Serenity.Data;
using Newtonsoft.Json;
using System.IO;

namespace Serenity
{
    public interface INotLoggedException
    {
        bool NotLoggedException { get; }
    }

    /// <summary>
    ///   Centralized logger for exceptions.</summary>
    public static class ExceptionLog
    {
        private static Action<Exception> _exceptionLogger;

        /// <summary>
        ///   Logs an exception. Nothing logged if exception logger delegate is not set.</summary>
        /// <param name="e"></param>
        public static void Log(this Exception e)
        {
            if (_exceptionLogger != null && e != null)
                try
                {
                    var no = e as INotLoggedException;
                    if (no == null ||
                        !no.NotLoggedException)
                        _exceptionLogger(e);
                }
                catch
                {
                }
        }

        /// <summary>
        ///   Sets the exception logger delegate</summary>
        /// <param name="exceptionLogger">
        ///   Exception logger delegate</param>
        public static void SetExceptionLogger(Action<Exception> exceptionLogger)
        {
            _exceptionLogger = exceptionLogger;
        }

        /// <summary>
        ///   Sets custom exception data with given property name and value. Sets the data in base exception.</summary>
        /// <param name="exception">
        ///   Exception to set custom data in.</param>
        /// <param name="property">
        ///   Custom exception data name.</param>
        /// <param name="value">
        ///   Custom exception data value.</param>
        public static void SetData(this Exception exception, string property, object value)
        {
            exception.GetBaseException().Data[property] = value;
        }

        /// <summary>
        ///   Tries to format additional exception data (only ones in form of string to be safe) in exception.Data collection.</summary>
        /// <param name="exception">
        ///   Exception.</param>
        /// <returns>
        ///   Formatted exception data.</returns>
        public static string FormatExceptionData(Exception exception)
        {
            if (exception != null)
                exception = exception.GetBaseException();

            if (exception != null && exception.Data.Keys.Count > 0)
            {                   
                bool hasCustomData = false;

                var sb = new StringBuilder();
                var jw = new JsonTextWriter(new StringWriter(sb));

                foreach (var key in exception.Data.Keys)
                    if (key is String)
                    {
                        if (!hasCustomData)
                        {
                            jw.WriteStartObject();
                            hasCustomData = true;
                        }

                        var value = exception.Data[key];
                        try
                        {
                            jw.WritePropertyName((string)key);
                            JsonSerializer.Create(JsonSettings.Tolerant).Serialize(jw, value);
                        }
                        catch
                        {
                            jw.WritePropertyName((string)key);
                            jw.WriteValue("error converting data to string");
                        }
                    }

                if (hasCustomData)
                {
                    jw.WriteEndObject();
                    jw.Flush();
                    return sb.ToString();
                }
            }

            return null;
        }
    }
}
