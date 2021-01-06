using Serenity.Abstractions;
using System;
using System.Security.Claims;

namespace Serenity.Tests
{
    public class MockUserAccessor : IUserAccessor
    {
        private readonly Func<ClaimsPrincipal> getUser;

        public MockUserAccessor(Func<ClaimsPrincipal> getUser)
        {
            this.getUser = getUser ?? throw new ArgumentNullException(nameof(getUser));
        }

        public ClaimsPrincipal User => getUser();
    }
}