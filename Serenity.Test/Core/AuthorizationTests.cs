using FakeItEasy;
using Serenity.Abstractions;
using Serenity.Services;
using Serenity.Testing;
using System.Collections.Generic;
using Xunit;

namespace Serenity.Test
{
    [Collection("AvoidParallel")]
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
        public void Authorization_HasPermissionThrowsExceptionIfNoPermissionServiceIsRegistered()
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

        [Fact]
        public void Authorization_ValidateLoggedInThrowsExceptionIfNotLoggedIn()
        {
            using (new MunqContext())
            {
                TestUserDefinition testUser = null;

                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                registrar.RegisterInstance<IAuthorizationService>(
                    new TestAuthorizationService(() => testUser));

                var exception = Assert.Throws<ValidationError>(() =>
                    Authorization.ValidateLoggedIn());

                Assert.Equal("NotLoggedIn", exception.ErrorCode);

                testUser = new TestUserDefinition();
                Authorization.ValidateLoggedIn();
            }
        }

        [Fact]
        public void Authorization_ValidatePermissionThrowsExceptionIfDoesntHavePermission()
        {
            using (new MunqContext())
            {
                bool hasPermission = false;

                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                registrar.RegisterInstance<IPermissionService>(
                    new TestPermissionService((s) => hasPermission));

                var exception = Assert.Throws<ValidationError>(() =>
                    Authorization.ValidatePermission("Dummy"));

                Assert.Equal("AccessDenied", exception.ErrorCode);

                hasPermission = true;

                Authorization.ValidatePermission("Dummy");
            }
        }

        [Fact]
        public void Authorization_HasPermissionReturnsTrueForAsterisk()
        {
            using (new MunqContext())
            {
                Assert.Equal(true, Authorization.HasPermission("*"));
            }
        }

        [Fact]
        public void Authorization_HasPermissionReturnsIsLoggedInForEmptyStringOrQuestionMark()
        {
            using (new MunqContext())
            {
                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                var fake = A.Fake<IAuthorizationService>();

                bool isLoggedIn = false;
                A.CallTo(() => fake.IsLoggedIn).ReturnsLazily(() => isLoggedIn);
                A.CallTo(() => fake.Username).ReturnsLazily(() => isLoggedIn ? "dummy": null);
                registrar.RegisterInstance<IAuthorizationService>(fake);

                Assert.Equal(false, Authorization.HasPermission("?"));
                Assert.Equal(false, Authorization.HasPermission(""));
                Assert.Equal(false, Authorization.HasPermission(null));
                isLoggedIn = true;
                Assert.Equal(true, Authorization.HasPermission("?"));
                Assert.Equal(true, Authorization.HasPermission(""));
                Assert.Equal(false, Authorization.HasPermission(null));
            }
        }
    }
}