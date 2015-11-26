using jQueryApi;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public static partial class Q
    {
        /// <summary>
        /// Shows a custom alert dialog
        /// </summary>
        /// <param name="message">Message</param>
        [InlineCode("Q.alert({message})")]
        public static void Alert(string message)
        {
        }

        /// <summary>
        /// Shows a custom alert dialog
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="options">Options</param>
        [InlineCode("Q.alert({message}, {options})")]
        public static void Alert(string message, AlertOptions options)
        {
        }

        /// <summary>
        /// Shows a custom warning dialog
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="options">Options</param>
        [InlineCode("Q.warning({message}, {options})")]
        public static void Warning(string message, AlertOptions options)
        {
        }

        /// <summary>
        /// Shows a custom confirmation dialog
        /// </summary>
        /// <param name="message">Message</param>
        [InlineCode("Q.confirm({message}, {onYes})")]
        public static void Confirm(string message, Action onYes)
        {
        }

        /// <summary>
        /// Shows a custom confirmation dialog
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="options">Options</param>
        [InlineCode("Q.confirm({message}, {onYes}, {options})")]
        public static void Confirm(string message, Action onYes, ConfirmOptions options)
        {
        }

        /// <summary>
        /// Shows a custom information dialog
        /// </summary>
        /// <param name="message">Message</param>
        [InlineCode("Q.information({message})")]
        public static void Information(string message)
        {
        }

        /// <summary>
        /// Shows a custom information dialog
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="options">Options</param>
        [InlineCode("Q.information({message}, {onOk}, {options})")]
        public static void Information(string message, Action onOk, ConfirmOptions options)
        {
        }
    }
}