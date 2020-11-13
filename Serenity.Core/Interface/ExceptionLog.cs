using Serenity.Abstractions;
using System;

namespace Serenity
{
    /// <summary>
    ///   Centralized logger for exceptions.</summary>
    public static class ExceptionLog
    {
#if !NET
        /// <summary>
        ///   Logs an exception. Nothing logged if exception logger delegate is not set.</summary>
        /// <param name="e"></param>
#if !NET45
        [Obsolete("Use overload with IExceptionLogger")]
#endif
        public static void Log(this Exception e)
        {
            try
            {
                if ((e as INotLoggedException)?.NotLoggedException != true)
                {
                    var logger = Dependency.TryResolve<IExceptionLogger>();
                    if (logger != null)
                        logger.Log(e);
                }
            }
            catch
            {
            }
        }
#endif

        /// <summary>
        /// Logs an exception. Nothing logged if exception logger is null or exception
        /// is of type INotLoggedException.</summary>
        /// <param name="e">Exception</param>
        /// <param name="logger">Exception logger</param>
        public static void Log(this Exception e, IExceptionLogger logger)
        {
            try
            {
                if (logger != null && (e as INotLoggedException)?.NotLoggedException != true)
                    logger.Log(e);
            }
            catch
            {
            }
        }
    }
}
