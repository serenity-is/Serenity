using System;
using System.Collections.Generic;

namespace Serenity
{
    public static partial class Q
    {
        /// <summary>
        /// Gets localized representation of a text key, or key if not defined
        /// </summary>
        /// <param name="key">Text key like Enums.WeekDays.Monday</param>
        /// <returns>Localized text</returns>
        public static string Text(string key)
        {
            return LocalText.table[key] ?? key ?? "";
        }

        /// <summary>
        /// Gets localized representation of a text key
        /// </summary>
        /// <param name="key">Text key like Enums.WeekDays.Monday</param>
        /// <returns>Localized text</returns>
        public static string TryGetText(string key)
        {
            return LocalText.table[key];
        }
    }
}