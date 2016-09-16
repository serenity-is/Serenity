#if !COREFX
using System.Linq;
using System.Security.Principal;

namespace Serenity.Web.MvcFakes
{
    public class FakePrincipal : IPrincipal
    {
        private readonly IIdentity _identity;
        private readonly string[] _roles;

        public FakePrincipal(IIdentity identity, string[] roles)
        {
            _identity = identity;
            _roles = roles;
        }

        public IIdentity Identity
        {
            get { return _identity; }
        }

        public bool IsInRole(string role)
        {
            if (_roles == null)
                return false;

            return _roles.Contains(role);
        }
    }

}
#endif