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
        [AlternateSignature]
        public static void Alert(string message)
        {
        }

        /// <summary>
        /// Shows a custom alert dialog
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="options">Options</param>
        public static void Alert(string message, AlertOptions options)
        {
            Q.Externals.AlertDialog(message, options);
        }

        /// <summary>
        /// Shows a custom warning dialog
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="options">Options</param>
        public static void Warning(string message, AlertOptions options)
        {
            Q.Externals.AlertDialog(message, jQuery.ExtendObject(new AlertOptions
            {
                Title = Texts.Dialogs.WarningTitle,
                DialogClass = "s-MessageDialog s-WarningDialog"
            }, options));
        }

        /// <summary>
        /// Shows a custom confirmation dialog
        /// </summary>
        /// <param name="message">Message</param>
        [AlternateSignature]
        public static void Confirm(string message, Action onYes)
        {
        }

        /// <summary>
        /// Shows a custom confirmation dialog
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="options">Options</param>
        public static void Confirm(string message, Action onYes, ConfirmOptions options)
        {
            Q.Externals.ConfirmDialog(message, onYes, options);
        }

        /// <summary>
        /// Shows a custom information dialog
        /// </summary>
        /// <param name="message">Message</param>
        [AlternateSignature]
        public static void Information(string message)
        {
            Information(message, null, null);
        }

        /// <summary>
        /// Shows a custom information dialog
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="options">Options</param>
        public static void Information(string message, Action onOk, ConfirmOptions options)
        {
            Q.Externals.ConfirmDialog(message, onOk, jQuery.ExtendObject(new ConfirmOptions
            {
                Title = Texts.Dialogs.InformationTitle,
                YesButton = Texts.Dialogs.OkButton,
                NoButton = null,
                DialogClass = "s-MessageDialog s-InformationDialog"
            }, options));
        }
    }
}