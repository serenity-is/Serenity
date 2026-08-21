namespace Serenity.Services;

/// <summary>
/// Authorizes access to a service method by reading one of
/// <see cref="DeletePermissionAttribute"/>, <see cref="ModifyPermissionAttribute"/>
/// or <see cref="ReadPermissionAttribute"/> from the target type, which is usually
/// a Row class.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AuthorizeDeleteAttribute"/> class.
/// </remarks>
/// <param name="sourceType">The source type.</param>
public class AuthorizeDeleteAttribute(Type sourceType) : ServiceAuthorizeAttribute(sourceType, typeof(DeletePermissionAttribute),
          typeof(ModifyPermissionAttribute), typeof(ReadPermissionAttribute))
{
}