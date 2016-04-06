using System.Runtime.CompilerServices;

namespace Serenity
{
    public static partial class Q
    {
        /// <summary>
        /// Display a warning notification
        /// </summary>
        /// <param name="message">Message</param>
        [InlineCode("Q.notifyWarning({message}, {title}, {options})")]
        public static void NotifyWarning(string message, string title = "", ToastrOptions options = null)
        {
        }

        /// <summary>
        /// Display a success notification
        /// </summary>
        /// <param name="message">Message</param>
        [InlineCode("Q.notifySuccess({message}, {title}, {options})")]
        public static void NotifySuccess(string message, string title = "", ToastrOptions options = null)
        {
        }

        /// <summary>
        /// Display an information notification
        /// </summary>
        /// <param name="message">Message</param>
        [InlineCode("Q.notifyInfo({message}, {title}, {options})")]
        public static void NotifyInfo(string message, string title = "", ToastrOptions options = null)
        {
        }

        /// <summary>
        /// Display an error notification
        /// </summary>
        /// <param name="message">Message</param>
        [InlineCode("Q.notifyError({message}, {title}, {options})")]
        public static void NotifyError(string message, string title = "", ToastrOptions options = null)
        {
        }

        [InlineCode("Q.positionToastContainer({create})")]
        public static void PositionToastContainer(bool create)
        {
        }

        [IntrinsicProperty]
        public static ToastrOptions DefaultNotifyOptions { [InlineCode("Q.defaultNotifyOptions")] get; private set; }

        [InlineCode("window.console && window.console.log({message})")]
        public static void Log(object message)
        {
        }
    }
}