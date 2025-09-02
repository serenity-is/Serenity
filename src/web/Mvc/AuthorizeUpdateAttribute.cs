namespace Serenity.Services;

/// <summary>
/// Authorizes access to a service method by reading one of 
/// <see cref="UpdatePermissionAttribute"/>, <see cref="ModifyPermissionAttribute"/>
/// or <see cref="ReadPermissionAttribute"/> from the target type, which is usually
/// a Row class.
/// </summary>
/// <remarks>
/// Creates a new instance of the attribute
/// </remarks>
/// <param name="sourceType">Source type</param>
public class AuthorizeUpdateAttribute(Type sourceType) : ServiceAuthorizeAttribute(sourceType, typeof(UpdatePermissionAttribute),
          typeof(ModifyPermissionAttribute), typeof(ReadPermissionAttribute))
{
}