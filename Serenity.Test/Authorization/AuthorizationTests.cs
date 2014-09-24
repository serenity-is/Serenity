using Serenity.Abstractions;
using Serenity.Testing;
using System.Collections.Generic;
using Xunit;

namespace Serenity.Test
{
    public partial class AuthorizationTests
    {
        [Fact]
        public void Authorization_UsernameThrowsExceptionIfNoAuthorizationServiceIsRegistered()
        {
            using (new MunqContext())
            {
                var exception = Assert.Throws<KeyNotFoundException>(() => Authorization.Username);
                Assert.Contains(typeof(IAuthorizationService).Name, exception.Message);
            }
        }

        [Fact]
        public void Authorization_IsLoggedInThrowsExceptionIfNoAuthorizationServiceIsRegistered()
        {
            using (new MunqContext())
            {
                var exception = Assert.Throws<KeyNotFoundException>(() => Authorization.IsLoggedIn);
                Assert.Contains(typeof(IAuthorizationService).Name, exception.Message);
            }
        }

        [Fact]
        public void Authorization_UserDefinitionThrowsExceptionIfNoUserRetrieveServiceIsRegistered()
        {
            using (new MunqContext())
            {
                var testUser = new TestUserDefinition();
                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                registrar.RegisterInstance<IAuthorizationService>(
                    new TestAuthorizationService(() => testUser));

                var exception = Assert.Throws<KeyNotFoundException>(() => Authorization.UserDefinition);
                Assert.Contains(typeof(IUserRetrieveService).Name, exception.Message);
            }
        }

        [Fact]
        public void Authorization_UserIdThrowsExceptionIfNoUserRetrieveServiceIsRegistered()
        {
            using (new MunqContext())
            {
                var testUser = new TestUserDefinition();
                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                registrar.RegisterInstance<IAuthorizationService>(
                    new TestAuthorizationService(() => testUser));

                var exception = Assert.Throws<KeyNotFoundException>(() => Authorization.UserId);
                Assert.Contains(typeof(IUserRetrieveService).Name, exception.Message);
            }
        }

        [Fact]
        public void Authorization_HasPermssionThrowsExceptionIfNoPermissionServiceIsRegistered()
        {
            using (new MunqContext())
            {
                var exception = Assert.Throws<KeyNotFoundException>(() => Authorization.HasPermission("x"));
                Assert.Contains(typeof(IPermissionService).Name, exception.Message);
            }
        }

        [Fact]
        public void Authorization_UsernameReturnsUsernameFromRegisteredAuthorizationService()
        {
            using (new MunqContext())
            {
                TestUserDefinition testUser = null;

                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                
                registrar.RegisterInstance<IAuthorizationService>(
                    new TestAuthorizationService(() => testUser));

                Assert.Equal(null, Authorization.Username);

                testUser = new TestUserDefinition();

                Assert.Equal(testUser.Username, Authorization.Username);

                testUser.Username = "x";
                Assert.Equal("x", Authorization.Username);
            }
        }

        [Fact]
        public void Authorization_LoggedInUsesRegisteredAuthorizationService()
        {
            using (new MunqContext())
            {
                TestUserDefinition testUser = null;

                var registrar = Dependency.Resolve<IDependencyRegistrar>();

                registrar.RegisterInstance<IAuthorizationService>(
                    new TestAuthorizationService(() => testUser));

                Assert.Equal(false, Authorization.IsLoggedIn);

                testUser = new TestUserDefinition();

                Assert.Equal(true, Authorization.IsLoggedIn);
            }
        }

        [Fact]
        public void Authorization_UserDefinitionUsesRegisteredUserRetrieveService()
        {
            using (new MunqContext())
            {
                TestUserDefinition testUser = null;

                var registrar = Dependency.Resolve<IDependencyRegistrar>();

                registrar.RegisterInstance<IAuthorizationService>(
                    new TestAuthorizationService(() => testUser));

                registrar.RegisterInstance<IUserRetrieveService>(
                    new TestUserRetrieveService(() => new[] { testUser }));

                Assert.Equal(null, Authorization.UserDefinition);

                testUser = new TestUserDefinition();

                Assert.Equal(true, Authorization.IsLoggedIn);
            }
        }

        [Fact]
        public void Authorization_UserIdUsesRegisteredUserRetrieveService()
        {
            using (new MunqContext())
            {
                TestUserDefinition testUser = null;

                var registrar = Dependency.Resolve<IDependencyRegistrar>();

                registrar.RegisterInstance<IAuthorizationService>(
                    new TestAuthorizationService(() => testUser));

                registrar.RegisterInstance<IUserRetrieveService>(
                    new TestUserRetrieveService(() => new[] { testUser }));

                Assert.Equal(null, Authorization.UserId);

                testUser = new TestUserDefinition();

                Assert.Equal(testUser.Id, Authorization.UserId);
            }
        }

        [Fact]
        public void Authorization_HasPermissionUsesRegisteredPermissionService()
        {
            using (new MunqContext())
            {
                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                bool hasYPermission = false;

                registrar.RegisterInstance<IPermissionService>(
                    new TestPermissionService((s) => s == "y" && hasYPermission));

                Assert.Equal(false, Authorization.HasPermission("x"));
                Assert.Equal(false, Authorization.HasPermission("y"));

                hasYPermission = true;

                Assert.Equal(false, Authorization.HasPermission("x"));
                Assert.Equal(true, Authorization.HasPermission("y"));
            }
        }
    }
}