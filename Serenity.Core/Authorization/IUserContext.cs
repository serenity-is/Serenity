using System.Security.Claims;

namespace Serenity.Abstractions
{
    /// <summary>
    /// Abstraction to access current user
    /// </summary>
    public interface IUserContext
    {
        /// <summary>
        /// Gets current user
        /// </summary>
        ClaimsPrincipal User { get; }
    }
}