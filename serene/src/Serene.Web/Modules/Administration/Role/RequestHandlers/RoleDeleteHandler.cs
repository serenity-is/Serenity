using MyRow = Serene.Administration.RoleRow;

namespace Serene.Administration;

public interface IRoleDeleteHandler : IDeleteHandlerAsync<MyRow> { }

public class RoleDeleteHandler(IRequestContext context)
    : DeleteRequestHandlerAsync<MyRow>(context), IRoleDeleteHandler
{
}