namespace Serenity.Abstractions
{
    /// <summary>
    /// Authorization service abstraction
    /// </summary>
    public interface IAuthorizationService
    {
        /// <summary>
        /// True if there is a currenty logged user
        /// </summary>
        bool IsLoggedIn { get; }
        /// <summary>
        /// Return currently logged user name
        /// </summary>
        string Username { get; }
    }
}