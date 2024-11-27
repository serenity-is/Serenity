using MyRequest = Serenity.Services.DeleteRequest;
using MyResponse = Serenity.Services.DeleteResponse;
using MyRow = Serenity.Demo.Northwind.SupplierRow;

namespace Serenity.Demo.Northwind;

public interface ISupplierDeleteHandler : IDeleteHandler<MyRow, MyRequest, MyResponse> { }

public class SupplierDeleteHandler(IRequestContext context) :
    DeleteRequestHandler<MyRow, MyRequest, MyResponse>(context), ISupplierDeleteHandler
{
}