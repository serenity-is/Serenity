using MyRequest = Serenity.Services.RetrieveRequest;
using MyResponse = Serenity.Services.RetrieveResponse<Serenity.Demo.Northwind.CategoryLangRow>;
using MyRow = Serenity.Demo.Northwind.CategoryLangRow;

namespace Serenity.Demo.Northwind;

public interface ICategoryLangRetrieveHandler : IRetrieveHandler<MyRow, MyRequest, MyResponse> { }

public class CategoryLangRetrieveHandler(IRequestContext context) :
    RetrieveRequestHandler<MyRow, MyRequest, MyResponse>(context), ICategoryLangRetrieveHandler
{
}