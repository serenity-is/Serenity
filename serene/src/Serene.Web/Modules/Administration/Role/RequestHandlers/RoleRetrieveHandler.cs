using MyRow = Serene.Administration.RoleRow;

namespace Serene.Administration;

public interface IRoleRetrieveHandler : IRetrieveHandlerAsync<MyRow> { }
public class RoleRetrieveHandler(IRequestContext context)
    : RetrieveRequestHandlerAsync<MyRow>(context), IRoleRetrieveHandler
{
}