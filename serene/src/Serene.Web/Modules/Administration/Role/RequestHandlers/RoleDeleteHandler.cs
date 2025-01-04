using MyRow = Serene.Administration.RoleRow;
using MyRequest = Serenity.Services.DeleteRequest;
using MyResponse = Serenity.Services.DeleteResponse;

namespace Serene.Administration;

public interface IRoleDeleteHandler : IDeleteHandler<MyRow, MyRequest, MyResponse> { }

public class RoleDeleteHandler(IRequestContext context)
    : DeleteRequestHandler<MyRow, MyRequest, MyResponse>(context), IRoleDeleteHandler
{
}