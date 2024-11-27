using MyRequest = Serenity.Services.SaveRequest<Serenity.Demo.Northwind.ProductLangRow>;
using MyResponse = Serenity.Services.SaveResponse;
using MyRow = Serenity.Demo.Northwind.ProductLangRow;

namespace Serenity.Demo.Northwind;

public interface IProductLangSaveHandler : ISaveHandler<MyRow, MyRequest, MyResponse> { }

public class ProductLangSaveHandler(IRequestContext context) :
    SaveRequestHandler<MyRow, MyRequest, MyResponse>(context), IProductLangSaveHandler
{
}