using MyRequest = Serenity.Services.DeleteRequest;
using MyResponse = Serenity.Services.DeleteResponse;
using MyRow = Serenity.Demo.Northwind.CategoryLangRow;

namespace Serenity.Demo.Northwind;

public interface ICategoryLangDeleteHandler : IDeleteHandler<MyRow, MyRequest, MyResponse> { }

public class CategoryLangDeleteHandler(IRequestContext context) :
    DeleteRequestHandler<MyRow, MyRequest, MyResponse>(context), ICategoryLangDeleteHandler
{
}