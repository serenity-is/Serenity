using jQueryApi;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public static partial class Q
    {
        /// <summary>
        /// Html encodes a string
        /// </summary>
        /// <param name="value">string</param>
        /// <returns>encoded string</returns>
        [InlineCode("Q.htmlEncode({value})")]
        public static string HtmlEncode(object value)
        {
            return null;
        }

        /// <summary>
        /// Creates a new div and appends it to document body
        /// </summary>
        /// <returns>Newly created div</returns>
        [InlineCode("Q.newBodyDiv()")]
        public static jQueryObject NewBodyDiv()
        {
            return null;
        }

        [InlineCode("Q.clearOptions({select})")]
        public static void ClearOptions(jQueryObject select)
        {
        }

        [InlineCode("Q.addEmptyOption({select})")]
        public static void AddEmptyOption(jQueryObject select)
        {
        }

        [InlineCode("Q.addOption({select}, {key}, {text})")]
        public static void AddOption(jQueryObject select, string key, string text)
        {
        }

        [InlineCode("Q.findElementWithRelativeId({element}, {relativeId})")]
        public static jQueryObject FindElementWithRelativeId(jQueryObject element, string relativeId)
        {
            return null;
        }

        [InlineCode("Q.outerHtml({element})")]
        public static string OuterHtml(this jQueryObject element)
        {
            return null;
        }
    }
}