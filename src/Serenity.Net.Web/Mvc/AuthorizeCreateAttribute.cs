namespace Serenity.Services;

/// <summary>
/// Authorizes access to a service method by reading one of 
/// <see cref="InsertPermissionAttribute"/>, <see cref="ModifyPermissionAttribute"/>
/// or <see cref="ReadPermissionAttribute"/> from the target type, which is usually
/// a Row class.
/// </summary>
public class AuthorizeCreateAttribute : ServiceAuthorizeAttribute
{
    /// <summary>
    /// Creates a new instance of the attribute
    /// </summary>
    /// <param name="sourceType">Source type</param>
    public AuthorizeCreateAttribute(Type sourceType)
        : base(sourceType, typeof(InsertPermissionAttribute), 
              typeof(ModifyPermissionAttribute), typeof(ReadPermissionAttribute))
    {
    }
}