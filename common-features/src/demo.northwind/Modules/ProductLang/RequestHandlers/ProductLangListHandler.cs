using MyRequest = Serenity.Services.ListRequest;
using MyResponse = Serenity.Services.ListResponse<Serenity.Demo.Northwind.ProductLangRow>;
using MyRow = Serenity.Demo.Northwind.ProductLangRow;

namespace Serenity.Demo.Northwind;

public interface IProductLangListHandler : IListHandler<MyRow, MyRequest, MyResponse> { }

public class ProductLangListHandler(IRequestContext context) :
    ListRequestHandler<MyRow, MyRequest, MyResponse>(context), IProductLangListHandler
{
}