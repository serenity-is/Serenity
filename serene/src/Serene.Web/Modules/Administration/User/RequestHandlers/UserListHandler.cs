using MyRow = Serene.Administration.UserRow;

namespace Serene.Administration;

public interface IUserListHandler : IListHandlerAsync<MyRow, UserListRequest, ListResponse<MyRow>> { }

public class UserListHandler(IRequestContext context)
    : ListRequestHandlerAsync<MyRow, UserListRequest, ListResponse<MyRow>>(context), IUserListHandler
{
}
