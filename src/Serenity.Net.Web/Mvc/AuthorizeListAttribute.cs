namespace Serenity.Services
{
    /// <summary>
    /// Authorizes a service by target type's ReadPermission or ServiceLookupPermission attributes.
    /// </summary>
    public class AuthorizeListAttribute : ServiceAuthorizeAttribute
    {
        public AuthorizeListAttribute(Type sourceType)
            : base(sourceType, typeof(ReadPermissionAttribute))
        {
            OrPermission = sourceType.GetAttribute<ServiceLookupPermissionAttribute>()?.Permission;
        }
    }
}