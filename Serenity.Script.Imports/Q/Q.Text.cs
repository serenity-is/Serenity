using System.Runtime.CompilerServices;

namespace Serenity
{
    public static partial class Q
    {
        /// <summary>
        /// Gets localized representation of a text key, or key if not defined
        /// </summary>
        /// <param name="key">Text key like Enums.WeekDays.Monday</param>
        /// <returns>Localized text</returns>
        [InlineCode("Q.text({key})")]
        public static string Text(string key)
        {
            return null;
        }

        /// <summary>
        /// Gets localized representation of a text key
        /// </summary>
        /// <param name="key">Text key like Enums.WeekDays.Monday</param>
        /// <returns>Localized text</returns>
        [InlineCode("Q.tryGetText({key})")]
        public static string TryGetText(string key)
        {
            return null;
        }
    }
}