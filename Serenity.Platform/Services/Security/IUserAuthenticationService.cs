using Serenity.Services;
using System;

namespace Serenity
{
    public interface IUserAuthenticationService
    {
        bool Validate(ref string username, string password);
    }
}