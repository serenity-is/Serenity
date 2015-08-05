using FakeItEasy;
using Serenity.Abstractions;
using Serenity.Testing;
using System;
using System.Collections.Generic;
using Xunit;

namespace Serenity.Test
{
    [Collection("AvoidParallel")]
    public class DependencyTests
    {
        [Fact]
        public void Dependency_HasResolver_ReturnsCorrectState()
        {
            try
            {
                Assert.Equal(false, Dependency.HasResolver);
                Dependency.SetResolver(new MunqContainer());
                Assert.True(Dependency.HasResolver);
                Dependency.SetResolver(null);
                Assert.False(Dependency.HasResolver);
            }
            finally
            {
                Dependency.SetResolver(null);
            }
        }

        [Fact]
        public void Dependency_Resolver_ThrowsExceptionIfNoResolverIsSet()
        {
            try
            {
                Assert.Throws<InvalidProgramException>(() => Dependency.Resolver);
                Dependency.SetResolver(new MunqContainer());
                Dependency.SetResolver(null);
                Assert.Throws<InvalidProgramException>(() => Dependency.Resolver);
            }
            finally
            {
                Dependency.SetResolver(null);
            }
        }

        [Fact]
        public void Dependency_Resolver_ReturnsObjectSetBySetResolverMethod()
        {
            try
            {
                var container1 = new MunqContainer();
                Dependency.SetResolver(container1);
                Assert.Equal(container1, Dependency.Resolver);
                var container2 = new MunqContainer();
                Dependency.SetResolver(container2);
                Assert.Equal(container2, Dependency.Resolver);
                Dependency.SetResolver(null);
                Assert.False(Dependency.HasResolver);
            }
            finally
            {
                Dependency.SetResolver(null);
            }
        }

        [Fact]
        public void Dependency_SetResolver_ReturnsPriorResolver()
        {
            try
            {
                var container1 = new MunqContainer();
                var before1 = Dependency.SetResolver(container1);
                Assert.Null(before1);

                var container2 = new MunqContainer();
                var before2 = Dependency.SetResolver(container2);
                Assert.Equal(container1, before2);

                var before3 = Dependency.SetResolver(null);
                Assert.Equal(container2, before3);
            }
            finally
            {
                Dependency.SetResolver(null);
            }
        }

        [Fact]
        public void Dependency_Resolve_ThrowsExceptionIfNoResolverIsSet()
        {
            Assert.Throws<InvalidProgramException>(() => Dependency.Resolve<IAuthorizationService>());
        }

        [Fact]
        public void Dependency_Resolve_ThrowsExceptionIfNoProviderIsRegistered()
        {
            using (new MunqContext())
            {
                Assert.Throws<KeyNotFoundException>(() => Dependency.Resolve<IAuthorizationService>());
            }
        }

        [Fact]
        public void Dependency_Resolve_UsesRegisteredContainer()
        {
            try
            {
                var resolver = A.Fake<IDependencyResolver>();
                var provider = A.Fake<IAuthenticationService>();
                A.CallTo(() => resolver.Resolve<IAuthenticationService>())
                    .Returns(provider);

                Dependency.SetResolver(resolver);
                
                var actual = Dependency.Resolve<IAuthenticationService>();
                
                Assert.Equal(provider, actual);

                A.CallTo(() => resolver.Resolve<IAuthenticationService>())
                    .MustHaveHappened(Repeated.Exactly.Once);

                A.CallTo(() => resolver.Resolve<IAuthenticationService>(A<string>.Ignored))
                    .MustNotHaveHappened();
            }
            finally
            {
                Dependency.SetResolver(null);
            }
        }

        [Fact]
        public void Dependency_ResolveWithName_ThrowsExceptionIfNoResolverIsSet()
        {
            Assert.Throws<InvalidProgramException>(() => Dependency.Resolve<IAuthorizationService>("Dummy"));
        }

        [Fact]
        public void Dependency_ResolveWithName_ThrowsExceptionIfNoProviderIsRegistered()
        {
            using (new MunqContext())
            {
                Assert.Throws<KeyNotFoundException>(() => Dependency.Resolve<IAuthorizationService>("Dummy"));
            }
        }

