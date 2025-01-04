using MyRow = Serene.Administration.RoleRow;
using MyRequest = Serenity.Services.SaveRequest<Serene.Administration.RoleRow>;
using MyResponse = Serenity.Services.SaveResponse;

namespace Serene.Administration;

public interface IRoleSaveHandler : ISaveHandler<MyRow, MyRequest, MyResponse> { }

public class RoleSaveHandler(IRequestContext context)
    : SaveRequestHandler<MyRow, MyRequest, MyResponse>(context), IRoleSaveHandler
{
    protected override void InvalidateCacheOnCommit()
    {
        base.InvalidateCacheOnCommit();

        Cache.InvalidateOnCommit(UnitOfWork, UserPermissionRow.Fields);
        Cache.InvalidateOnCommit(UnitOfWork, RolePermissionRow.Fields);
    }
}