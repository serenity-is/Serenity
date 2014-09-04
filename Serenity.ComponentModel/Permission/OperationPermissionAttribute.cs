using System;

namespace Serenity.Data
{
    public abstract class OperationPermissionAttribute : Attribute
    {
        public OperationPermissionAttribute(string permission)
        {
            this.Permission = permission;
        }

        public string Permission { get; private set; }
    }
}