using System.Html;
using jQueryApi;

namespace Serenity
{
    public static partial class Q
    {
        /// <summary>
        /// Uses jQuery BlockUI plugin to block access to whole page (default) or 
        /// a part of it, by using a transparent overlay covering the whole area.
        /// </summary>
        /// <param name="options">Parameters for the BlockUI plugin.</param>
        /// <remarks>If options are not specified, this function blocks 
        /// whole page with a transparent overlay. Default z-order of the overlay
        /// div is 2000, so a higher z-order shouldn't be used in page.</remarks>
        public static void BlockUI(BlockUIOptions options = null)
        {
            options = jQuery.ExtendObject(
                new BlockUIOptions
                {
                    BaseZ = 2000,
                    Message = "",
                    OverlayCSS = new
                    {
                        opacity = "0.0",
                        zIndex = 2000,
                        cursor = "wait"
                    },
                    FadeOut = 0
                }, 
                options);

            if (options.UseTimeout)
                Window.SetTimeout(
                    delegate
                    {
                        jQuery.Instance.blockUI(options);
                    }, 
                    0
                );
            else
                jQuery.Instance.blockUI(options);
        }

        /// <summary>
        /// Undos blocking done by BlockUI
        /// </summary>
        public static void BlockUndo()
        {
            jQuery.Instance.unblockUI(new { fadeOut = 0 });
        }
    }
}