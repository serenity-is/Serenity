using System.Data;
using Microsoft.AspNetCore.Mvc;
using MyRepository = Serenity.Extensions.Repositories.UserPreferenceRepository;
using MyRow = Serenity.Extensions.Entities.UserPreferenceRow;

namespace Serenity.Extensions.Endpoints;

[Route("Services/Extensions/UserPreference/[action]")]
[ConnectionKey(typeof(MyRow)), ServiceAuthorize]
public class UserPreferenceEndpoint : ServiceEndpoint
{
    [HttpPost]
    public ServiceResponse Update(IUnitOfWork uow, UserPreferenceUpdateRequest request)
    {
        return new MyRepository(Context).Update(uow, request);
    }

    public UserPreferenceRetrieveResponse Retrieve(IDbConnection connection, UserPreferenceRetrieveRequest request)
    {
        return new MyRepository(Context).Retrieve(connection, request);
    }
}
