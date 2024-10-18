using Microsoft.AspNetCore.DataProtection;
using MyRow = Serene.Administration.UserRow;

namespace Serene.Administration.Endpoints;

[Route("Services/Administration/User/[action]")]
[ConnectionKey(typeof(MyRow)), ServiceAuthorize(typeof(MyRow))]
public class UserEndpoint : ServiceEndpoint
{
    [HttpPost, AuthorizeCreate(typeof(MyRow))]
    public SaveResponse Create(IUnitOfWork uow, SaveRequest<MyRow> request, [FromServices] IUserSaveHandler handler)
    {
        return handler.Create(uow, request);
    }

    [HttpPost, AuthorizeUpdate(typeof(MyRow))]
    public SaveResponse Update(IUnitOfWork uow, SaveRequest<MyRow> request, [FromServices] IUserSaveHandler handler)
    {
        return handler.Update(uow, request);
    }

    [HttpPost, AuthorizeDelete(typeof(MyRow))]
    public DeleteResponse Delete(IUnitOfWork uow, DeleteRequest request, [FromServices] IUserDeleteHandler handler)
    {
        return handler.Delete(uow, request);
    }

    public RetrieveResponse<MyRow> Retrieve(IDbConnection connection, RetrieveRequest request, [FromServices] IUserRetrieveHandler handler)
    {
        return handler.Retrieve(connection, request);
    }

    public ListResponse<MyRow> List(IDbConnection connection, UserListRequest request, [FromServices] IUserListHandler handler)
    {
        return handler.List(connection, request);
    }       
}