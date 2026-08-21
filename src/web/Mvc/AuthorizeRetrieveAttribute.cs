namespace Serenity.Services;

/// <summary>
/// Authorizes retrieve access to a service method by reading <see cref="ReadPermissionAttribute"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AuthorizeRetrieveAttribute"/> class.
/// </remarks>
/// <param name="sourceType">The source type.</param>
public class AuthorizeRetrieveAttribute(Type sourceType) 
    : ServiceAuthorizeAttribute(sourceType, typeof(ReadPermissionAttribute))
{
}