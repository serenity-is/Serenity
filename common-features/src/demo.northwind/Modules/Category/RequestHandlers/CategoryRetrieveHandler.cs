using MyRequest = Serenity.Services.RetrieveRequest;
using MyResponse = Serenity.Services.RetrieveResponse<Serenity.Demo.Northwind.CategoryRow>;
using MyRow = Serenity.Demo.Northwind.CategoryRow;

namespace Serenity.Demo.Northwind;

public interface ICategoryRetrieveHandler : IRetrieveHandler<MyRow, MyRequest, MyResponse> { }

public class CategoryRetrieveHandler(IRequestContext context) :
    RetrieveRequestHandler<MyRow, MyRequest, MyResponse>(context), ICategoryRetrieveHandler
{
}