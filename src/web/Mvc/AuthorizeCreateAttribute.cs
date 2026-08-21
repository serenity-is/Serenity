namespace Serenity.Services;

/// <summary>
/// Authorizes access to a service method by reading one of
/// <see cref="InsertPermissionAttribute"/>, <see cref="ModifyPermissionAttribute"/>
/// or <see cref="ReadPermissionAttribute"/> from the target type, which is usually
/// a Row class.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AuthorizeCreateAttribute"/> class.
/// </remarks>
/// <param name="sourceType">The source type.</param>
public class AuthorizeCreateAttribute(Type sourceType) : ServiceAuthorizeAttribute(sourceType, typeof(InsertPermissionAttribute), 
          typeof(ModifyPermissionAttribute), typeof(ReadPermissionAttribute))
{
}