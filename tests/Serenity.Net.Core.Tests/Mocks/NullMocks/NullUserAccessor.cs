using Serenity.Abstractions;
using System.Security.Claims;

namespace Serenity.Tests
{
    public class NullUserAccessor : IUserAccessor
    {
        public ClaimsPrincipal User => null;
    }
}