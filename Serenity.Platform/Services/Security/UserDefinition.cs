using Serenity.Services;
using System;

namespace Serenity
{
    [Serializable]
    public class UserDefinition : IUserDefinition
    {
        public Int64 UserId { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public Int16 IsActive { get; set; }
    }
}