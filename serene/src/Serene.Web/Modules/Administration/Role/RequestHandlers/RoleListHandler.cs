using MyRow = Serene.Administration.RoleRow;

namespace Serene.Administration;

public interface IRoleListHandler : IListHandlerAsync<MyRow> { }

public class RoleListHandler(IRequestContext context)
    : ListRequestHandlerAsync<MyRow>(context), IRoleListHandler
{
}