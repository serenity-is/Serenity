using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    /// <summary>
    /// Import of the stack trace library (stacktrace.js, http://http://stacktracejs.com/)
    /// </summary>
    [Imported, GlobalMethods, IgnoreNamespace]
    public static class StackTrace
    {
        /// <summary>
        /// Gets stack trace as a string
        /// </summary>
        /// <param name="options">Options for stack trace function</param>
        /// <returns>Stack trace</returns>
        [ScriptName("printStackTrace")]
        public static string GetTrace(StackTraceOptions options)
        {
            return null;
        }

        /// <summary>
        /// Logs stack trace if window.console exists
        /// </summary>
        /// <param name="options">Options for stack trace function</param>
        [InlineCode("window && window.console && window.console.log(({e} && ({e}._error || {e}).stack) || printStackTrace({{e: ({e} && {e}._error) || {e}}}))")]
        public static void Log(Exception e)
        {
        }
    }
}