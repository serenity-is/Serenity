using MyRepository = Serenity.Extensions.Repositories.UserPreferenceRepository;
using MyRow = Serenity.Extensions.Entities.UserPreferenceRow;

namespace Serenity.Extensions.Endpoints;

/// <summary>
/// Service endpoint for managing user preferences.
/// </summary>
[Route("Services/Extensions/UserPreference/[action]")]
[ConnectionKey(typeof(MyRow)), ServiceAuthorize]
public class UserPreferenceEndpoint : ServiceEndpoint
{
    /// <summary>
    /// Updates or deletes a user preference.
    /// </summary>
    /// <param name="uow">The unit of work.</param>
    /// <param name="request">The update request.</param>
    /// <returns>The save response.</returns>
    [HttpPost]
    public ServiceResponse Update(IUnitOfWork uow, UserPreferenceUpdateRequest request)
    {
        return new MyRepository(Context).Update(uow, request);
    }

    /// <summary>
    /// Retrieves a user preference.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="request">The retrieve request.</param>
    /// <returns>The retrieve response.</returns>
    public UserPreferenceRetrieveResponse Retrieve(IDbConnection connection, UserPreferenceRetrieveRequest request)
    {
        return new MyRepository(Context).Retrieve(connection, request);
    }
}
