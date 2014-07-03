using jQueryApi.UI.Widgets;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    /// <summary>
    /// Options for the Q.Confirm and Q.Information functions
    /// </summary>
    [Imported, Serializable]
    public class ConfirmOptions
    {
        /// <summary>
        /// Dialog title (default is 'Onay')
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Yes button text (default is 'Evet')
        /// </summary>
        public string YesButton { get; set; }

        /// <summary>
        /// No button text (default is 'No')
        /// </summary>
        public string NoButton { get; set; }

        /// <summary>
        /// Css class
        /// </summary>
        public string DialogClass { get; set; }

        /// <summary>
        /// HtmlEncode the alert message (default is true)
        /// </summary>
        public bool HtmlEncode { get; set; }

        /// <summary>
        /// Dialog close event
        /// </summary>
        public Action OnClose { get; set; }

        /// <summary>
        /// Cancel or close button event
        /// </summary>
        public Action OnCancel { get; set; }

        /// <summary>
        /// No button click event
        /// </summary>
        public Action OnNo { get; set; }

        /// <summary>
        /// Dialog open event
        /// </summary>
        public Action OnOpen { get; set; }
    }
}