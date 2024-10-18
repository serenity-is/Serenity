using MyRequest = Serenity.Services.SaveRequest<Serenity.Demo.Northwind.CustomerRow>;
using MyResponse = Serenity.Services.SaveResponse;
using MyRow = Serenity.Demo.Northwind.CustomerRow;

namespace Serenity.Demo.Northwind;

public interface ICustomerSaveHandler : ISaveHandler<MyRow, MyRequest, MyResponse> { }

public class CustomerSaveHandler(IRequestContext context) :
    SaveRequestHandler<MyRow, MyRequest, MyResponse>(context), ICustomerSaveHandler
{
}