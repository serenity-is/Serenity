namespace Serenity.Logging
{
    /// <summary>
    /// Logging level constants
    /// </summary>
    public enum LoggingLevel
    {
        /// <summary>
        /// Verbose
        /// </summary>
        Verbose = -6,
        /// <summary>
        /// Debug
        /// </summary>
        Debug = -5,
        /// <summary>
        /// Information
        /// </summary>
        Info = -4,
        /// <summary>
        /// Warning
        /// </summary>
        Warn = -3,
        /// <summary>
        /// Error
        /// </summary>
        Error = -2,
        /// <summary>
        /// Fatal
        /// </summary>
        Fatal = -1,
        /// <summary>
        /// Off
        /// </summary>
        Off = 0
    }
}