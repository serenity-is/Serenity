namespace Serenity.Services;

/// <summary>
/// Authorizes access to a service method by reading <see cref="ReadPermissionAttribute"/>.
/// It also allows lookup mode access (as an OR permission) if the source type has
/// <see cref="ServiceLookupPermissionAttribute"/>.
/// </summary>
public class AuthorizeListAttribute : ServiceAuthorizeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizeListAttribute"/> class.
    /// </summary>
    /// <param name="sourceType">The source type.</param>
    public AuthorizeListAttribute(Type sourceType)
        : base(sourceType, typeof(ReadPermissionAttribute))
    {
        OrPermission = sourceType.GetCustomAttribute<ServiceLookupPermissionAttribute>(inherit: false)?.Permission;
    }
}