        [Fact]
        public void Dependency_ResolveWithName_UsesRegisteredContainer()
        {
            try
            {
                var resolver = A.Fake<IDependencyResolver>();
                var provider = A.Fake<IAuthenticationService>();
                A.CallTo(() => resolver.Resolve<IAuthenticationService>(A<String>.Ignored))
                    .Returns(provider);

                Dependency.SetResolver(resolver);

                var actual = Dependency.Resolve<IAuthenticationService>("Scope");

                Assert.Equal(provider, actual);

                A.CallTo(() => resolver.Resolve<IAuthenticationService>("Scope"))
                    .MustHaveHappened(Repeated.Exactly.Once);

                A.CallTo(() => resolver.Resolve<IAuthenticationService>(A<string>.That.Matches(x => x != "Scope")))
                    .MustNotHaveHappened();

                A.CallTo(() => resolver.Resolve<IAuthenticationService>())
                    .MustNotHaveHappened();
            }
            finally
            {
                Dependency.SetResolver(null);
            }
        }

        [Fact]
        public void Dependency_TryResolve_ReturnsNullIfNoResolverIsSet()
        {
            var actual = Dependency.TryResolve<IAuthorizationService>();
            Assert.Null(actual);
        }

        [Fact]
        public void Dependency_TryResolve_ReturnsNullIfNoProviderIsRegistered()
        {
            using (new MunqContext())
            {
                var actual = Dependency.TryResolve<IAuthorizationService>();
                Assert.Null(actual);
            }
        }

        [Fact]
        public void Dependency_TryResolve_UsesRegisteredContainer()
        {
            try
            {
                var resolver = A.Fake<IDependencyResolver>();
                var provider = A.Fake<IAuthenticationService>();
                A.CallTo(() => resolver.TryResolve<IAuthenticationService>())
                    .Returns(provider);

                Dependency.SetResolver(resolver);

                var actual = Dependency.TryResolve<IAuthenticationService>();

                Assert.Equal(provider, actual);

                A.CallTo(() => resolver.TryResolve<IAuthenticationService>())
                    .MustHaveHappened(Repeated.Exactly.Once);

                A.CallTo(() => resolver.TryResolve<IAuthenticationService>(A<string>.Ignored))
                    .MustNotHaveHappened();
            }
            finally
            {
                Dependency.SetResolver(null);
            }
        }

        [Fact]
        public void Dependency_TryResolveWithName_ReturnsNullIfNoResolverIsSet()
        {
            var actual = Dependency.TryResolve<IAuthorizationService>("Dummy");
            Assert.Null(actual);
        }

        [Fact]
        public void Dependency_TryResolveWithName_ReturnsNullIfNoProviderIsRegistered()
        {
            using (new MunqContext())
            {
                var actual = Dependency.TryResolve<IAuthorizationService>("Dummy");
                Assert.Null(actual);
            }
        }

        [Fact]
        public void Dependency_TryResolveWithName_UsesRegisteredContainer()
        {
            try
            {
                var resolver = A.Fake<IDependencyResolver>();
                var provider = A.Fake<IAuthenticationService>();
                A.CallTo(() => resolver.TryResolve<IAuthenticationService>(A<String>.Ignored))
                    .Returns(provider);

                Dependency.SetResolver(resolver);

                var actual = Dependency.TryResolve<IAuthenticationService>("Scope");

                Assert.Equal(provider, actual);

                A.CallTo(() => resolver.TryResolve<IAuthenticationService>("Scope"))
                    .MustHaveHappened(Repeated.Exactly.Once);

                A.CallTo(() => resolver.TryResolve<IAuthenticationService>(A<string>.That.Matches(x => x != "Scope")))
                    .MustNotHaveHappened();

                A.CallTo(() => resolver.TryResolve<IAuthenticationService>())
                    .MustNotHaveHappened();
            }
            finally
            {
                Dependency.SetResolver(null);
            }
        }
    }
}