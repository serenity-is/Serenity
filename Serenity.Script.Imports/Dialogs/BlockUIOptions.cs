using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    /// <summary>
    /// Options for the BlockUI plugin
    /// </summary>
    [Imported, Serializable, ScriptNamespace("Q")]
    public class BlockUIOptions
    {
        /// <summary>
        /// Gets or sets baze Z order of the overlay div
        /// </summary>
        public int BaseZ { get; set; }

        /// <summary>
        /// Gets or sets message to be shown in overlay div (default: Please wait...)
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets CSS key value pairs for overlay
        /// </summary>
        public dynamic OverlayCSS { get; set; }

        /// <summary>
        /// Gets or sets use timeout flag. Set to true to call Block UI after a 0 ms timeout.
        /// </summary>
        public bool UseTimeout { get; set; }

        /// <summary>
        /// Gets or sets fade out time in ms
        /// </summary>
        public int FadeOut { get; set; }
    }
}