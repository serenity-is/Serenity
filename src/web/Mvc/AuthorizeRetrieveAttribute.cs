namespace Serenity.Services;

/// <summary>
/// Authorizes retrieve access to a service method by reading <see cref="ReadPermissionAttribute"/>.
/// <see cref="ServiceLookupPermissionAttribute"/>
/// </summary>
/// <remarks>
/// Creates a new instance of the attribute
/// </remarks>
/// <param name="sourceType">Source type</param>
public class AuthorizeRetrieveAttribute(Type sourceType) 
    : ServiceAuthorizeAttribute(sourceType, typeof(ReadPermissionAttribute))
{
}