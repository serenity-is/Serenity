using MyRequest = Serenity.Services.ListRequest;
using MyResponse = Serenity.Services.ListResponse<Serenity.Demo.Northwind.CategoryRow>;
using MyRow = Serenity.Demo.Northwind.CategoryRow;

namespace Serenity.Demo.Northwind;

public interface ICategoryListHandler : IListHandler<MyRow, MyRequest, MyResponse> { }

public class CategoryListHandler(IRequestContext context) :
    ListRequestHandler<MyRow, MyRequest, MyResponse>(context), ICategoryListHandler
{
}