namespace Serenity.Services;

/// <summary>
/// Authorizes access to a service method by reading one of 
/// <see cref="DeletePermissionAttribute"/>, <see cref="ModifyPermissionAttribute"/>
/// or <see cref="ReadPermissionAttribute"/> from the target type, which is usually
/// a Row class.
/// </summary>
public class AuthorizeDeleteAttribute : ServiceAuthorizeAttribute
{
    /// <summary>
    /// Creates a new instance of the attribute
    /// </summary>
    /// <param name="sourceType">Source type</param>
    public AuthorizeDeleteAttribute(Type sourceType)
        : base(sourceType, typeof(DeletePermissionAttribute),
              typeof(ModifyPermissionAttribute), typeof(ReadPermissionAttribute))
    {
    }
}