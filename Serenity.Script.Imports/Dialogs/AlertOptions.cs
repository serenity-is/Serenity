using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    /// <summary>
    /// Options for the Q.Alert function
    /// </summary>
    [Imported, Serializable]
    public class AlertOptions
    {
        /// <summary>
        /// Dialog title (default is 'Hata')
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Ok button text (default is 'Tamam')
        /// </summary>
        public string OkButton { get; set; }

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
        /// Dialog open event
        /// </summary>
        public Action OnOpen { get; set; }
    }
}