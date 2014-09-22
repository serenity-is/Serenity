using System;

namespace Serenity
{
    public interface IUserDefinition
    {
        Int64 UserId { get; }
        string Username { get; }
        string DisplayName { get; }
        string Email { get; }
        Int16 IsActive { get;  }
    }
}