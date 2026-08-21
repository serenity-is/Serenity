
namespace Serenity.Abstractions;

/// <summary>
/// Combines user access, retrieval, claim creation, impersonation, and cache invalidation into a single abstraction.
/// </summary>
/// <remarks>
/// Although <see cref="DefaultUserProvider"/> implements <see cref="IImpersonator"/>, its impersonation methods
/// may throw <see cref="InvalidOperationException"/> when the underlying <see cref="IUserAccessor"/> does not
/// implement <see cref="IImpersonator"/>.
/// </remarks>
public interface IUserProvider : IUserAccessor, IUserRetrieveService, IUserClaimCreator, IImpersonator, IRemoveCachedUser, IRemoveAll
{
}