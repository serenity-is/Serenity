
namespace Serenity.Data
{
    public class ReadPermissionAttribute : PermissionAttributeBase
    {
        public ReadPermissionAttribute(object permission)
            : base(permission)
        {
        }

        public ReadPermissionAttribute(object module, object permission)
            : base(module, permission)
        {
        }

        public ReadPermissionAttribute(object module, object submodule, object permission)
            : base(module, submodule, permission)
        {
        }
    }
}