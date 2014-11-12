using System;

namespace Serenity.Data
{
    public abstract class PermissionAttributeBase : Attribute
    {
        public PermissionAttributeBase(string permission)
        {
            this.Permission = permission;
        }

        public PermissionAttributeBase(object module, string permission)
            : this(module.ToString() + ":" + permission)
        {
        }

        public PermissionAttributeBase(object module, object submodule, string permission)
            : this(module.ToString() + ":" + submodule.ToString() + ":" + permission)
        {
        }

        public string Permission { get; private set; }
    }
}