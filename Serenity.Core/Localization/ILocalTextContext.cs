using System.Globalization;

namespace Serenity.Localization
{
    /// <summary>
    /// Local text context abstraction.
    /// </summary>
    public interface ILocalTextContext
    {
        /// <summary>
        /// Returns true if pending approval texts should be shown
        /// </summary>
        bool IsApprovalMode { get; }

        /// <summary>
        /// Returns the UI culture that local texts should be translated to
        /// </summary>
        CultureInfo UICulture { get; }
    }
}