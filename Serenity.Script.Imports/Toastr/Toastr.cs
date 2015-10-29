using jQueryApi;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    /// <summary>
    /// Import of the toastr plugin (toastr.js, https://github.com/CodeSeven/toastr)
    /// </summary>
    [Imported, IgnoreNamespace, ScriptName("toastr")]
    public static class Toastr
    {
        /// <summary>
        /// Display a warning notification
        /// </summary>
        /// <param name="message">Message</param>
        public static void Warning(string message)
        {
        }

        /// <summary>
        /// Display a warning notification
        /// </summary>
        /// <param name="message">Message</param>
        public static void Warning(string message, string title, ToastrOptions options)
        {
        }


        /// <summary>
        /// Display a success notification
        /// </summary>
        /// <param name="message">Message</param>
        public static void Success(string message)
        {
        }

        /// <summary>
        /// Display a success notification
        /// </summary>
        /// <param name="message">Message</param>
        public static void Success(string message, string title, ToastrOptions options)
        {
        }

        /// <summary>
        /// Display an information notification
        /// </summary>
        /// <param name="message">Message</param>
        public static void Info(string message)
        {
        }

        /// <summary>
        /// Display an information notification
        /// </summary>
        /// <param name="message">Message</param>
        public static void Info(string message, string title, ToastrOptions options)
        {
        }

        /// <summary>
        /// Display an error notification
        /// </summary>
        /// <param name="message">Message</param>
        public static void Error(string message)
        {
        }

        /// <summary>
        /// Display an error notification
        /// </summary>
        /// <param name="message">Message</param>
        public static void Error(string message, string title, ToastrOptions options)
        {
        }

        /// <summary>
        /// Clears notifications
        /// </summary>
        public static void Clear()
        {
        }

        /// <summary>
        /// Gets container for toastr
        /// </summary>
        /// <param name="options">Options</param>
        /// <param name="create">Create if not exists</param>
        /// <returns>Current container or emtpy array if none</returns>
        public static jQueryObject GetContainer(ToastrOptions options, bool create)
        {
            return null;
        }

        /// <summary>
        /// Gets global toastr options
        /// </summary>
        [IntrinsicProperty]
        public static ToastrOptions Options
        {
            get { return null; }
        }
    }
}