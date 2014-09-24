using System;

namespace Serenity.Testing
{
    public class TestUserDefinition : IUserDefinition
    {
        static int nextId = 789012345;

        public TestUserDefinition(string username = null, string userId = null)
        {
            Id = userId ?? (nextId++).ToString();
            Username = username ?? ("user" + userId);
            DisplayName = username + " DisplayName";
            Email = username + "@dummy.com";
            IsActive = 1;
        }

        public string Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public Int16 IsActive { get; set; }
    }
}