using System;
using System.Collections.Specialized;
using System.Web.Security;

namespace Serenity.Web.Providers
{
    public class SiteRoleProvider : RoleProvider
    {
        public override void Initialize(string name, NameValueCollection config) 
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (String.IsNullOrEmpty(name)) 
                name = "SiteRoleProvider";

            if (String.IsNullOrEmpty(config["description"]))
                config["description"] = "Site Role Provider";

            base.Initialize(name, config);
        }

        public override string ApplicationName 
        {
            get { return "site"; }
            set { }
        }

        public override void CreateRole(string rolename)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string rolename, bool throwOnPopulatedRole) 
        {
            throw new NotImplementedException();
        }

        public override void AddUsersToRoles(string[] usernames, string[] rolenames) 
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles() 
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username) 
        {
            return new string[0];
        }

        public override string[] GetUsersInRole(string rolename) 
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string rolename) 
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] rolenames) 
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string rolename) 
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string rolename, string usernameToMatch) 
        {
            throw new NotImplementedException();
        }
    }
}