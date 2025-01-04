using MyRow = Serene.Administration.UserRow;
using MyRequest = Serenity.Services.RetrieveRequest;
using MyResponse = Serenity.Services.RetrieveResponse<Serene.Administration.UserRow>;

namespace Serene.Administration;

public interface IUserRetrieveHandler : IRetrieveHandler<MyRow, MyRequest, MyResponse> { }

public class UserRetrieveHandler(IRequestContext context)
    : RetrieveRequestHandler<MyRow, MyRequest, MyResponse>(context), IUserRetrieveHandler
{
}