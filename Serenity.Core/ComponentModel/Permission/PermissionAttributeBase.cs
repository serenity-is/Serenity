using System;

namespace Serenity.Data
{
    public abstract class PermissionAttributeBase : Attribute
    {
        public PermissionAttributeBase(object permission)
        {
            this.Permission = permission == null ? null : permission.ToString();
        }

        public PermissionAttributeBase(object module, object permission)
            : this(module.ToString() + ":" + permission)
        {
        }

        public PermissionAttributeBase(object module, object submodule, object permission)
            : this(module.ToString() + ":" + submodule + ":" + permission)
        {
        }

        public string Permission { get; private set; }
    }
}