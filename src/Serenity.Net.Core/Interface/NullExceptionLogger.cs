namespace Serenity
{
    /// <summary>
    /// Null exception logger that logs nothing
    /// </summary>
    public class NullExceptionLogger : IExceptionLogger
    {
        private NullExceptionLogger()
        {
        }

        /// <summary>
        /// NullExceptionLogger instance
        /// </summary>
        public static readonly NullExceptionLogger Instance = new NullExceptionLogger();

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="category">Optional category, can be null</param>
        public void Log(Exception exception, string category)
        {
        }
    }
}