namespace Serenity.Services;

/// <summary>
/// Authorizes access to a service method by reading <see cref="ReadPermissionAttribute"/>.
/// It also allows lookup mode access (as OrPermission) if the source type has 
/// <see cref="ServiceLookupPermissionAttribute"/>
/// </summary>
public class AuthorizeListAttribute : ServiceAuthorizeAttribute
{
    /// <summary>
    /// Creates a new instance of the attribute
    /// </summary>
    /// <param name="sourceType">Source type</param>
    public AuthorizeListAttribute(Type sourceType)
        : base(sourceType, typeof(ReadPermissionAttribute))
    {
        OrPermission = sourceType.GetAttribute<ServiceLookupPermissionAttribute>()?.Permission;
    }
}