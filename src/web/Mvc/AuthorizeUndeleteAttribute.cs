namespace Serenity.Services;

/// <summary>
/// Authorizes access to a service method by reading one of
/// <see cref="UndeletePermissionAttribute"/>, <see cref="DeletePermissionAttribute"/>,
/// <see cref="ModifyPermissionAttribute"/> or <see cref="ReadPermissionAttribute"/>
/// from the target type, which is usually a Row class.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AuthorizeUndeleteAttribute"/> class.
/// </remarks>
/// <param name="sourceType">The source type.</param>
public class AuthorizeUndeleteAttribute(Type sourceType) : ServiceAuthorizeAttribute(sourceType,
          typeof(UndeletePermissionAttribute), typeof(DeletePermissionAttribute),
          typeof(ModifyPermissionAttribute), typeof(ReadPermissionAttribute))
{
}
