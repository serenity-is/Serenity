using System.Html;

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
            var dialog = J(Window.Document.Body).Children(".ui-dialog").Last();
            var toastrDiv = J("#toast-container");

            var options = new ToastrOptions
            {
                TimeOut = 3000,
                FadeIn = 250,
                FadeOut = 500,
                ExtendedTimeOut = 500
            };


            if (dialog.Length > 0)
            {
                if (!toastrDiv.HasClass("dialog-toast") &&
                    toastrDiv.Length > 0)
                {
                    toastrDiv.Remove();
                }

                options.Target = dialog;
                options.PositionClass = "toast-top-full-width dialog-toast";
            }
            else
            {
                toastrDiv.RemoveClass("dialog-toast");

                if (toastrDiv.HasClass("dialog-toast") &&
                    toastrDiv.Length > 0)
                {
                    toastrDiv.Remove();
                }

                options.PositionClass = "toast-top-full-width";
            }

            return options;
        }

    }
}
