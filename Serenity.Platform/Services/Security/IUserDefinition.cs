using Serenity.Services;
using System;

namespace Serenity
{
    public interface IUserDefinition
    {
        Int32 UserId { get; }
        string Username { get; }
        string DisplayName { get; }
        string Email { get; }
        Int16 IsActive { get;  }
    }
    
    [Serializable]
    public class UserDefinition : IUserDefinition
    {
        public Int32 UserId { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public Int16 IsActive { get; set; }
    }
}