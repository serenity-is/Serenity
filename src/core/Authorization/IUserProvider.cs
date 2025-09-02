
namespace Serenity.Abstractions;

/// <summary>
/// Abstraction that is a combination of IUserAccessor, IImpersonator, IUserClaimCreator, IUserRetrieveService and IUserCacheInvalidator.
/// Note that although the <see cref="DefaultUserProvider"/> implements IImpersonator, its methods may throw exceptions if the underlying 
/// IUserAccessor does not implement IImpersonator.
/// </summary>
public interface IUserProvider : IUserAccessor, IUserRetrieveService, IUserClaimCreator, IImpersonator, IRemoveCachedUser, IRemoveAll
{
}