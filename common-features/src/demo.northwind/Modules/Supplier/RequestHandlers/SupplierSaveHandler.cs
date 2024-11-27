using MyRequest = Serenity.Services.SaveRequest<Serenity.Demo.Northwind.SupplierRow>;
using MyResponse = Serenity.Services.SaveResponse;
using MyRow = Serenity.Demo.Northwind.SupplierRow;

namespace Serenity.Demo.Northwind;

public interface ISupplierSaveHandler : ISaveHandler<MyRow, MyRequest, MyResponse> { }

public class SupplierSaveHandler(IRequestContext context) :
    SaveRequestHandler<MyRow, MyRequest, MyResponse>(context), ISupplierSaveHandler
{
}