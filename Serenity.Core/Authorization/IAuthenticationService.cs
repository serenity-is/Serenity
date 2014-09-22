using Serenity.Services;
using System;

namespace Serenity.Abstractions
{
    /// <summary>
    /// Authentication service abstraction
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Returns true if username/password pair is correct
        /// </summary>
        bool Validate(ref string username, string password);
    }
}