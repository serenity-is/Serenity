namespace Serenity.Localization
{
    /// <summary>
    /// Local text context abstraction for sites that support a pending approval mode.
    /// </summary>
    public interface ILocalTextContext
    {
        /// <summary>
        /// Returns true if site is in pending approval mode for current user (e.g. a moderator)
        /// </summary>
        bool IsApprovalMode { get; }
    }
}