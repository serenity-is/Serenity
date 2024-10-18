using MyRow = Serene.Administration.RoleRow;

namespace Serene.Administration.Endpoints;

[Route("Services/Administration/Role/[action]")]
[ConnectionKey(typeof(MyRow)), ServiceAuthorize(typeof(MyRow))]
public class RoleEndpoint : ServiceEndpoint
{
    [HttpPost, AuthorizeCreate(typeof(MyRow))]
    public SaveResponse Create(IUnitOfWork uow, SaveRequest<MyRow> request, [FromServices] IRoleSaveHandler handler)
    {
        return handler.Create(uow, request);
    }

    [HttpPost, AuthorizeUpdate(typeof(MyRow))]
    public SaveResponse Update(IUnitOfWork uow, SaveRequest<MyRow> request, [FromServices] IRoleSaveHandler handler)
    {
        return handler.Update(uow, request);
    }

    [HttpPost, AuthorizeDelete(typeof(MyRow))]
    public DeleteResponse Delete(IUnitOfWork uow, DeleteRequest request, [FromServices] IRoleDeleteHandler handler)
    {
        return handler.Delete(uow, request);
    }

    public RetrieveResponse<MyRow> Retrieve(IDbConnection connection, RetrieveRequest request, [FromServices] IRoleRetrieveHandler handler)
    {
        return handler.Retrieve(connection, request);
    }

    public ListResponse<MyRow> List(IDbConnection connection, ListRequest request, [FromServices] IRoleListHandler handler)
    {
        return handler.List(connection, request);
    }
}
