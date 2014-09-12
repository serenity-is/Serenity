namespace BasicApplication.Administration
{
    using Serenity;
    using Serenity.Web.Providers;
    using System;

    public class AuthenticationService : IAuthenticationService
    {
        public bool Validate(ref string username, string password)
        {
            if (username.IsTrimmedEmpty())
                return false;

            username = username.TrimToEmpty();

            var user = Dependency.Resolve<IUserRetrieveService>().ByUsername(username) as UserDefinition;

            if (user != null)
            {
                username = user.Username;

                if (user.Source == "site")
                    return SiteMembershipProvider.ComputeSHA512(password + user.PasswordSalt).Equals(user.PasswordHash, StringComparison.OrdinalIgnoreCase);

                throw new ArgumentOutOfRangeException("userSource");
            }

            return false;
        }
    }
}