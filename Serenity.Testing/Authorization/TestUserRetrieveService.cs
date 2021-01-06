using Serenity.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Serenity.Testing
{
    public class TestUserRetrieveService : IUserRetrieveService
    {
        private Func<string, IUserDefinition> byUsername;
        private Func<string, IUserDefinition> byId;

        public TestUserRetrieveService(
            Func<string, IUserDefinition> byUsername,
            Func<string, IUserDefinition> byId)
        {
            this.byId = byId;
            this.byUsername = byUsername;
        }

        public TestUserRetrieveService(Func<IUserDefinition[]> getUsers)
        {
            if (getUsers == null)
                throw new ArgumentNullException("users");

            this.byId = id => (getUsers() ?? new IUserDefinition[0])
                .FirstOrDefault(x => x.Id == id);
            
            this.byUsername = name => (getUsers() ?? new IUserDefinition[0])
                .FirstOrDefault(x => String.Compare(x.Username, name, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public IUserDefinition ById(string id)
        {
 	        return byId(id);
        }

        public IUserDefinition ByUsername(string username)
        {
 	        return byUsername(username);
        }
    }
}