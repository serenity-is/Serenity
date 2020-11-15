using System.Security.Claims;

namespace Serenity.Abstractions
{
    /// <summary>
    /// Abstraction to access current user
    /// </summary>
    public interface IUserAccessor
    {
        /// <summary>
        /// Gets current user
        /// </summary>
        ClaimsPrincipal User { get; }
    }
}