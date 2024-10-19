
namespace Serenity.Abstractions;

/// <summary>
/// Abstraction that is a combination of IUserAccessor, IImpersonator, IUserClaimCreator, IUserRetrieveService and IUserCacheInvalidator.
/// Note that although it implements IImpersonator, its methods may throw exceptions if the underlying 
/// IUserAccessor does not implement IImpersonator.
/// It implements IUserCacheInvalidator but its methods may silently ignore or throw exceptions if the underlying IUserRetrieveService
/// does not implement IUserCacheInvalidator. The DefaultUserRetriever implementation does not throw exceptions in such cases and tries
/// to handle them gracefully.
/// </summary>
public interface IUserProvider : IUserAccessor, IUserRetrieveService, IUserClaimCreator, IImpersonator, IUserCacheInvalidator
{
}