using Serenity.Abstractions;
using System;

namespace Serenity.Testing
{
    public class TestPermissionService : IPermissionService
    {
        private Func<string, bool> hasPermission;

        public TestPermissionService(Func<string, bool> hasPermission)
        {
            this.hasPermission = hasPermission;
        }

        public bool HasPermission(string permission)
        {
            return hasPermission(permission);
        }
    }
}