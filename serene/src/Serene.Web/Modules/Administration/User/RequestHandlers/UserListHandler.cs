using MyRow = Serene.Administration.UserRow;
using MyRequest = Serene.Administration.UserListRequest;
using MyResponse = Serenity.Services.ListResponse<Serene.Administration.UserRow>;

namespace Serene.Administration;

public interface IUserListHandler : IListHandler<MyRow, MyRequest, MyResponse> { }

public class UserListHandler(IRequestContext context)
    : ListRequestHandler<MyRow, MyRequest, MyResponse>(context), IUserListHandler
{
}