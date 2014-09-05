using Serenity.Services;
using System;

namespace Serenity
{
    public interface IAuthenticationService
    {
        bool Validate(ref string username, string password);
    }
}