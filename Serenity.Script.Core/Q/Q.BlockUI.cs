using System.Html;
using jQueryApi;
using System;

namespace Serenity
{
    public static partial class Q
    {
        private static int blockUICount = 0;

        private static void BlockUIWithCheck(BlockUIOptions options)
        {
            if (blockUICount > 0)
            {
                blockUICount++;
                return;
            }

            jQuery.Instance.blockUI(options);
            blockUICount++;
        }

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
                Window.SetTimeout((System.Function)new Action(delegate {
                        BlockUIWithCheck(options);
                    }), 0);
            else
                BlockUIWithCheck(options);
        }

        /// <summary>
        /// Undos blocking done by BlockUI
        /// </summary>
        public static void BlockUndo()
        {
            if (blockUICount > 1)
            {
                blockUICount--;
                return;
            }

            blockUICount--;
            jQuery.Instance.unblockUI(new { fadeOut = 0 });
        }
    }
}