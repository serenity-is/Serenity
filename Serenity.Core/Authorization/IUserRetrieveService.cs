using Serenity.Services;
using System;

namespace Serenity.Abstractions
{
    public interface IUserRetrieveService
    {
        IUserDefinition ById(Int64 id);
        IUserDefinition ByUsername(string username);
    }
}