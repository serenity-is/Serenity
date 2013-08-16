using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Serenity
{
    public static partial class Q
    {
        /// <summary>
        /// Display a warning notification
        /// </summary>
        /// <param name="message">Message</param>
        public static void NotifyWarning(string message)
        {
            Toastr.Warning(message);
        }

        /// <summary>
        /// Display a success notification
        /// </summary>
        /// <param name="message">Message</param>
        public static void NotifySuccess(string message)
        {
            Toastr.Success(message);
        }

        /// <summary>
        /// Display an information notification
        /// </summary>
        /// <param name="message">Message</param>
        public static void NotifyInfo(string message)
        {
            Toastr.Info(message);
        }

        /// <summary>
        /// Display an error notification
        /// </summary>
        /// <param name="message">Message</param>
        public static void NotifyError(string message)
        {
            Toastr.Error(message);
        }
    }
}
