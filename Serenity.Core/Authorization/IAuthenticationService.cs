using Serenity.Services;
using System;

namespace Serenity.Abstractions
{
    public interface IAuthenticationService
    {
        bool Validate(ref string username, string password);
    }
}