using Serenity.Abstractions;
using System;
using System.Security.Claims;
using System.Security.Principal;
using Xunit;

namespace Serenity.Tests.Authorization
{
    public class AuthorizationExtensionsTests
    {
        [Fact]
        public void IsLoggedIn_Returns_False_If_UserAccessor_IsNull()
        {
            Assert.False(((IUserAccessor)null).IsLoggedIn());
        }

        private class FakeUserAccessor : IUserAccessor
        {
            private readonly Func<ClaimsPrincipal> getUser;

            public FakeUserAccessor(Func<ClaimsPrincipal> getUser)
            {
                this.getUser = getUser ?? throw new ArgumentNullException(nameof(getUser));
            }

            public ClaimsPrincipal User => getUser();
        }

        [Fact]
        public void IsLoggedIn_Returns_False_If_UserAccessor_User_Property_IsNull()
        {
            var userAccessor = new FakeUserAccessor(() => null);
            Assert.False(userAccessor.IsLoggedIn());
        }

        [Fact]
        public void IsLoggedIn_Returns_False_If_UserAccessor_User_Identity_IsNull()
        {
            var userAccessor = new FakeUserAccessor(() => new ClaimsPrincipal());
            Assert.False(userAccessor.IsLoggedIn());
        }

        [Fact]
        public void IsLoggedIn_Returns_False_If_UserAccessor_User_Identity_IsAuthenticated_IsFalse()
        {
            var identity = new ClaimsIdentity();
            Assert.False(identity.IsAuthenticated);
            var userAccessor = new FakeUserAccessor(() => new ClaimsPrincipal(identity));
            Assert.False(userAccessor.IsLoggedIn());
        }

        [Fact]
        public void IsLoggedIn_Returns_True_If_UserAccessor_User_Identity_IsAuthenticated_IsTrue()
        {
            var identity = new GenericIdentity("test");
            Assert.True(identity.IsAuthenticated);
            var userAccessor = new FakeUserAccessor(() => new ClaimsPrincipal(identity));
            Assert.True(userAccessor.IsLoggedIn());
        }

        [Fact]
        public void IsLoggedIn_Returns_False_If_Principal_IsNull()
        {
            Assert.False(((ClaimsPrincipal)null).IsLoggedIn());
        }

        [Fact]
        public void IsLoggedIn_Returns_False_If_Principal_Identity_IsNull()
        {
            var principal = new ClaimsPrincipal();
            Assert.False(principal.IsLoggedIn());
        }

        [Fact]
        public void IsLoggedIn_Returns_False_If_Principal_User_Identity_IsAuthenticated_IsFalse()
        {
            var identity = new ClaimsIdentity();
            Assert.False(identity.IsAuthenticated);
            Assert.False(new ClaimsPrincipal(identity).IsLoggedIn());
        }

        [Fact]
        public void IsLoggedIn_Returns_True_If_Principal_User_Identity_IsAuthenticated_IsTrue()
        {
            var identity = new GenericIdentity("test");
            Assert.True(identity.IsAuthenticated);
            Assert.True(new ClaimsPrincipal(identity).IsLoggedIn());
        }
    }
}