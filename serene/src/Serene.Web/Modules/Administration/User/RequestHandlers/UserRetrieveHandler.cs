using MyRow = Serene.Administration.UserRow;

namespace Serene.Administration;

public interface IUserRetrieveHandler : IRetrieveHandlerAsync<MyRow> { }

public class UserRetrieveHandler(IRequestContext context)
    : RetrieveRequestHandlerAsync<MyRow>(context), IUserRetrieveHandler
{
}