namespace Serenity.Testing
{
    using Serenity.Abstractions;
    using System;

    /// <remarks>
    /// This class is not thread safe! Only use in single threaded unit test contexts!
    /// </remarks>
    public class ImpersonationContext : IAuthorizationService, IUserRetrieveService, IDisposable
    {
        private IAuthorizationService oldAuthorizationService;
        private IUserRetrieveService oldUserRetrieveService;
        private string username;
        private IUserDefinition userDefinition;
        private object authorizationServiceReg;
        private object userRetrieveServiceReg;

        public ImpersonationContext(IUserDefinition userDefinition)
            : this(userDefinition.Username)
        {
            this.userDefinition = userDefinition;
        }

        public ImpersonationContext(string username)
        {
            this.username = username;
            oldAuthorizationService = Dependency.TryResolve<IAuthorizationService>();
            oldUserRetrieveService = Dependency.TryResolve<IUserRetrieveService>();
            var registrar = Dependency.Resolve<IDependencyRegistrar>();
            authorizationServiceReg = registrar.RegisterInstance<IAuthorizationService>(this);
            userRetrieveServiceReg = registrar.RegisterInstance<IUserRetrieveService>(this);
        }

        public virtual void Dispose()
        {
            var authorizationService = Dependency.TryResolve<IAuthorizationService>();
            var userRetrieveService = Dependency.TryResolve<IAuthorizationService>();

            if (authorizationService != null && authorizationService != this)
                throw new InvalidOperationException("Container changed! Possible multi-thread error, or disposing order mistake!");

            if (userRetrieveService != null && userRetrieveService != this)
                throw new InvalidOperationException("Container changed! Possible multi-thread error, or disposing order mistake!");

            var registrar = Dependency.Resolve<IDependencyRegistrar>();

            if (oldAuthorizationService == null)
            {
                registrar.Remove(authorizationServiceReg);
            }
            else
            {
                registrar.RegisterInstance<IAuthorizationService>(oldAuthorizationService);
            }

            if (oldUserRetrieveService == null)
            {
                registrar.Remove(userRetrieveServiceReg);
            }
            else
            {
                registrar.RegisterInstance<IUserRetrieveService>(oldUserRetrieveService);
            }

            oldAuthorizationService = null;
            oldUserRetrieveService = null;
            userRetrieveServiceReg = null;
            authorizationServiceReg = null;
        }

        public bool IsLoggedIn
        {
            get { return username.IsNullOrEmpty(); }
        }

        public string Username
        {
            get { return username; }
        }

        public IUserDefinition ById(string id)
        {
            if (userDefinition != null && userDefinition.Id == id)
                return userDefinition;

            return oldUserRetrieveService == null ? null : oldUserRetrieveService.ById(id);
        }

        public IUserDefinition ByUsername(string username)
        {
            if (username == this.username && userDefinition != null)
                return userDefinition;

            return oldUserRetrieveService == null ? null : oldUserRetrieveService.ByUsername(username);
        }
    }
}