using System.Runtime.CompilerServices;

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
        [InlineCode("Q.blockUI({options})")]
        public static void BlockUI(BlockUIOptions options = null)
        {
        }

        [InlineCode("Q.blockUndo()")]
        public static void BlockUndo()
        {
        }
    }
}