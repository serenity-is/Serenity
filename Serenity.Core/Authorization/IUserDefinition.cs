using System;

namespace Serenity
{
    /// <summary>
    /// User definition abstraction.
    /// </summary>
    public interface IUserDefinition
    {
        /// <summary>
        /// User ID
        /// </summary>
        Int64 UserId { get; }
        /// <summary>
        /// User login name
        /// </summary>
        string Username { get; }
        /// <summary>
        /// Display name for user (can be same with Username)
        /// </summary>
        string DisplayName { get; }
        /// <summary>
        /// Email address
        /// </summary>
        string Email { get; }
        /// <summary>
        /// Is user active (1 = active, 0 = disabled, -1 = deleted)
        /// </summary>
        Int16 IsActive { get;  }
    }
}