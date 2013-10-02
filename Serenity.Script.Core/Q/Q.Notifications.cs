using jQueryApi;
using System;
using System.Html;
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
            Toastr.Warning(message, "", GetToastrOptions());
        }

        /// <summary>
        /// Display a success notification
        /// </summary>
        /// <param name="message">Message</param>
        public static void NotifySuccess(string message)
        {
            Toastr.Success(message, "", GetToastrOptions());
        }

        /// <summary>
        /// Display an information notification
        /// </summary>
        /// <param name="message">Message</param>
        public static void NotifyInfo(string message)
        {
            Toastr.Info(message, "", GetToastrOptions());
        }

        /// <summary>
        /// Display an error notification
        /// </summary>
        /// <param name="message">Message</param>
        public static void NotifyError(string message)
        {
            Toastr.Error(message, "", GetToastrOptions());
        }

        private static ToastrOptions GetToastrOptions()
        {
            var dialog = jQuery.FromElement(Window.Document.Body).Children(".ui-dialog").Last();
            var toastrDiv = jQuery.Select("#toast-container");
            if (dialog.Length > 0)
            {
                if (!toastrDiv.HasClass("dialog-toast") &&
                    toastrDiv.Length > 0)
                {
                    toastrDiv.Remove();
                }

                return new ToastrOptions
                {
                    Target = dialog,
                    PositionClass = "toast-top-full-width dialog-toast"
                };
            }
            else
            {
                toastrDiv.RemoveClass("dialog-toast");

                if (toastrDiv.HasClass("dialog-toast") &&
                    toastrDiv.Length > 0)
                {
                    toastrDiv.Remove();
                }

                return new ToastrOptions
                {
                    PositionClass = "toast-top-full-width"
                };
            }
        }
    }
}
