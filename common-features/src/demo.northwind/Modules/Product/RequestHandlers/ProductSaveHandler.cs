using MyRequest = Serenity.Services.SaveRequest<Serenity.Demo.Northwind.ProductRow>;
using MyResponse = Serenity.Services.SaveResponse;
using MyRow = Serenity.Demo.Northwind.ProductRow;

namespace Serenity.Demo.Northwind;

public interface IProductSaveHandler : ISaveHandler<MyRow, MyRequest, MyResponse> { }

public class ProductSaveHandler(IRequestContext context) :
    SaveRequestHandler<MyRow, MyRequest, MyResponse>(context), IProductSaveHandler
{
}