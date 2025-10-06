using MyRow = Serenity.Demo.Northwind.CustomerRow;

namespace Serenity.Demo.Northwind;

public interface ICustomerSaveHandler : ISaveHandler<MyRow> { }

public class CustomerSaveHandler(IRequestContext context) :
    SaveRequestHandler<MyRow>(context), ICustomerSaveHandler
{
}