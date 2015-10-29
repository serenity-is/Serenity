using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;

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

        public static void PositionToastContainer(bool create)
        {
            if (Window.Instance.As<dynamic>().toastr == null)
                return;

            var dialog = J(Window.Document.Body).Children(".ui-dialog:visible").Last();
            var container = Toastr.GetContainer(null, create);

            if (container.Length == 0)
                return;

            if (dialog.Length > 0)
            {
                var position = dialog.Position();
                container.AddClass("positioned-toast toast-top-full-width");
                container.CSS(new
                {
                    position = "absolute",
                    top = (position.Top + 28) + "px",
                    left = (position.Left + 6) + "px",
                    width = (dialog.GetWidth() - 12) + "px"
                }.As<JsDictionary<string, object>>());
            }
            else
            {
                container.AddClass("toast-top-full-width");
                if (container.HasClass("positioned-toast"))
                {
                    container.RemoveClass("positioned-toast");
                    container.CSS(new
                    {
                        position = "",
                        top = "",
                        left = "",
                        width = ""
                    }.As<JsDictionary<string, object>>());
                }
            }
        }

        private static ToastrOptions GetToastrOptions()
        {
            var options = new ToastrOptions
            {
                TimeOut = 20000,
                ShowDuration = 2250,
                HideDuration = 2200,
                ExtendedTimeOut = 500,
                PositionClass = "toast-top-full-width",
            };

            PositionToastContainer(true);

            return options;
        }

        [InlineCode("window.console && window.console.log({message})")]
        public static void Log(object message)
        {
        }

    }
}
