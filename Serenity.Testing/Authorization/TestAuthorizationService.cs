using Serenity.Abstractions;
using System;

namespace Serenity.Testing
{
    public class TestAuthorizationService : IAuthorizationService
    {
        private Func<IUserDefinition> getUser;

        public TestAuthorizationService(Func<IUserDefinition> getUser)
        {
            this.getUser = getUser;
        }

        public bool IsLoggedIn
        {
            get 
            { 
                var user = getUser();
                return user == null || user.Username.IsEmptyOrNull() ? false : true;
            }
        }

        public string Username
        {
            get 
            { 
                var user = getUser();
                return user == null ? null : user.Username;
            }
        }

        public void Register()
        {
            Dependency.Resolve<IDependencyRegistrar>().RegisterInstance(this);
        }
    }
}