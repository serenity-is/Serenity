using System;

namespace Serenity.Testing
{
    public class TestUserDefinition : IUserDefinition
    {
        static int nextId = 789012345;

        public TestUserDefinition(string username = null, int? userId = null)
        {
            UserId = userId ?? (nextId++);
            Username = username ?? ("user" + userId);
            DisplayName = username + " DisplayName";
            Email = username + "@dummy.com";
            IsActive = 1;
        }

        public Int64 UserId { get; set;  }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public Int16 IsActive { get; set; }
    }
}